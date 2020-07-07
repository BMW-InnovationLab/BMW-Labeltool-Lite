namespace Rcv.ScriptHost.Contracts.Container
{
    /// <summary>
    /// File container interface for work with files 
    /// in filesystem or someone else.
    /// </summary>
    public interface IFileContainer
    {
        /// <summary>
        /// Create given path in container.
        /// </summary>
        /// <param name="path">Path to create</param>
        /// <returns>Created path as string</returns>
        string CreatePath(params string[] path);

        /// <summary>
        /// Deletes all contents from given path. 
        /// Path will cleaned up. Directory of path
        /// will resist.
        /// </summary>
        /// <param name="path">Path to cleanup</param>
        void CleanUp(params string[] path);
    }
}
