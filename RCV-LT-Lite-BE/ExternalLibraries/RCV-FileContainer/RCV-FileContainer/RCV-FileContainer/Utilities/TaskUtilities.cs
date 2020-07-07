using System;
using System.Threading.Tasks;

namespace RCV.FileContainer.Utilities
{
    /// <summary>
    /// Static utility class to execute asynchronous methods synchronously.
    /// </summary>
    public static class TaskUtilities
    {
        /// <summary>
        /// Generic method to execute an asynchronously method synchronously and to access the return value.
        /// </summary>
        /// <typeparam name="T">Generic method type.</typeparam>
        /// <param name="asyncTask">Asynchronous generic task to execute synchronously</param>
        /// <returns>The generic result T.</returns>
        public static T ExecuteSync<T>(Task<T> asyncTask)
        {
            #region validation

            if (asyncTask == null)
            {
                throw new ArgumentNullException(nameof(asyncTask));
            }

            #endregion

            Task task = Task.Run(async () => { await asyncTask; });

            task.Wait();

            return asyncTask.Result;
        }

        /// <summary>
        /// Void method to execute an asynchronously method synchronously.
        /// </summary>
        /// <param name="asyncTask">Asynchronous void task to execute synchronously</param>
        public static void ExecuteSync(Task asyncTask)
        {
            #region validation

            if (asyncTask == null)
            {
                throw new ArgumentNullException(nameof(asyncTask));
            }

            #endregion

            Task task = Task.Run(async () => { await asyncTask; });

            task.Wait();
        }
    }
}
