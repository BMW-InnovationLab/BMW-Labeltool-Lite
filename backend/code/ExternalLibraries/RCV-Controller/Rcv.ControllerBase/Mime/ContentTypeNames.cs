using System.Net.Mime;

namespace Rcv.Base.Mime
{
    /// <summary>
    /// Names of content types.
    /// </summary>
    public static class ContentTypeNames
    {
        /// <summary>
        /// Content types of multipart content.
        /// </summary>
        public static class Multipart
        {
            /// <summary>
            /// Form-data multipart content.
            /// </summary>
            public const string FormData = "multipart/form-data";
        }

        /// <summary>
        /// Content types of application content.
        /// </summary>
        public static class Application
        {
            /// <summary>
            /// Json content.
            /// </summary>
            public const string Json = MediaTypeNames.Application.Json;

            /// <summary>
            /// X-www-form-urlencoded content.
            /// </summary>
            public const string XWWWFormUrlEncoded = "x-www-form-urlencoded";
        }
    }
}
