using System;

namespace Assets.Reusable.Utils
{
    public static class NullableExtensions
    {
        public static B? Select<A,B>(this A? obj, Func<A, B> func) where A:struct where B : struct
        {
            if (obj.HasValue) return func(obj.Value);
            return null;
        }
    }
}