using RCV.FileContainer.Contracts;

namespace RCV.FileContainer.Utilities
{
    /// <summary>
    /// PathUtil to determine path information from underlying file system.
    /// </summary>
    public class PathUtil : IPathUtil
    {
        /// <inheritdoc />
        public char AltDirectorySeparatorChar
        {
            get { return System.IO.Path.AltDirectorySeparatorChar; }
        }

        /// <inheritdoc />
        public char DirectorySeparatorChar
        {
            get { return System.IO.Path.DirectorySeparatorChar; }
        }

        /// <inheritdoc />
        public string Combine(params string[] paths)
        {
            return System.IO.Path.Combine(paths);
        }
    }
}
