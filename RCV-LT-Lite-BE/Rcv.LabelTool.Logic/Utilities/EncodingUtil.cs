using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

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
        private static Encoding FileEncoding = System.Text.Encoding.Unicode;

        /// <summary>
        /// Get StreamReader from file stream with encoded content.
        /// </summary>
        /// <param name="stream">Stream to read</param>
        /// <returns>StreamReader with content encoding</returns>
        public static System.IO.StreamReader GetStreamReader(System.IO.Stream stream, Encoding encoding = null)
        {
            #region validation

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            #endregion

            return new System.IO.StreamReader(stream, (encoding ?? FileEncoding));
        }

        /// <summary>
        /// Get encoded byte represenation of string content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string content, Encoding encoding = null)
        {
            #region validation

            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            #endregion

            return (encoding ?? FileEncoding).GetBytes(content);
        }
    }
}