using Rcv.FileUtils;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Web.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Rcv.LabelTool.Services.Repositories.Util
{
    /// <summary>
    /// Util class for label file name determination.
    /// </summary>
    internal static class LabelStoreUtil
    {
        /// <summary>
        /// Determines label file name from imagelabel information.
        /// </summary>
        /// <param name="image">Imagelabel to get label file name from</param>
        /// <returns>Name of label file</returns>
        public static string GetLabelFileName(this ImageLabel image)
        {
            #region validation

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            return GetLabelFileName(image.Id);
        }

        /// <summary>
        /// Determines label file name from imagefilename.
        /// </summary>
        /// <param name="imageFileName">Name of imagefile</param>
        /// <returns>Name of labelfile</returns>
        public static string GetLabelFileName(string imageFileName)
        {
            #region validation

            if (string.IsNullOrEmpty(imageFileName))
            {
                throw new ArgumentNullException(nameof(imageFileName));
            }

            #endregion

            // get name of imagefile without extension
            string imageFileNameRaw = FileNameUtil.GetFileNameWithoutExtension(imageFileName);

            // labelname for that image
            return $"{imageFileNameRaw}.json";
        }

        /// <summary>
        /// Get labels as byte representation to store then in filesystem.
        /// </summary>
        /// <param name="image">Imagelabel to get label content as byte array</param>
        /// <returns>Byte array representation of image labels</returns>
        public static byte[] GetLabelsAsByte(this ImageLabel image)
        {
            #region validation

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            #endregion

            string labelsJson = JsonSerializer.Serialize(image.Labels);
            return EncodingUtil.GetBytes(labelsJson);
        }

        /// <summary>
        /// Get label list from file stream content.
        /// </summary>
        /// <param name="labelFileStream">Stream of label file content</param>
        /// <returns>List of labels from content stream</returns>
        public static IEnumerable<Label> GetLabelsFromStream(Stream labelFileStream)
        {
            #region validation

            if (labelFileStream == null)
            {
                throw new ArgumentNullException(nameof(labelFileStream));
            }

            #endregion

            using (StreamReader streamReader = EncodingUtil.GetStreamReader(labelFileStream))
            {
                string labelContentJson = streamReader.ReadToEnd();
                return JsonSerializer.Deserialize<IEnumerable<Label>>(labelContentJson);
            }
        }
    }
}
