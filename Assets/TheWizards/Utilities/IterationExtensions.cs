using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.DualContouring
{
    public static class IterationExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> act)
        {
            foreach (var f  in list)act(f);
        }
    }
}