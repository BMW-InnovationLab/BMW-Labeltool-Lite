using System;

namespace Rcv.LabelTool.Contracts.Exceptions
{
    /// <summary>
    /// Exception class should be used for errors in label formats.
    /// </summary>
    public class LabelFormatExceptions : Exception
    {
        public LabelFormatExceptions(string message) :
            base(message)
        {
        }
    }
}
