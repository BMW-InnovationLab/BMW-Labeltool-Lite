using Newtonsoft.Json;
using Rcv.FileUtils.AccessControl;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Web.Utilities;
using RCV.FileContainer.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rcv.LabelTool.Web.Repositories
{
    public class ObjectClassRepository : IObjectClassRepository
    {
        #region members

        private IFileContainer FileContainer { get; }

        #endregion

        #region constructor

        public ObjectClassRepository(IFileContainer fileContainer)
        {
            FileContainer = fileContainer ?? throw new ArgumentNullException(nameof(fileContainer));
        }

        #endregion

        #region IObjectClassRepository

        public bool ExistsObjectClass(Topic topic, uint objectClassId)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            return GetObjectClasses(topic).Any(o => o.Id == objectClassId);
        }

        public ObjectClass GetObjectClass(Topic topic, uint objectClassId)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            return GetObjectClasses(topic).Single(o => o.Id == objectClassId);
        }

        public IEnumerable<ObjectClass> GetObjectClasses(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            IEnumerable<ObjectClass> objectClasses = LoadObjectClasses(topic);
            CompleteUndefinedColors(objectClasses);
            return objectClasses;
        }

        #endregion

        #region helper functions

        private string[] GetTopicPath(Topic topic)
        {
            return new string[] { topic.FolderPath };
        }

        private void CompleteUndefinedColors(IEnumerable<ObjectClass> objectClasses)
        {
            #region validation

            if (objectClasses == null)
            {
                throw new ArgumentNullException(nameof(objectClasses));
            }

            #endregion

            foreach (ObjectClass objectClassWithUndefinedColor in objectClasses.Where(o => o.Color == ObjectClass.UNDEFINED_COLOR))
            {
                objectClassWithUndefinedColor.ColorCode = ObjectClassColorUtil.GetDefaultColorCode(objectClassWithUndefinedColor.Id);
            }
        }

        /// <summary>
        /// Loads objectclasses from objectclasses.json-file.
        /// If file doesn't exsists, emtpy list will be returned.
        /// </summary>
        /// <param name="topic">Topic of objectclasses</param>
        /// <returns>List of objectclasses</returns>
        private IEnumerable<ObjectClass> LoadObjectClasses(Topic topic)
        {
            if (!FileContainer.ExistsFile("objectclasses.json", GetTopicPath(topic)))
            {
                return new List<ObjectClass>();
            }

            return FileSession.Execute((fileName, filePath) =>
            {
                using (StreamReader streamReader = EncodingUtil.GetStreamReader(FileContainer.GetFileStream(fileName, filePath)))
                {
                    string objectclassesContent = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<IEnumerable<ObjectClass>>(objectclassesContent);
                }
            }, "objectclasses.json", GetTopicPath(topic));
        }

        #endregion
    }
}