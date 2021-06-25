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
        /// Gets a list of all objectclasses specified in this topic.
        /// </summary>
        /// <param name="topic">Topic of objectclass</param>
        /// <returns>Objectclasses from given topic</returns>
        IEnumerable<ObjectClass> GetObjectClasses(Topic topic);

        /// <summary>
        /// Creates a new objectclass with given information.
        /// </summary>
        /// <param name="topic">Topic of objectclass</param>
        /// <param name="objectClass">Objectclass to create</param>
        ObjectClass CreateObjectClass(Topic topic, ObjectClass objectClass);
    }
}
