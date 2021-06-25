using Rcv.LabelTool.Contracts.Enumerations;
using RCV.FileContainer.Contracts;

namespace Rcv.LabelTool.Contracts.Repositories
{
    /// <summary>
    /// Interface definition of datastore.
    /// </summary>
    public interface IDataStore : IFileContainer
    {
        /// <summary>
        /// Type of datastore.
        /// </summary>
        EDataStoreType DataStoreType { get; }
    }
}
