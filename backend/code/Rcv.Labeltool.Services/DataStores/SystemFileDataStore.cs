using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Repositories;
using RCV.FileContainer.Container;
using Serilog;

namespace Rcv.LabelTool.Services.DataStores
{
    public class SystemFileDataStore : SystemFileContainer, IDataStore
    {
        #region members

        public EDataStoreType DataStoreType { get; }

        #endregion

        #region constructor

        public SystemFileDataStore(EDataStoreType dataStoreType, string rootPath) : base(rootPath)
        {
            Log.Information($"Create SystemFileDataStore of type {dataStoreType}");
            DataStoreType = dataStoreType;
        }

        #endregion

    }
}
