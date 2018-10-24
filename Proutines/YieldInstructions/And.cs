using System;

namespace Proutines
{
    internal class And : IYieldInstruction
    {
        private readonly IYieldInstruction a;
        private readonly IYieldInstruction b;

        private bool isAFinished;
        private bool isBFinished;

        public And(IYieldInstruction a, IYieldInstruction b)
        {
            this.a = a;
            this.b = b;
        }

        public bool Yield()
        {
            if (!isAFinished)
                if (!a.Yield())
                {
                    isAFinished = true;
                }
            if (!isBFinished)
                if (!b.Yield())
                {
                    isBFinished = true;
                }

            return !(isAFinished && isBFinished);
        }

        public bool Resetable => a.Resetable && b.Resetable;
        public void Reset()
        {
            isAFinished = false;
            isBFinished = false;
            a.Reset();
            b.Reset();
        }
    }

    internal class After : IYieldInstruction
    {
        private readonly IYieldInstruction a;
        private readonly IYieldInstruction b;

        private bool isAFinished;

        public After(IYieldInstruction a, IYieldInstruction b)
        {
            this.a = a;
            this.b = b;
        }

        public bool Yield()
        {
            if (!isAFinished)
            {
                if (!a.Yield())
                {
                    isAFinished = true;
                }
            }

            if (isAFinished)
            {
                if (!b.Yield())
                {
                    return false;
                }
            }

            return true;
        }

        public bool Resetable => a.Resetable && b.Resetable;
        public void Reset()
        {
            isAFinished = false;
            a.Reset();
            b.Reset();
        }
    }
    internal class AfterFunction : IYieldInstruction
    {
        private readonly IYieldInstruction a;
        private readonly Func<IYieldInstruction> bFunc;
        private IYieldInstruction b;

        private bool isAFinished;

        public AfterFunction(IYieldInstruction a, Func<IYieldInstruction> bFunc)
        {
            this.a = a;
            this.bFunc = bFunc;
        }

        public bool Yield()
        {
            if (!isAFinished)
            {
                if (!a.Yield())
                {
                    isAFinished = true;
                    b = bFunc.Invoke();
                }
            }

            if (isAFinished)
            {
                if (!b.Yield())
                {
                    return false;
                }
            }

            return true;
        }

        public bool Resetable => a.Resetable;
        public void Reset()
        {
            isAFinished = false;
            a.Reset();
            b = null;
        }
    }

    internal class AfterFunctionWithParameter<T> : IYieldInstruction
    {
        private readonly IYieldOperation<T> a;
        private readonly Func<T, IYieldInstruction> bFunc;
        private IYieldInstruction b;

        private bool isAFinished;

        public AfterFunctionWithParameter(IYieldOperation<T> a, Func<T, IYieldInstruction> bFunc)
        {
            this.a = a;
            this.bFunc = bFunc;
        }

        public bool Yield()
        {
            if (!isAFinished)
            {
                if (!a.Yield())
                {
                    isAFinished = true;
                    b = bFunc.Invoke(a.Value);
                }
            }

            if (isAFinished)
            {
                if (!b.Yield())
                {
                    return false;
                }
            }

            return true;
        }

        public bool Resetable => a.Resetable;
        public void Reset()
        {
            isAFinished = false;
            a.Reset();
            b = null;
        }
    }
    
    internal class AfterFunction<T> : YieldOperationBase<T>
    {
        private readonly IYieldInstruction a;
        private readonly Func<IYieldOperation<T>> bFunc;
        private IYieldOperation<T> b;

        private bool isAFinished;

        public AfterFunction(IYieldInstruction a, Func<IYieldOperation<T>> b)
        {
            this.a = a;
            bFunc = b;
        }

        public override bool Resetable => a.Resetable;

        protected override void Reset()
        {
            isAFinished = false;
            a.Reset();
            b = null;
        }

        protected override YieldValue WillYield()
        {
            if (!isAFinished)
            {
                if (!a.Yield())
                {
                    // A is finished
                    isAFinished = true;
                    b = bFunc.Invoke();
                }
            }

            if (isAFinished)
            {
                if (!b.Yield())
                {
                    // B is finished
                    return YieldValue.Finish(b.Value);
                }
            }

            return YieldValue.Continue();
        }
    }

    internal class AfterFunctionWithParameter<TIn,TOut> : YieldOperationBase<TOut>
    {
        private readonly IYieldOperation<TIn> a;
        private readonly Func<TIn, IYieldOperation<TOut>> bFunc;
        private IYieldOperation<TOut> b;

        private bool isAFinished;

        public AfterFunctionWithParameter(IYieldOperation<TIn> a, Func<TIn, IYieldOperation<TOut>> b)
        {
            this.a = a;
            bFunc = b;
        }

        public override bool Resetable => a.Resetable;

        protected override void Reset()
        {
            isAFinished = false;
            a.Reset();
            b = null;
        }

        protected override YieldValue WillYield()
        {
            if (!isAFinished)
            {
                if (!a.Yield())
                {
                    // A is finished
                    isAFinished = true;
                    b = bFunc.Invoke(a.Value);
                }
            }

            if (isAFinished)
            {
                if (!b.Yield())
                {
                    // B is finished
                    return YieldValue.Finish(b.Value);
                }
            }

            return YieldValue.Continue();
        }
    }

    internal class After<T> : YieldOperationBase<T>
    {
        private readonly IYieldInstruction a;
        private readonly IYieldOperation<T> b;

        private bool isAFinished;

        public After(IYieldInstruction a, IYieldOperation<T> b)
        {
            this.a = a;
            this.b = b;
        }

        public override bool Resetable => a.Resetable && b.Resetable;

        protected override void Reset()
        {
            isAFinished = false;
            a.Reset();
            b.Reset();
        }

        protected override YieldValue WillYield()
        {
            if (!isAFinished)
            {
                if (!a.Yield())
                {
                    // A is finished
                    isAFinished = true;
                }
            }

            if (isAFinished)
            {
                if (!b.Yield())
                {
                    // B is finished
                    return YieldValue.Finish(b.Value);
                }
            }

            return YieldValue.Continue();
        }
    }
}