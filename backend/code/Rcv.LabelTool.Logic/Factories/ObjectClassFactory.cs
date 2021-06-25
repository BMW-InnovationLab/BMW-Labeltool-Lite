using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Models.View;
using System;

namespace Rcv.LabelTool.Logic.Factories
{
    /// <summary>
    /// Factory for creation of objectclass-representation objects.
    /// </summary>
    public static class ObjectClassFactory
    {
        /// <summary>
        /// Create objectclass view object from objectclass.
        /// </summary>
        /// <param name="objectClass">Source objectclass object</param>
        /// <returns>View representation of objectclass</returns>
        public static ObjectClassView CreateView(ObjectClass objectClass)
        {
            #region validation

            if (objectClass == null)
            {
                throw new ArgumentNullException(nameof(objectClass));
            }

            #endregion

            return new ObjectClassView()
            {
                Id = objectClass.Id,
                Sort = objectClass.Sort,
                Name = objectClass.Name,
                ColorCode = objectClass.ColorCode,
                ParentObjectClassId = objectClass.ParentObjectClassId
            };
        }
    }
}
