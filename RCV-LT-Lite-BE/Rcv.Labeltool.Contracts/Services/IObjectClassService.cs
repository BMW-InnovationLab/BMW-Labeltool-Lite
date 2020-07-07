using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Interface definition for objectclass services.
    /// </summary>
    public interface IObjectClassService
    {
        /// <summary>
        /// Checks if specified objectclassId exists for given topic.
        /// </summary>
        /// <param name="topic">Topic of objectclasses</param>
        /// <param name="objectClassId">Id of objectclass</param>
        /// <returns>TRUE if objectclass with id exists, otherwise FALSE</returns>
        bool ExistsObjectClass(Topic topic, uint objectClassId);

        /// <summary>
        /// Get objectclass with given ID.
        /// </summary>
        /// <param name="topic">Topic of objectclass</param>
        /// <param name="objectClassId">Id of objectclass</param>
        /// <returns>Objectclass with given id</returns>
        ObjectClass GetObjectClass(Topic topic, uint objectClassId);

        /// <summary>
        /// Gets a list of all objectclasses specified in this topic.
        /// </summary>
        /// <param name="topic">Topic of objectclass</param>
        /// <returns>Objectclasses from given topic</returns>
        IEnumerable<ObjectClass> GetObjectClasses(Topic topic);
    }
}
