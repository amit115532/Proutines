using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Proutines
{
    /// <summary>
    /// This is a base classs for executing coroutines. It is thread safe
    /// </summary>
    public class ThreadSafeCoroutinesExecutor : IExternalCoroutinesExecutor
    {
        protected readonly object Locker = new object();
        private readonly IList<ICoroutineLogic> coroutinesLogic = new List<ICoroutineLogic>();
        private readonly Queue<ICoroutineLogic> nextCoroutinesLogic = new Queue<ICoroutineLogic>();
        private readonly Queue<ICoroutineLogic> removedCoroutines = new Queue<ICoroutineLogic>();
        private int shouldRemoveAllCoroutines;
        private int shouldAddNewCoroutines;

        internal ICoroutineLogic CurrentCoroutine { get; private set; }
        internal int CurrentCoroutineIndex { get; private set; }

        public virtual void Dispose()
        {
            StopAllCoroutines();
            Update(); // Must call update so it will actually remove all coroutines
        }

        public void Update()
        {
            if (ShouldRemoveAllCoroutines())
            {
                DisposeCoroutines();
                return;
            }

            if (ShouldAddNewCoroutines())
            {
                AddCoroutines();
            }

            UpdateCoroutines(0);

            RemoveCoroutines();
        }

        public void UpdateCoroutines(int fromIndex)
        {
            for (var i = fromIndex; i < coroutinesLogic.Count; ++i)
            {
                CurrentCoroutineIndex = i;
                CurrentCoroutine = coroutinesLogic[i];

                var shouldBeRemoved = !(coroutinesLogic[i].ExecuteCurrentInstruction());

                CurrentCoroutineIndex = -1;
                CurrentCoroutine = null;

                if (shouldBeRemoved)
                {
                    removedCoroutines.Enqueue(coroutinesLogic[i]);
                }
            }
        }

        private bool ShouldRemoveAllCoroutines()
        {
            return Interlocked.Exchange(ref shouldRemoveAllCoroutines, 0) != 0;
        }

        private bool ShouldAddNewCoroutines()
        {
            return Interlocked.Exchange(ref shouldAddNewCoroutines, 0) != 0;
        }

        protected void RemoveCoroutines()
        {
            while (removedCoroutines.Count != 0)
            {
                coroutinesLogic.Remove(removedCoroutines.Dequeue());
            }
        }

        private void AddCoroutines()
        {
            lock (Locker)
            {
                while (nextCoroutinesLogic.Count != 0)
                {
                    var nextCoroutine = nextCoroutinesLogic.Dequeue();
                    coroutinesLogic.Add(nextCoroutine);
                }
            }
        }

        public ICoroutine StartCoroutine(IEnumerator<IYieldInstruction> coroutineEnumerator)
        {
            lock (Locker)
            {
                var coroutineImplementation = new CoroutineLogic(coroutineEnumerator);
                nextCoroutinesLogic.Enqueue(coroutineImplementation);

                Interlocked.Increment(ref shouldAddNewCoroutines);

                return coroutineImplementation.GetExternalCoroutine();
            }
        }

        public void StopAllCoroutines()
        {
            Interlocked.Increment(ref shouldRemoveAllCoroutines);
            Interlocked.Exchange(ref shouldAddNewCoroutines, 0);

            lock (Locker)
            {
                foreach (var coroutine in nextCoroutinesLogic.ToList())
                    coroutine.Dispose();

                nextCoroutinesLogic.Clear();
            }
        }

        protected void DisposeCoroutines()
        {
            foreach (var coroutine in coroutinesLogic.ToList())
                coroutine.Dispose();

            coroutinesLogic.Clear();
        }

        public int Count => coroutinesLogic.Count;
    }
}