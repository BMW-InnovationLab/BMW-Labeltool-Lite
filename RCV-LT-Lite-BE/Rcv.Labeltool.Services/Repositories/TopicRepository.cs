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
    /// <summary>
    /// Get topic data from file system.
    /// </summary>
    public class TopicRepository : ITopicRepository
    {
        #region constants

        private static readonly string TOPICS_FILE_NAME = "topics.json";

        #endregion

        #region members

        private IFileContainer FileContainer { get; }

        #endregion

        #region constructor

        public TopicRepository(IFileContainer fileContainer)
        {
            FileContainer = fileContainer ?? throw new ArgumentNullException(nameof(fileContainer));
        }

        #endregion

        #region ITopicRepository

        public bool ExistsTopic(long topicId)
        {
            return GetTopics().Any(o => o.Id == topicId);
        }

        public Topic GetTopic(long topicId)
        {
            return GetTopics().Single(o => o.Id == topicId);
        }

        public IEnumerable<Topic> GetTopics()
        {
            return FileSession.Execute((fileName, filePath) =>
            {
                using (StreamReader streamReader = EncodingUtil.GetStreamReader(FileContainer.GetFileStream(fileName, filePath)))
                {
                    string fileContent = streamReader.ReadToEnd();

                    return JsonConvert.DeserializeObject<IEnumerable<Topic>>(fileContent);
                }
            }, TOPICS_FILE_NAME);
        }

        #endregion
    }
}