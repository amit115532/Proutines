using System;

namespace Proutines.Generators
{
    public delegate double LerpManipulator(double t);

    public static class LerpManipulators
    {
        public static double Linear(double t)
        {
            return t;
        }

        public static double EaseIn(double t)
        {
            return 1f - Math.Cos(t * Math.PI * 0.5f);
        }

        public static double EaseOut(double t)
        {
            return Math.Sin(t * Math.PI * 0.5f);
        }

        public static double EaseInOut(double t)
        {
            return t * t * (3f - 2f * t);
        }

        public static double SmootherEaseInOut(double t)
        {
            return t * t * t * (t * (6f * t - 15f) + 10f);
        }
    }
}