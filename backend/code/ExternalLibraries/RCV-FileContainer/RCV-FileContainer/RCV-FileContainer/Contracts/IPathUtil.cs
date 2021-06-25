namespace RCV.FileContainer.Contracts
{
    /// <summary>
    /// Util for Path determination.
    /// </summary>
    public interface IPathUtil
    {
        /// <see cref="System.IO.Path.AltDirectorySeparatorChar"/>
        char AltDirectorySeparatorChar { get; }

        /// <see cref="System.IO.Path.DirectorySeparatorChar"/>
        char DirectorySeparatorChar { get; }

        /// <see cref="System.IO.Path.Combine(string[])"/>
        string Combine(params string[] paths);
    }
}
