using System;

namespace Proutines
{
    internal class OnFinishedInstruction : IYieldInstruction
    {
        private readonly IYieldInstruction instruction;
        private readonly Action onFinished;

        public OnFinishedInstruction(IYieldInstruction instruction, Action onFinished)
        {
            this.instruction = instruction;
            this.onFinished = onFinished;
        }

        public bool Yield()
        {
            if (instruction.Yield())
            {
                return true;
            }

            onFinished.Invoke();
            return false;
        }

        public bool Resetable => instruction.Resetable;
        public void Reset()
        {
            instruction.Reset();
        }
    }

    internal class OnFinishedInstructionWithParameter<T> : IYieldInstruction
    {
        private readonly IYieldOperation<T> instruction;
        private readonly Action<T> onFinished;

        public OnFinishedInstructionWithParameter(IYieldOperation<T> instruction, Action<T> onFinished)
        {
            this.instruction = instruction;
            this.onFinished = onFinished;
        }

        public bool Yield()
        {
            if (instruction.Yield())
            {
                return true;
            }

            onFinished.Invoke(instruction.Value);
            return false;
        }

        public bool Resetable => instruction.Resetable;
        public void Reset()
        {
            instruction.Reset();
        }
    }

    internal class OnFinishedInstruction<T> : YieldOperationBase<T>
    {
        private readonly IYieldInstruction instruction;
        private readonly Func<T> onFinished;
        private readonly T value;

        public OnFinishedInstruction(IYieldInstruction instruction, Func<T> onFinished)
        {
            this.instruction = instruction;
            this.onFinished = onFinished;
        }

        public OnFinishedInstruction(IYieldInstruction instruction, T value)
        {
            this.instruction = instruction;
            this.value = value;
            onFinished = null;
        }

        public override bool Resetable => instruction.Resetable;

        protected override void Reset()
        {
            instruction.Reset();
        }

        protected override YieldValue WillYield()
        {
            if (instruction.Yield())
            {
                return YieldValue.Continue();
            }

            return YieldValue.Finish(onFinished == null ? value : onFinished.Invoke());
        }
    }

    internal class OnFinishedInstruction<TIn, TOut> : YieldOperationBase<TOut>
    {
        private readonly IYieldOperation<TIn> operation;
        private readonly Func<TIn, TOut> onFinished;

        public OnFinishedInstruction(IYieldOperation<TIn> operation, Func<TIn, TOut> onFinished)
        {
            this.operation = operation;
            this.onFinished = onFinished;
        }

        public override bool Resetable => operation.Resetable;

        protected override void Reset()
        {
            operation.Reset();
        }

        protected override YieldValue WillYield()
        {
            if (operation.Yield())
            {
                return YieldValue.Continue();
            }

            return YieldValue.Finish(onFinished.Invoke(operation.Value));
        }
    }
}