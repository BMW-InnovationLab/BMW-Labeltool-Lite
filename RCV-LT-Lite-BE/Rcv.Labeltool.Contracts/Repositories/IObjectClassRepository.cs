using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Repositories
{
    /// <summary>
    /// Interface defintion for ObjectClassRepository.
    /// </summary>
    public interface IObjectClassRepository
    {
        /// <summary>
        /// Checks if objectclass with given id exists in topic.
        /// </summary>
        /// <param name="topic">Topic where objectclass is stored</param>
        /// <param name="objectClassId">Id of objectclass to check for</param>
        /// <returns>TRUE if objectclass exists, otherwise FALSE</returns>
        bool ExistsObjectClass(Topic topic, uint objectClassId);

        /// <summary>
        /// Get objectclass of topic with specified Id.
        /// </summary>
        /// <param name="topic">Topic where objectclass is stored</param>
        /// <param name="objectClassId">Id of objectclass to get</param>
        /// <returns>Objectclass with specified id</returns>
        ObjectClass GetObjectClass(Topic topic, uint objectClassId);

        /// <summary>
        /// Returns List of Objectclasses specific for the given topic.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <returns>List of objectclasses for this topic</returns>
        IEnumerable<ObjectClass> GetObjectClasses(Topic topic);
    }
}
