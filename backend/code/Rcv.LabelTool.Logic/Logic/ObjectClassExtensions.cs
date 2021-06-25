using Rcv.LabelTool.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Logic.Logic
{
    /// <summary>
    /// Extension methods for objectclasses.
    /// </summary>
    public static class ObjectClassExtensions
    {
        /// <summary>
        /// Find objectclasses by name or name equivalent.
        /// </summary>
        /// <param name="objectClasses">List of objectclasses</param>
        /// <param name="name">Name of objectclass to find</param>
        /// <returns>Most eqvivalent objectclass</returns>
        public static ObjectClass FindObjectClassByName(this IEnumerable<ObjectClass> objectClasses, string name)
        {
            #region validation

            if (objectClasses == null)
            {
                throw new ArgumentNullException(nameof(objectClasses));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            #endregion

            // find objectclass with matching name
            ObjectClass result = objectClasses.FirstOrDefault(o => o.Name == name);
            if (result == null)
            {
                // find objectclass with name with ignore case
                result = objectClasses.FirstOrDefault(o => o.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            }
            if (result == null)
            {
                // find objectclass with name with ignore case and only with letters and digits
                string nameCleaned = name.GetCleanedString();
                result = objectClasses.FirstOrDefault(o => o.Name.GetCleanedString().Equals(nameCleaned, StringComparison.CurrentCultureIgnoreCase));
            }
            return result;
        }

        /// <summary>
        /// Get objectclass with given ID and Name. If no objectclass found for
        /// this parameters NULL will be returned.
        /// </summary>
        /// <param name="objectClasses">List of objectclasses</param>
        /// <param name="objectClassId">ID of objectclass to search for</param>
        /// <param name="objectClassName">Name of objectclass to search for (case sensitive)</param>
        /// <returns>Objectclass or NULL</returns>
        public static ObjectClass GetObjectClass(this IEnumerable<ObjectClass> objectClasses, long objectClassId, string objectClassName)
        {
            #region validation

            if (objectClasses == null)
            {
                throw new ArgumentNullException(nameof(objectClasses));
            }

            #endregion

            return objectClasses.FirstOrDefault(o => o.Name == objectClassName && o.Id == objectClassId);
        }

        /// <summary>
        /// Get objectclass with given name. If not objectclass found with this 
        /// name NULL will be returned.
        /// </summary>
        /// <param name="objectClasses">List of objectclasses</param>
        /// <param name="objectClassName">Name of objectclass to find (case sensitive)</param>
        /// <returns>Objectclass or NULL</returns>
        public static ObjectClass GetObjectClass(this IEnumerable<ObjectClass> objectClasses, string objectClassName)
        {
            #region validation

            if (objectClasses == null)
            {
                throw new ArgumentNullException(nameof(objectClasses));
            }

            #endregion

            return objectClasses.FirstOrDefault(o => o.Name == objectClassName);
        }

        /// <summary>
        /// Get objectclass with given Id. If no objectclass can be found with
        /// this id, NULL will be returned.
        /// </summary>
        /// <param name="objectClasses">List of objectclasses</param>
        /// <param name="objectClassId">Id of objectclass to find</param>
        /// <returns>Objectclass or NULL</returns>
        public static ObjectClass GetObjectClass(this IEnumerable<ObjectClass> objectClasses, long objectClassId)
        {
            #region validation

            if (objectClasses == null)
            {
                throw new ArgumentNullException(nameof(objectClasses));
            }

            #endregion

            return objectClasses.FirstOrDefault(o => o.Id == objectClassId);
        }

        #region private helper

        /// <summary>
        /// Cleanup string. Returns a string with only letters and digits.
        /// </summary>
        /// <param name="str">String to cleanup</param>
        /// <returns>String with only letters and digits</returns>
        private static string GetCleanedString(this string str)
        {
            string result = string.Empty;
            foreach (char c in str)
            {
                if (char.IsLetterOrDigit(c))
                {
                    result += c;
                }
            }

            return result;
        }

        #endregion
    }
}
