using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using System;
using System.Collections.Generic;

namespace Rcv.LabelTool.Web.Services
{
    public class LabelService : ILabelService
    {
        #region member

        private ILabelRepository LabelRepository { get; }

        #endregion

        #region constructor

        public LabelService(ILabelRepository labelRepository)
        {
            LabelRepository = labelRepository ?? throw new ArgumentNullException(nameof(labelRepository));
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

            return LabelRepository.GetLabels(topic, imageLabel);
        }

        public IEnumerable<string> GetLabeledImageFileNames(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            return LabelRepository.GetLabelFileNames(topic);
        }

        public IEnumerable<Label> SaveLabels(Topic topic, ImageLabel image)
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

            if (image.Labels != null)
            {
                if (image.Labels.Count > 0)
                {
                    IEnumerable<Label> result = LabelRepository.CreateLabels(topic, image);
                    return result;
                }
                else
                {
                    LabelRepository.DeleteLabels(topic, image);
                    return new List<Label>();
                }
            }
            return null;
        }

        #endregion
    }
}