using System;

namespace Rcv.HttpUtils
{
    /// <summary>
    /// Util for creation of URI's.
    /// </summary>
    public static class UriUtil
    {
        /// <summary>
        /// Gets URI combined from baseAdress with given parts.
        /// Parts can include slashes as many as foo. Empty parts will be 
        /// ignored. Multiple slashes will be replaced to only one slash.
        /// </summary>
        /// <param name="baseAdress">Base adress of URI</param>
        /// <param name="parts">relative path of URI in parts</param>
        /// <returns>Full URI</returns>
        public static Uri GetUri(string baseAdress, params string[] parts)
        {
            #region validation

            if (string.IsNullOrEmpty(baseAdress))
            {
                throw new ArgumentNullException(nameof(baseAdress));
            }

            #endregion

            if (!baseAdress.EndsWith('/'))
            {
                baseAdress += '/';
            }

            Uri baseUri = new Uri(baseAdress);

            return GetUri(baseUri, parts);
        }

        /// <summary>
        /// Gets URI combined from baseAdress with given parts.
        /// Parts can include slashes as many as foo. Empty parts will be 
        /// ignored. Multiple slashes will be replaced to only one slash.
        /// </summary>
        /// <param name="baseUri">Base adress of URI</param>
        /// <param name="parts">relative path of URI in parts</param>
        /// <returns>Full URI</returns>
        public static Uri GetUri(Uri baseUri, params string[] parts)
        {
            #region validation

            if (baseUri == null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }

            #endregion

            string relativeUri = string.Empty;

            for (int i = 0; i < parts.Length; i++)
            {
                // do not prepend slash if
                //      - is first
                //      - part is empty
                //      - part is param (starts with ?)
                //      - part is next param (starts with ;)
                if (i > 0 && PrependSlash(parts[i]))
                {
                    relativeUri += '/';
                }

                relativeUri += parts[i];
            }

            while (relativeUri.Contains("//"))
            {
                relativeUri = relativeUri.Replace("//", "/");
            }

            if (relativeUri.StartsWith('/'))
            {
                relativeUri = relativeUri.Substring(1);
            }

            string baseAbsoluteUri = baseUri.AbsoluteUri;
            if (!baseAbsoluteUri.EndsWith('/') && PrependSlash(relativeUri))
            {
                baseAbsoluteUri += '/';
            }
            
            return new Uri(new Uri(baseAbsoluteUri), relativeUri);
        }

        #region private methods

        /// <summary>
        /// Checks if part of uri is empty or parameter. Returns true if not, so a slash can be prepended.
        /// </summary>
        /// <param name="part">Part of uri to check</param>
        /// <returns>TRUE if slash should be prepended, otherwise FALSE</returns>
        private static bool PrependSlash(string part)
        {
            return !string.IsNullOrEmpty(part) && !part.StartsWith('?') && !part.StartsWith(';');
        }

        #endregion
    }
}
