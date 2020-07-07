using System;

namespace Rcv.ScriptHost.Contracts.Services
{
    /// <summary>
    /// Interface definition of Date Time Service.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Returns current date-time-stamp.
        /// </summary>
        /// <returns>Date-Time-Stamp of current datetime</returns>
        DateTime GetNow();
    }
}
