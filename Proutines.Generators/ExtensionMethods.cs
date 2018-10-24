using System;

namespace Proutines.Generators
{
    public static class ExtensionMethods
    {
        public static IYieldGenerator<T> LoopGeneratorForever<T>(this IYieldGenerator<T> yieldGenerator)
        {
            if (!yieldGenerator.Resetable)
            {
                throw new ResetNotSupportedException(yieldGenerator);
            }

            return new LoopForeverYieldGenerator<T>(yieldGenerator);
        }

        public static IYieldGenerator<T> Foreach<T>(this IYieldGenerator<T> generator, Action<T> func)
        {
            return new ForeachYieldGenerator<T>(generator, func);
        }

        public static IYieldGenerator<T> OnChange<T>(this IYieldGenerator<T> generator, Action<T> func)
        {
            return new OnChangeYieldGenerator<T>(generator, func);
        }

        public static IYieldGenerator<TOut> Select<TIn, TOut>(this IYieldGenerator<TIn> generator, Func<TIn, TOut> func)
        {
            return new SelectYieldGenerator<TIn, TOut>(generator, func);
        }

        public static IYieldGenerator<float> ToFloat(this IYieldGenerator<double> generator)
        {
            return new SelectYieldGenerator<double, float>(generator, (t) => (float)t);
        }

        public static IYieldGenerator<double> ToDouble(this IYieldGenerator<float> generator)
        {
            return new SelectYieldGenerator<float, double>(generator, (t) => t);
        }

        public static IYieldGenerator<float> WaitForSecondsEach(this IYieldGenerator<float> generator)
        {
            return new WaitForSecondsGenerator(generator);
        }

        public static IYieldGenerator<double> WaitForSecondsEach(this IYieldGenerator<double> generator)
        {
            return new WaitForSecondsGeneratorDouble(generator);
        }

        public static IYieldGenerator<int> WaitForSecondsEach(this IYieldGenerator<int> generator)
        {
            return new WaitForSecondsGeneratorInt(generator);
        }

        public static IYieldOperation<T> ReturnLastValue<T>(this IYieldGenerator<T> generator)
        {
            return new ReturnLastGeneratorValue<T>(generator);
        }

        public static IYieldGenerator<T> InjectWaitInstruction<T>(this IYieldGenerator<T> generator, IYieldInstruction instruction)
        {
            if (!instruction.Resetable)
            {
                throw new ResetNotSupportedException(instruction);
            }

            return new WaitForInstructionGenerator<T>(generator, instruction);
        }

        public static IYieldGenerator<T> InjectWaitInstruction<T>(this IYieldGenerator<T> generator, Func<T, IYieldInstruction> instruction)
        {
            return new WaitForCallbackInstructionGenerator<T>(generator, instruction);
        }

        public static IYieldGenerator<T> StopWhen<T>(this IYieldGenerator<T> generator, IYieldInstruction instruction)
        {
            return new StopWhenInstructionGenerator<T>(generator, instruction);
        }

        public static IYieldGenerator<T> StopWhenFalse<T>(this IYieldGenerator<T> generator, Func<bool> when)
        {
            return new StopWhenFunctionInstructionGenerator<T>(generator, when);
        }

        public static IYieldGenerator<T> StopWhenFalse<T>(this IYieldGenerator<T> generator, Func<T, bool> when)
        {
            return new StopWhenFunctionWithParameterInstructionGenerator<T>(generator, when);
        }

        public static IYieldGenerator<T> LoopGenerator<T>(this IYieldGenerator<T> generator, int maxCycles)
        {
            if (maxCycles <= 1)
                return generator;

            if (!generator.Resetable)
            {
                throw new ResetNotSupportedException(generator);
            }

            return new MaxCyclesGeneratorInstruction<T>(generator, maxCycles);
        }

        public static ILerpGenerator PingPongForever(this ILerpGenerator generator)
        {
            if (!generator.Resetable)
            {
                throw new ResetNotSupportedException(generator);
            }

            return new PingPongResetableYieldGenerator(generator).LoopGeneratorForever().MarkAsLerpable();
        }

        public static ILerpGenerator PingPong(this ILerpGenerator generator, int maxCycles)
        {
            if (maxCycles <= 1)
                return new PingPongResetableYieldGenerator(generator);

            if (!generator.Resetable)
            {
                throw new ResetNotSupportedException(generator);
            }

            return new PingPongResetableYieldGenerator(generator).LoopGenerator(maxCycles).MarkAsLerpable();
        }

        private static ILerpGenerator MarkAsLerpable(this IYieldGenerator<double> generator)
        {
            return new Lerpable(generator);
        }
    }
}