using System;

namespace Proutines.Generators
{
    internal class WaitForCallbackInstructionGenerator<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> generator;
        private readonly Func<T, IYieldInstruction> instructionFunc;
        private IYieldInstruction instruction;
        private bool isFirstTime = true;
        private bool shouldStopAfterInstruction;

        public WaitForCallbackInstructionGenerator(IYieldGenerator<T> generator, Func<T, IYieldInstruction> instructionFunc)
        {
            this.generator = generator;
            this.instructionFunc = instructionFunc;
        }

        public T GetValue()
        {
            return generator.GetValue();
        }

        public bool Yield()
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                shouldStopAfterInstruction = !generator.Yield();
            }

            if (instruction == null)
                instruction = instructionFunc.Invoke(generator.GetValue());

            if (!instruction.Yield())
            {
                instruction = null;

                if (shouldStopAfterInstruction)
                {
                    return false;
                }

                if (!generator.Yield())
                {
                    return false;
                }
            }

            return true;
        }

        public bool Resetable => generator.Resetable;
        public void Reset()
        {
            isFirstTime = true;
            instruction = null;
            generator.Reset();
        }
    }

    internal class WaitForInstructionGenerator<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> generator;
        private readonly IYieldInstruction instruction;
        private bool isFirstTime = true;

        public WaitForInstructionGenerator(IYieldGenerator<T> generator, IYieldInstruction instruction)
        {
            if (!instruction.Resetable)
                throw new ResetNotSupportedException(instruction);

            this.generator = generator;
            this.instruction = instruction;
        }

        public T GetValue()
        {
            return generator.GetValue();
        }

        public bool Yield()
        {
            if (isFirstTime)
            {
                isFirstTime = false;
                return generator.Yield();
            }

            if (!instruction.Yield())
            {
                instruction.Reset();
                if (!generator.Yield())
                {
                    return false;
                }
            }

            return true;
        }

        public bool Resetable => generator.Resetable;
        public void Reset()
        {
            isFirstTime = true;
            generator.Reset();
            instruction.Reset();
        }
    }

    internal class StopWhenInstructionGenerator<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> generator;
        private readonly IYieldInstruction instruction;

        public StopWhenInstructionGenerator(IYieldGenerator<T> generator, IYieldInstruction instruction)
        {
            this.generator = generator;
            this.instruction = instruction;
        }

        public T GetValue()
        {
            return generator.GetValue();
        }

        public bool Yield()
        {
            if (instruction.Yield())
            {
                if (!generator.Yield())
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool Resetable => generator.Resetable && instruction.Resetable;
        public void Reset()
        {
            generator.Reset();
            instruction.Reset();
        }
    }

    internal class StopWhenFunctionInstructionGenerator<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> generator;
        private readonly Func<bool> when;

        public StopWhenFunctionInstructionGenerator(IYieldGenerator<T> generator, Func<bool> when)
        {
            this.generator = generator;
            this.when = when;
        }

        public T GetValue()
        {
            return generator.GetValue();
        }

        public bool Yield()
        {
            return generator.Yield() && when.Invoke();
        }

        public bool Resetable => generator.Resetable;
        public void Reset()
        {
            generator.Reset();
        }
    }

    internal class StopWhenFunctionWithParameterInstructionGenerator<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> generator;
        private readonly Func<T, bool> when;

        public StopWhenFunctionWithParameterInstructionGenerator(IYieldGenerator<T> generator, Func<T, bool> when)
        {
            this.generator = generator;
            this.when = when;
        }

        public T GetValue()
        {
            return generator.GetValue();
        }

        public bool Yield()
        {
            return generator.Yield() && when.Invoke(generator.GetValue());
        }

        public bool Resetable => generator.Resetable;
        public void Reset()
        {
            generator.Reset();
        }
    }
}