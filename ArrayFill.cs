using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filler
{
    public static class Extensions
    {
        public static void Fill<T>(this T[] src, T value)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src[i] = value;
            }
        }
        public static void Fill<T>(this List<T> src, T value)
        {
            for (int i = 0; i < src.Count; i++)
            {
                src[i] = value;
            }
        }
    }
}