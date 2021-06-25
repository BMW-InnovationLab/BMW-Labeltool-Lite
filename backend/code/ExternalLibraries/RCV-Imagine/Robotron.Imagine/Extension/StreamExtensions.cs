namespace Robotron.Imagine.Extension
{
    /// <summary>
    /// Extension for streams.
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// Rewind stream to first position.
        /// </summary>
        /// <param name="stream">Stream to rewind</param>
        public static void Rewind(this System.IO.Stream stream)
        {
            if (stream.CanSeek && stream.Position != 0)
            {
                stream.Seek(0L, System.IO.SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Copy stream as new stream.
        /// </summary>
        /// <param name="stream">Stream to copy</param>
        /// <returns>Copy of original stream</returns>
        public static System.IO.Stream CopyStream(this System.IO.Stream stream)
        {
            System.IO.Stream targetStream = new System.IO.MemoryStream();
            stream.Rewind();
            stream.CopyTo(targetStream);
            targetStream.Rewind();
            return targetStream;
        }
    }
}
