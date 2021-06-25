using System;
using System.Text;

namespace Rcv.LabelTool.Web.Utilities
{
    /// <summary>
    /// Util for encoding of files.
    /// </summary>
    public static class EncodingUtil
    {
        /// <summary>
        /// Standard encoding of application.
        /// All text based files are stored by using this encoding.
        /// </summary>
        private static Encoding FileEncoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// Get StreamReader from file stream with encoded content.
        /// </summary>
        /// <param name="stream">Stream to read</param>
        /// <returns>StreamReader with content encoding</returns>
        public static System.IO.StreamReader GetStreamReader(System.IO.Stream stream)
        {
            #region validation

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            #endregion

            return new System.IO.StreamReader(stream, FileEncoding, true);
        }

        /// <summary>
        /// Get encoded byte represenation of string content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string content)
        {
            #region validation

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            #endregion

            return FileEncoding.GetBytes(content);
        }
    }
}