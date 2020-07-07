namespace RCV.Babel.Util
{
    /// <summary>
    /// Language util of RCV.
    /// </summary>
    public static class LanguageUtil
    {
        /// <summary>
        /// Get culture specific key from accept-language header.
        /// </summary>
        /// <param name="acceptLanguageHeader">Accept language header content</param>
        /// <returns>Culture key with pattern xx-XX</returns>
        public static string GetCultureFromAcceptLanguage(string acceptLanguageHeader)
        {
            if (string.IsNullOrEmpty(acceptLanguageHeader))
            {
                return Instance.DefaultCulture;
            }

            string culture = acceptLanguageHeader;
            if (culture.Contains(';'))
            {
                culture = acceptLanguageHeader.Substring(0, acceptLanguageHeader.IndexOf(';'));
            }
            if (culture.Contains(','))
            {
                culture = acceptLanguageHeader.Substring(0, acceptLanguageHeader.IndexOf(','));
            }

            return culture;
        }
    }
}
