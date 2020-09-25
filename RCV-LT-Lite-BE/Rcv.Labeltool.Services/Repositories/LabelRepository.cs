using Newtonsoft.Json;
using Rcv.FileUtils.AccessControl;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Web.Logic;
using Rcv.LabelTool.Web.Utilities;
using RCV.FileContainer.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

            IEnumerable<Label> labels = FileSession.Execute((fileName, filePath) =>
            {
                if (!FileContainer.ExistsFile(fileName, filePath))
                {
                    return new List<Label>();
                }

                using (StreamReader streamReader = EncodingUtil.GetStreamReader(FileContainer.GetFileStream(fileName, filePath), Encoding.UTF8))
                {
                    string labelContentJson = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<IEnumerable<Label>>(labelContentJson);
                }
            }, GetLabelFileName(imageLabel.Url), GetLabelsPath(topic));

            imageLabel.SetLabels(labels);
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

        public IEnumerable<Label> CreateLabels(Topic topic, ImageLabel imageLabel)
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

            // set id for undefined label-ids
            // undefined label id will be lower than 0
            foreach (Label label in imageLabel.Labels)
            {
                if (label.Id < 0)
                {
                    label.Id = imageLabel.Labels.GetNext(o => o.Id);
                }
            }

            FileSession.Execute((fileName, filePath) =>
            {
                string labelssAsJson = JsonConvert.SerializeObject(imageLabel.Labels);
                byte[] newFileContent = EncodingUtil.GetBytes(labelssAsJson, Encoding.UTF8);
                FileContainer.CreateFile(fileName, newFileContent, filePath);

                return true;
            }, GetLabelFileName(imageLabel.Url), GetLabelsPath(topic));

            return imageLabel.Labels;
        }

        public void DeleteLabels(Topic topic, ImageLabel imageLabel)
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

            FileContainer.DeleteFile(GetLabelFileName(imageLabel.Id), GetLabelsPath(topic));
        }

        #endregion

        #region helper functions

        private string[] GetLabelsPath(Topic topic)
        {
            return FileContainer.CreateDirectory("labels/json", new[] { topic.FolderPath });
        }

        private string GetLabelFileName(string imageFileName)
        {
            string imgFileRaw = FileNameUtil.GetFileNameWithoutExtension(imageFileName);

            // labelname for that image
            string labelFileName = $"{imgFileRaw}.json";
            return labelFileName;
        }

        #endregion
    }
}