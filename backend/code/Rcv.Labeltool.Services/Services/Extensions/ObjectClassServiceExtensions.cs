using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Web.Services.Extensions
{
    /// <summary>
    /// Extension for objectclass service.
    /// </summary>
    public static class ObjectClassServiceExtensions
    {
        /// <summary>
        /// Create objectclass with given name if currently not exists in collection of object classes.
        /// Returns newly generated object class or objectclass with given name.
        /// </summary>
        /// <param name="objectClassService">Objectclass service</param>
        /// <param name="topic">Topic for object classes</param>
        /// <param name="objectClassName">Name of object class</param>
        /// <returns>Newly created object class</returns>
        public static ObjectClass CreateIfNotExists(this IObjectClassService objectClassService, Topic topic, string objectClassName)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(objectClassName))
            {
                throw new ArgumentNullException(nameof(objectClassName));
            }

            #endregion

            objectClassName = objectClassName.Replace('\\', '_').Replace('/', '_').Replace(':', '_').Replace('*', '_')
                .Replace('?', '_').Replace('^', '_').Replace('<', '_').Replace('<', '_').Replace('|', '_');

            // get list of all object classes
            IEnumerable<ObjectClass> objectClasses = objectClassService.GetObjectClasses(topic);

            // find object class with name
            ObjectClass objectClass = objectClasses.SingleOrDefault(o => o.Name.Equals(objectClassName, StringComparison.CurrentCultureIgnoreCase));

            if (objectClass == null)
            {
                // create object class
                objectClass = new ObjectClass()
                {
                    Name = objectClassName,
                    ColorCode = ObjectClassColorUtil.GetDefaultColorCode((uint)objectClasses.Count())
                };

                objectClass = objectClassService.CreateObjectClass(topic, objectClass);
            }
            return objectClass;
        }
    }
}