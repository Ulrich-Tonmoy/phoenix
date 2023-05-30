using System;

namespace Phoenix.Editor.Utilities
{
    public static class MathUtilities
    {
        public static float Epsilon => 0.00001f;
        public static bool IsSameAs(this float value, float other)
        {
            return Math.Abs(value - other) < Epsilon;
        }
        public static bool IsSameAs(this float? value, float? other)
        {
            if (!value.HasValue || !other.HasValue) return false;
            return Math.Abs(value.Value - other.Value) < Epsilon;
        }
    }
}
