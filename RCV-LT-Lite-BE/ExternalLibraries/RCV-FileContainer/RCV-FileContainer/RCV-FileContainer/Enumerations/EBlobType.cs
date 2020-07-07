namespace RCV.FileContainer.Enumerations
{
    /// <summary>
    /// Enumeration to differ between blobs and virtual directories meant to be used in the <see cref="Container.AzureBlobContainer"/>
    /// </summary>
    public enum EBlobType
    {
        /// <summary>
        /// Represents binary large objects
        /// </summary>
        Blobs,

        /// <summary>
        /// Represents virtual directories
        /// </summary>
        Directories
    }
}