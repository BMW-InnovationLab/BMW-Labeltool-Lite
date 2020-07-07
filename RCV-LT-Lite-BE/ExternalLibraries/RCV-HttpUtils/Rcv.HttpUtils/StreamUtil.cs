using System.IO;

namespace Rcv.HttpUtils
{
    /// <summary>
    /// Util to work with streams.
    /// </summary>
    public static class StreamUtil
    {
        /// <summary>
        /// Convert stream to byte array respresentation.
        /// </summary>
        /// <param name="stream">Stream to convert to byte array representation.</param>
        /// <returns>Byte array represenation of stream</returns>
        public static byte[] ConvertToByteArray(this Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Rewind stream to first position of stream if it is seekable.
        /// </summary>
        /// <param name="stream">Stream to rewind</param>
        public static void Rewind(this Stream stream)
        {
            if (stream.CanSeek && stream.Position > 0)
            {
                stream.Seek(0L, SeekOrigin.Begin);
            }
        }
    }
}
