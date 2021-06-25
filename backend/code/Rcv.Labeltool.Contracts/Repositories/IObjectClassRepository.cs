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
        /// Returns List of Objectclasses specific for the given topic.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <returns>List of objectclasses for this topic</returns>
        IEnumerable<ObjectClass> GetObjectClasses(Topic topic);

        /// <summary>
        /// Add new objectclass to classstore. ID will be generated in this method.
        /// </summary>
        /// <param name="topic">the topic</param>
        /// <param name="objectClass">Objectclass to add</param>
        /// <returns>Newly created objectclass</returns>
        ObjectClass AddObjectClass(Topic topic, ObjectClass objectClass);
    }
}
