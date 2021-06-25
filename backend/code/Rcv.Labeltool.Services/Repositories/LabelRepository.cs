using Rcv.FileUtils;
using Rcv.FileUtils.AccessControl;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Services.Repositories.Util;
using Rcv.LabelTool.Web.Logic;
using RCV.FileContainer.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rcv.LabelTool.Web.Repositories
{
    public class LabelRepository : ILabelRepository
    {
        #region member

        /// <summary>
        /// Filerepository to load labels.
        /// </summary>
        private IFileContainer FileContainer { get; }

        #endregion

        #region constructor

        public LabelRepository(IFileContainer fileContainer)
        {
            FileContainer = fileContainer ?? throw new ArgumentNullException(nameof(fileContainer));
        }

        #endregion

        #region ILabelRepository 

        public IEnumerable<Label> GetLabels(Topic topic, ImageLabel image)
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

            IEnumerable<Label> labels = FileSession.Execute((fileName, filePath) =>
            {
                if (!FileContainer.ExistsFile(fileName, filePath))
                {
                    return new List<Label>();
                }

                using (Stream labelFileStream = FileContainer.GetFileStream(fileName, filePath))
                {
                    return LabelStoreUtil.GetLabelsFromStream(labelFileStream);
                }
            }, image.GetLabelFileName(), GetLabelsPath(topic));

            image.SetLabels(labels);
            return labels;
        }

        public IEnumerable<string> GetLabelFileNames(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            // get paths of all labelfiles
            IEnumerable<string> labelFilePaths = FileContainer.GetFiles(GetLabelsPath(topic));

            // extract labelfilename without extension
            // equal to imagefilename without extension
            List<string> labelFileNames = new List<string>(labelFilePaths.Count());
            foreach (string labelFilePath in labelFilePaths)
            {
                labelFileNames.Add(FileNameUtil.GetFileNameWithoutExtension(labelFilePath));
            }

            return labelFileNames;
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

            // set id for labels
            foreach (Label label in labels)
            {
                label.Id = labels.GetNext(o => o.Id) + 1;
            }

            FileSession.Execute((fileName, filePath) =>
            {
                image.Labels = labels.ToList();
                byte[] labelFileContent = image.GetLabelsAsByte();
                FileContainer.CreateFile(fileName, labelFileContent, filePath);

                return true;
            }, image.GetLabelFileName(), GetLabelsPath(topic));

            return labels;
        }

        public IEnumerable<Label> AddOrUpdateOrDeleteLabels(Topic topic, ImageLabel image, IEnumerable<Label> labels)
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

            // set id for undefined label-ids
            // undefined label id will be lower than 0
            foreach (Label label in labels)
            {
                if (label.Id <= 0)
                {
                    label.Id = labels.GetNext(o => o.Id) + 1;
                }
            }

            FileSession.Execute((fileName, filePath) =>
            {
                image.Labels = labels.ToList();
                byte[] labelFileContent = image.GetLabelsAsByte();
                FileContainer.CreateFile(fileName, labelFileContent, filePath);

                return true;
            }, image.GetLabelFileName(), GetLabelsPath(topic));

            return labels;
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

            FileContainer.DeleteFile(image.GetLabelFileName(), GetLabelsPath(topic));
        }

        #endregion

        #region helper functions

        private string[] GetLabelsPath(Topic topic)
        {
            var path = FileContainer.CreateDirectory("labels", new[] { topic.FolderPath });
            return FileContainer.CreateDirectory("json", path);
        }

        #endregion
    }
}