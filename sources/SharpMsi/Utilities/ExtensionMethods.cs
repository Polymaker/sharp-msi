using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ExtensionMethods
    {
        public static int IndexOf<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].Equals(item))
                    return i;
            return -1;
        }
    }
}
