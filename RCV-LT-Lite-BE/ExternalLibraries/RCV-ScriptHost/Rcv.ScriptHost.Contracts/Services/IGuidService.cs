namespace Rcv.ScriptHost.Contracts.Services
{
    /// <summary>
    /// Interface definition for GUID-Service.
    /// </summary>
    public interface IGuidService
    {
        /// <summary>
        /// Generates GUID and returns it as string.
        /// </summary>
        /// <returns>String representation of new GUID.</returns>
        string GenerateGuid();
    }
}
