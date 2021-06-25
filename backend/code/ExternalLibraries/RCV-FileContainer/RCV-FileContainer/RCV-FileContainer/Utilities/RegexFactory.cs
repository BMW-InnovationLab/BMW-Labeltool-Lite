using System;
using System.Text.RegularExpressions;

namespace RCV.FileContainer.Utilities
{
    /// <summary>
    /// Factory for creation of regular expressions.
    /// </summary>
    internal static class RegexFactory
    {
        /// <summary>
        /// Create regular expression from search pattern which is used for
        /// define searches in filesystems.
        /// Allowes searches with wildcards '*' and '?' within strings.
        /// </summary>
        /// <param name="searchPattern">Search pattern for files</param>
        /// <returns>Regular expression for search pattern</returns>
        public static Regex CreateRegexFromSearchPattern(string searchPattern)
        {
            #region validation

            if (searchPattern == null)
            {
                throw new ArgumentNullException(nameof(searchPattern));
            }

            #endregion

            // replace wildcard for 'multiple letters' with regex-wildcard
            string regexSearchPattern = searchPattern.Replace("*", ".*");

            // replace wildcard for 'single letter' with regex-wildcard
            regexSearchPattern = regexSearchPattern.Replace("?", ".?");

            // set search pattern in 'begin-to-end'-symbols
            regexSearchPattern = $"^{regexSearchPattern}$";

            return new Regex(regexSearchPattern);
        }
    }
}
