using System;

namespace ios
{
    public static class Extensions
    {
        public static T apply<T>(this T obj, Action<T> func) {
            func.Invoke(obj);
            return obj;
        }
    }
}

