using Rcv.ScriptHost.Contracts.Services;
using System;

namespace Rcv.ScriptHost.Services.Services
{
    /// <summary>
    /// Service implementation of GuidService.
    /// </summary>
    public class GuidService : IGuidService
    {
        /// <summary>
        /// <see cref="IGuidService.GenerateGuid"/>
        /// </summary>
        public string GenerateGuid()
        {
            // genereate guid and return as string
            return Guid.NewGuid().ToString();
        }
    }
}
