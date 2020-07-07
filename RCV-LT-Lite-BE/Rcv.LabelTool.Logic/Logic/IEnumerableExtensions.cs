using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Web.Logic
{
    /// <summary>
    /// Extension for IEnumerable operations.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Get next integral number of list. Try to calculate max + 1.
        /// If no element is given in list, 0 will be returned.
        /// This method can be used to generate ids.
        /// </summary>
        /// <typeparam name="T">Type of list elements</typeparam>
        /// <param name="list">List where to get next id from</param>
        /// <param name="selector">Selector to get ids</param>
        /// <returns>Max + 1 or 0 if list is empty</returns>
        public static int GetNext<T>(this IEnumerable<T> list, Func<T, int> selector)
        {
            if (list.Count() > 0)
            {
                return list.Max(selector) + 1;
            }
            return 0;
        }
    }
}