namespace RCV.FileContainer.Enumerations
{
    /// <summary>
    /// Enumeration to differ between files and directories meant to be used in the <see cref="Container.AzureFileContainer"/> and <see cref="Container.SystemFileContainer"/>
    /// </summary>
    public enum EFileType
    {
        /// <summary>
        /// Represents files
        /// </summary>
        Files,

        /// <summary>
        /// Represents directories
        /// </summary>
        Directories
    }
}