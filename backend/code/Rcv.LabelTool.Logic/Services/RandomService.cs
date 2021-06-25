using Rcv.LabelTool.Contracts.Services;
using System;

namespace Rcv.LabelTool.Web.Services
{
    /// <summary>
    /// Implementation of IRandomService.
    /// Only for testing purposes.
    /// </summary>
    public class RandomService : IRandomService
    {
        #region member

        private Random Random { get; set; }

        #endregion

        #region constructor

        public RandomService()
        {
            this.Random = new Random();
        }

        #endregion

        #region IRandomService

        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        public double Next(double minValue, double maxValue)
        {
            return Random.NextDouble() * (maxValue - minValue) + minValue;
        }

        #endregion
    }
}