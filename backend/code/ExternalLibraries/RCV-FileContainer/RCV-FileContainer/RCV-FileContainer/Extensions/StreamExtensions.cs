namespace RCV.FileContainer.Extensions
{
    /// <summary>
    /// Extension for streams.
    /// </summary>
    public static class StreamExtensions
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
    }
}