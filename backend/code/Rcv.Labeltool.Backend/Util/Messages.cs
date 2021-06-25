namespace Rcv.LabelTool.Backend.Util
{
    /// <summary>
    /// Messages which will be reported by the webAPI.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// Message which is reported when image with given id can not be found.
        /// </summary>
        /// <param name="imageId">Id of image which can not be found</param>
        /// <returns>Message</returns>
        public static string ImageNotFound(string imageId)
        {
            return $"Image {imageId} not found!";
        }

        /// <summary>
        /// Message which is reported when image with given index can not be found.
        /// </summary>
        /// <param name="index">Index of image which can not be found</param>
        /// <returns>Message</returns>
        public static string ImageNotFound(uint index)
        {
            return $"Image at index {index} not found!";
        }

        /// <summary>
        /// Message which is reported when topic can not be found.
        /// </summary>
        /// <param name="topicId">Id of topic which can not be found</param>
        /// <returns>Message</returns>
        public static string TopicNotFound(long topicId)
        {
            return $"Topic {topicId} not found!";
        }

        /// <summary>
        /// Message which is reported when objectclass can not be found.
        /// </summary>
        /// <param name="objectClassId">Id of objectclass which can not be found</param>
        /// <returns>Message</returns>
        public static string ObjectClassNotFound(long objectClassId)
        {
            return $"Objectclass {objectClassId} not found!";
        }

        /// <summary>
        /// Message which is reported when 2D-topic labels contains
        /// 3D-coordinates.
        /// </summary>
        /// <returns>Message</returns>
        public static string Invalid2DLabelCoordinates()
        {
            return $"Labels of 2D-topic should have foreground and no background coordinates!";
        }

        /// <summary>
        /// Message which is reported when 3D-topic labels contains
        /// 2D-coordinates.
        /// </summary>
        /// <returns>Message</returns>
        public static string Invalid3DLabelCoordinates()
        {
            return $"Labels of 3D-topic should have foreground and background coordinates!";
        }

        /// <summary>
        /// Message which is reported when a label is outside of image
        /// dimension.
        /// </summary>
        /// <param name="imageWidth">Width of image</param>
        /// <param name="imageHeight">Height of image</param>
        /// <returns>Message</returns>
        public static string LabelCoordinatesOutsideOfImage(long imageWidth, long imageHeight)
        {
            return $"Label coordinates outside of image area ({imageWidth} x {imageHeight})!";
        }
    }
}
