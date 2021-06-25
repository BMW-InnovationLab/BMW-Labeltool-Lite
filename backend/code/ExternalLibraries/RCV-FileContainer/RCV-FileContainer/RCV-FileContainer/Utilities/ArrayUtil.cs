using System;

namespace RCV.FileContainer.Utilities
{
    /// <summary>
    /// Util for working with arrays.
    /// </summary>
    internal static class ArrayUtil
    {
        /// <summary>
        /// Checks if two arrays contains the same content.
        /// </summary>
        /// <param name="arr1">Array 1 with string content</param>
        /// <param name="arr2">Array 2 with string content</param>
        /// <param name="comparisonType">Comparison type of strings</param>
        /// <returns>TRUE if content of arrays are equal, otherwise FALSE</returns>
        internal static bool IsEqualTo(this string[] arr1, string[] arr2, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            // both arrays are null, so they are equal
            if (arr1 == null && arr2 == null)
            {
                return true;
            }

            // one of the arrays is null, the other not, so they are inequal
            if ((arr1 != null && arr2 == null) || (arr1 == null && arr2 != null))
            {
                return false;
            }

            if (arr1.Length == arr2.Length)
            {
                // number of entries of both arrays are equal, so check 
                // entries step by step
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (!arr1[i].Equals(arr2[i], comparisonType))
                    {
                        // item at position i is not equal in array 1 and array 2
                        return false;
                    }
                }

                // all items are equal
                return true;
            }

            // number of items of both arrays does not match
            return false;
        }
    }
}
