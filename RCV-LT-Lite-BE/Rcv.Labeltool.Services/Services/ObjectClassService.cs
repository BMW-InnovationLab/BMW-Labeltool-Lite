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

        public bool ExistsObjectClass(Topic topic, uint objectClassId)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            return ObjectClassRepository.ExistsObjectClass(topic, objectClassId);
        }

        public ObjectClass GetObjectClass(Topic topic, uint objectClassId)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            return ObjectClassRepository.GetObjectClass(topic, objectClassId);
        }

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

        #endregion
    }
}