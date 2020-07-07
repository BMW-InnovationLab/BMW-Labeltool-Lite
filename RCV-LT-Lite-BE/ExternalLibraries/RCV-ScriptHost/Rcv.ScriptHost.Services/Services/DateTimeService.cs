using Rcv.ScriptHost.Contracts.Services;
using System;

namespace Rcv.ScriptHost.Services.Services
{
    /// <summary>
    /// Implementation of DateTimeService.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        /// <summary>
        /// <see cref="IDateTimeService.GetNow"/>
        /// </summary>
        public DateTime GetNow()
        {
            // return current timestamp
            return DateTime.Now;
        }
    }
}
