using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using System;
using System.Collections.Generic;

namespace Rcv.LabelTool.Web.Services
{
    public class ObjectClassService : IObjectClassService
    {
        #region member

        private IObjectClassRepository ObjectClassRepository { get; }

        #endregion

        #region constructor

        public ObjectClassService(IObjectClassRepository objectClassRepository)
        {
            ObjectClassRepository = objectClassRepository ?? throw new ArgumentNullException(nameof(objectClassRepository));
        }

        #endregion

        #region IObjectClassService

        public IEnumerable<ObjectClass> GetObjectClasses(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            return ObjectClassRepository.GetObjectClasses(topic);
        }

        public ObjectClass CreateObjectClass(Topic topic, ObjectClass objectClass)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (objectClass == null)
            {
                throw new ArgumentNullException(nameof(objectClass));
            }

            #endregion

            // create class in repository
            return ObjectClassRepository.AddObjectClass(topic, objectClass);
        }

        #endregion
    }
}