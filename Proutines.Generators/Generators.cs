using System;
using System.Collections.Generic;

namespace Proutines.Generators
{
    public class GeneratorExtenstions
    {
        // Left blank intentionally
    }

    public static class Generator
    {
        private static GeneratorExtenstions _instance;

        public static GeneratorExtenstions New
        {
            get
            {
                if (_instance == null)
                    _instance = new GeneratorExtenstions();

                return _instance;
            }
        }

        public static ILerpGenerator Lerp(this GeneratorExtenstions ex, float totalTime, ITimeProvider timeProvider = null)
        {
            return new LerpGenerator(totalTime, null, timeProvider);
        }

        public static ILerpGenerator Lerp(this GeneratorExtenstions ex, double totalTime, ITimeProvider timeProvider = null)
        {
            return new LerpGenerator(totalTime, null, timeProvider);
        }

        public static ILerpGenerator Lerp(this GeneratorExtenstions ex, float totalTime, LerpManipulator lerpManipulator, ITimeProvider timeProvider = null)
        {
            return new LerpGenerator(totalTime, lerpManipulator, timeProvider);
        }

        public static ILerpGenerator Lerp(this GeneratorExtenstions ex, double totalTime, LerpManipulator lerpManipulator, ITimeProvider timeProvider = null)
        {
            return new LerpGenerator(totalTime, lerpManipulator, timeProvider);
        }

        public static IYieldGenerator<T> Custom<T>(this GeneratorExtenstions ex, Func<T> f)
        {
            return new CustomGenerator<T>(f);
        }

        public static IYieldGenerator<T> Sequence<T>(this GeneratorExtenstions ex, IEnumerable<T> seq)
        {
            return new SequenceGenerator<T>(seq);
        }

        /// <summary>
        /// From zero to the given <see cref="length"/> - 1
        /// </summary>
        public static IYieldGenerator<int> Range(this GeneratorExtenstions ex, int length)
        {
            return new RangeGen(length);
        }

        /// <summary>
        /// From <see cref="from"/> to <see cref="length"/> - 1
        /// </summary>
        public static IYieldGenerator<int> Range(this GeneratorExtenstions ex, int from, int length)
        {
            return new RangeGen(from, length);
        }

        public static IYieldGenerator<int> Range(this GeneratorExtenstions ex)
        {
            return new RangeGen();
        }

        public static IYieldGenerator<int> Random(this GeneratorExtenstions ex, int min, int max)
        {
            return new RandomGen(min, max);
        }

        public static IYieldGenerator<double> Random(this GeneratorExtenstions ex)
        {
            return new RandomGenDouble();
        }

        public static IYieldGenerator<bool> RandomBool(this GeneratorExtenstions ex)
        {
            return new RandomGen(0, 2).Select(s => s == 0);
        }

        public static IYieldGenerator<double> Random(this GeneratorExtenstions ex, int seed)
        {
            return new RandomGenDouble(seed);
        }

        public static IYieldGenerator<double> Random(this GeneratorExtenstions ex, double min, double max)
        {
            return new RandomGenDouble(min, max);
        }

        public static IYieldGenerator<double> Random(this GeneratorExtenstions ex, double min, double max, int seed)
        {
            return new RandomGenDouble(min, max, seed);
        }

        public static IYieldGenerator<float> RandomF(this GeneratorExtenstions ex)
        {
            return new RandomGenDouble().Select((s) => (float)s);
        }

        public static IYieldGenerator<float> RandomF(this GeneratorExtenstions ex, int seed)
        {
            return new RandomGenDouble(seed).Select((s) => (float)s);
        }

        public static IYieldGenerator<float> Random(this GeneratorExtenstions ex, float min, float max)
        {
            return new RandomGenDouble(min, max).Select((s) => (float)s);
        }

        public static IYieldGenerator<float> Random(this GeneratorExtenstions ex, float min, float max, int seed)
        {
            return new RandomGenDouble(min, max, seed).Select((s) => (float)s);
        }

        /// <summary>
        /// generator for current time
        /// </summary>
        public static IYieldGenerator<double> Time(this GeneratorExtenstions ex, ITimeProvider timeProvider = null)
        {
            return new TimeGen(false, timeProvider ?? TimeProviders.DefaultTimeProvider);
        }

        /// <summary>
        /// generator for current time relative to the start of the execution
        /// </summary>
        public static IYieldGenerator<double> TimeFromNow(this GeneratorExtenstions ex, ITimeProvider timeProvider = null)
        {
            return new TimeGen(true, timeProvider ?? TimeProviders.DefaultTimeProvider);
        }

        /// <summary>
        /// generator for current time
        /// </summary>
        public static IYieldGenerator<double> Time(this GeneratorExtenstions ex, double amountOfSeconds, ITimeProvider timeProvider = null)
        {
            return new TimeGen(amountOfSeconds, false, timeProvider ?? TimeProviders.DefaultTimeProvider);
        }

        /// <summary>
        /// generator for current time relative to the start of the execution
        /// </summary>
        public static IYieldGenerator<double> TimeFromNow(this GeneratorExtenstions ex, double amountOfSeconds, ITimeProvider timeProvider = null)
        {
            return new TimeGen(amountOfSeconds, true, timeProvider ?? TimeProviders.DefaultTimeProvider);
        }

        /// <summary>
        /// generator for current time
        /// </summary>
        public static IYieldGenerator<float> TimeF(this GeneratorExtenstions ex, ITimeProvider timeProvider = null)
        {
            return new TimeGen(false, timeProvider ?? TimeProviders.DefaultTimeProvider).Select(t => (float)t);
        }

        /// <summary>
        /// generator for current time relative to the start of the execution
        /// </summary>
        public static IYieldGenerator<float> TimeFromNowF(this GeneratorExtenstions ex, ITimeProvider timeProvider = null)
        {
            return new TimeGen(true, timeProvider ?? TimeProviders.DefaultTimeProvider).Select(t => (float)t);
        }

        /// <summary>
        /// generator for current time
        /// </summary>
        public static IYieldGenerator<float> Time(this GeneratorExtenstions ex, float amountOfSeconds, ITimeProvider timeProvider = null)
        {
            return new TimeGen(amountOfSeconds, false, timeProvider ?? TimeProviders.DefaultTimeProvider).Select(t => (float)t);
        }

        /// <summary>
        /// generator for current time relative to the start of the execution
        /// </summary>
        public static IYieldGenerator<float> TimeFromNow(this GeneratorExtenstions ex, float amountOfSeconds, ITimeProvider timeProvider = null)
        {
            return new TimeGen(amountOfSeconds, true, timeProvider ?? TimeProviders.DefaultTimeProvider).Select(t => (float)t);
        }
    }
}