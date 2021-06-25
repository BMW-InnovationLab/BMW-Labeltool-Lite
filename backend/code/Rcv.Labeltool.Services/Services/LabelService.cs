using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Logic.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Web.Services
{
    public class LabelService : ILabelService
    {
        #region member

        private ILabelRepository LabelRepository { get; }

        private IObjectClassRepository ObjectClassRepository { get; }

        #endregion

        #region constructor

        public LabelService(ILabelRepository labelRepository, IObjectClassRepository objectClassRepository)
        {
            LabelRepository = labelRepository ?? throw new ArgumentNullException(nameof(labelRepository));
            ObjectClassRepository = objectClassRepository ?? throw new ArgumentNullException(nameof(objectClassRepository));
        }

        #endregion

        #region ILabelService

        public IEnumerable<Label> GetLabels(Topic topic, ImageLabel imageLabel)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (imageLabel == null)
            {
                throw new ArgumentNullException(nameof(imageLabel));
            }

            #endregion

            IEnumerable<Label> labels = LabelRepository.GetLabels(topic, imageLabel).ToList();

            if (labels.Any(o => string.IsNullOrEmpty(o.ObjectClassName)))
            {
                // load objectclasses only when labels includes where name of objectclass is not set
                List<ObjectClass> objectClasses = ObjectClassRepository.GetObjectClasses(topic).ToList();
                foreach (Label label in labels)
                {
                    label.ObjectClassName = objectClasses.GetObjectClass(label.ObjectClassId).Name;
                }
            }

            return labels;
        }
        public IEnumerable<Label> AddLabels(Topic topic, ImageLabel image, IEnumerable<Label> labels)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (labels == null)
            {
                throw new ArgumentNullException(nameof(labels));
            }

            #endregion

            image.Labels = LabelRepository.AddLabels(topic, image, labels).ToList();
            return image.Labels ?? new List<Label>();
        }

        public IEnumerable<Label> SaveLabels(Topic topic, ImageLabel image, IEnumerable<Label> labels)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            List<Label> result = new List<Label>();

            if (labels != null)
            {
                if (labels.Any())
                {
                    result = LabelRepository.AddOrUpdateOrDeleteLabels(topic, image, labels).ToList();
                }
                else
                {
                    LabelRepository.DeleteLabels(topic, image);
                }
            }

            return result;
        }

        public void DeleteLabels(Topic topic, ImageLabel image)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            LabelRepository.DeleteLabels(topic, image);
        }

        #endregion
    }
}