using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyWPFUI.Extensions
{
    public static class ListExtensions
    {
        public static void Resize<T>(this List<T> list, int size, T element = default)
        {
            int currentSize = list.Count;
            if (size < currentSize)
            {
                list.RemoveRange(size, currentSize - size);
            }
            else if (size > currentSize)
            {
                if (size > list.Capacity) //this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                    list.Capacity = size;
                list.AddRange(Enumerable.Repeat(element, size - currentSize));
            }
        }

        public static void Resize<T>(this List<T> list, int sz) where T : new()
        {
            Resize(list, sz, new T());
        }

        public static bool IsEmpty(this IList list)
        {
            return list.Count == 0;
        }
    }
}
