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
using System.Text.Json;

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

            // Fill parent-child hierarchy of objectclasses
            foreach (ObjectClass objectClass in objectClasses)
            {
                if (objectClass.ParentObjectClassId.HasValue)
                {
                    objectClass.ParentObjectClass = objectClasses.Single(o => o.Id == objectClass.ParentObjectClassId);
                }
                objectClass.ObjectClasses = objectClasses.Where(o => o.ParentObjectClassId == objectClass.Id);
            }

            return objectClasses;
        }

        public ObjectClass AddObjectClass(Topic topic, ObjectClass objectClass)
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

            // get id for new objectclass
            List<ObjectClass> classes = GetObjectClasses(topic).ToList();
            objectClass.Id = classes.GetNext(o => o.Id);
            classes.Add(objectClass);

            // save classes file to filesystem
            SaveObjectClasses(topic, classes);

            return objectClass;
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

            var objectClasses = FileSession.Execute((fileName, filePath) =>
            {
                using (StreamReader streamReader = EncodingUtil.GetStreamReader(FileContainer.GetFileStream(fileName, filePath)))
                {
                    string objectclassesContent = streamReader.ReadToEnd();
                    return JsonSerializer.Deserialize<IEnumerable<ObjectClass>>(objectclassesContent);
                }
            }, "objectclasses.json", GetTopicPath(topic));

            // set topic reference id
            foreach (ObjectClass objectClass in objectClasses)
            {
                objectClass.TopicId = topic.Id;
            }

            return objectClasses;
        }

        /// <summary>
        /// Save list of objectclasses in json-file.
        /// </summary>
        /// <param name="topic">Topic of objectclasses</param>
        /// <param name="objectClasses">List of objectclasses to save</param>
        private void SaveObjectClasses(Topic topic, IEnumerable<ObjectClass> objectClasses)
        {
            FileSession.Execute((fileName, filePath) =>
            {
                string objectclassesAsJson = JsonSerializer.Serialize(objectClasses);
                byte[] newFileContent = EncodingUtil.GetBytes(objectclassesAsJson);
                FileContainer.CreateFile(fileName, newFileContent, filePath);

                return true;
            }, "objectclasses.json", GetTopicPath(topic));
        }

        #endregion
    }
}