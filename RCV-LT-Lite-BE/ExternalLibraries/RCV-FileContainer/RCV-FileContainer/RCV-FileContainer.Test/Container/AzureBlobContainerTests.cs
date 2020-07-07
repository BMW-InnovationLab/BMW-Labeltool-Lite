using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using RCV.FileContainer.Container;
using RCV.FileContainer.Contracts;
using RCV.FileContainer.Extensions;
using RCV.FileContainer.Utilities;

namespace RCV_FileContainer.Test.Container
{
    [TestClass]
    public class AzureBlobContainerTests
    {
        #region private fields

        private IFileContainer _fileContainer;

        private CloudStorageAccount _cloudStorageAccount;

        private CloudBlobContainer _cloudBlobContainer;

        private const string CONNECTION_STRING = "DefaultEndpointsProtocol=https;AccountName=bmwrcvdev;AccountKey=YW6ZSxrc4MOM3QhEGHyBkhZxyyEXVW1xd3ZB/thC5zSjBHQ68THwexwKR1jvl3vxPkHNQ+5VNWZHx4lWjAt1hw==;EndpointSuffix=core.windows.net";

        private const string CONTAINER_NAME = "automatedtests";

        private const string FILE_NAME = "CopyFile.txt";

        private const string FILE_CONTENT = "FooBar";

        private const string NEW_DIRECTORY = "NewDirectory";

        private const string SOURCE_DIRECTORY = "SourceDirectory";

        private const string TARGET_DIRECTORY = "TargetDirectory";

        private const string DUMMY_DIRECTORY = "DummyDirectory";

        #endregion

        #region test initialization

        [TestInitialize]
        public void Initialize()
        {
            _fileContainer = new AzureBlobContainer(CONNECTION_STRING, CONTAINER_NAME);

            _cloudStorageAccount = CloudStorageAccount.Parse(CONNECTION_STRING);

            _cloudBlobContainer = _cloudStorageAccount.CreateCloudBlobClient().GetContainerReference(CONTAINER_NAME);

            // SourceDirectory/CopyFile.txt
            TaskUtilities.ExecuteSync(_cloudBlobContainer.GetCloudBlockBlobReference(FILE_NAME, SOURCE_DIRECTORY).UploadTextAsync(FILE_CONTENT));

            // SourceDirectory/DummyDirectory/CopyFile.txt
            TaskUtilities.ExecuteSync(_cloudBlobContainer.GetCloudBlockBlobReference(FILE_NAME, DUMMY_DIRECTORY, new[] { SOURCE_DIRECTORY }).UploadTextAsync(FILE_CONTENT));

            // TargetDirectory/CopyFile.txt
            TaskUtilities.ExecuteSync(_cloudBlobContainer.GetCloudBlockBlobReference(FILE_NAME, TARGET_DIRECTORY).UploadTextAsync(FILE_CONTENT));

            // CopyFile.txt
            TaskUtilities.ExecuteSync(_cloudBlobContainer.GetCloudBlockBlobReference(FILE_NAME).UploadTextAsync(FILE_CONTENT));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _cloudBlobContainer.GetDirectoryReference(string.Empty).DeleteRecursive();
        }

        #endregion

        #region test methods

        [DataTestMethod]
        [DataRow(SOURCE_DIRECTORY, null, new[] { TARGET_DIRECTORY }, DisplayName = "Copy directory with target directory")]
        [DataRow(DUMMY_DIRECTORY, new[] { SOURCE_DIRECTORY }, null, DisplayName = "Copy directory with source directory")]
        [DataRow(DUMMY_DIRECTORY, new[] { SOURCE_DIRECTORY }, new[] { TARGET_DIRECTORY }, DisplayName = "Copy directory with both directories")]
        public void CopyDirectoryTest(string directoryName, string[] sourcePath, string[] targetPath)
        {
            // Arrange
            string relativeAddress = BlobUtilities.GetPath(directoryName, sourcePath);

            CloudBlobDirectory sourceCloudBlobDirectory = _cloudBlobContainer.GetDirectoryReference(relativeAddress);

            int expectedChildCount = TaskUtilities
                .ExecuteSync(
                    sourceCloudBlobDirectory.ListBlobsSegmentedAsync(
                        useFlatBlobListing: true,
                        blobListingDetails: BlobListingDetails.None,
                        maxResults: null,
                        currentToken: new BlobContinuationToken(),
                        options: new BlobRequestOptions(),
                        operationContext: new OperationContext()
                    )
                )
                .Results
                .Count();

            // Act
            _fileContainer.CopyDirectory(directoryName, sourcePath, targetPath);

            CloudBlobDirectory targetCloudBlobDirectory = _cloudBlobContainer.GetDirectoryReference(BlobUtilities.GetPath(directoryName, targetPath));

            int actualChildCount = TaskUtilities
                .ExecuteSync(
                    targetCloudBlobDirectory.ListBlobsSegmentedAsync(
                        useFlatBlobListing: true,
                        blobListingDetails: BlobListingDetails.None,
                        maxResults: null,
                        currentToken: new BlobContinuationToken(),
                        options: new BlobRequestOptions(),
                        operationContext: new OperationContext()
                    )
                )
                .Results
                .Count();

            // Assert
            Assert.AreEqual(expectedChildCount, actualChildCount);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, FILE_NAME, new[] { TARGET_DIRECTORY, SOURCE_DIRECTORY }, true, DisplayName = "Copy file with target directory")]
        [DataRow(FILE_NAME, new[] { SOURCE_DIRECTORY }, FILE_NAME, null, true, DisplayName = "Copy file with source directory")]
        [DataRow(FILE_NAME, new[] { SOURCE_DIRECTORY }, FILE_NAME, new[] { TARGET_DIRECTORY, SOURCE_DIRECTORY }, true, DisplayName = "Copy file with both directories")]
        public void CopyFileTest(string sourceFileName, string[] sourceParentFolders, string targetFileName, string[] targetParentFolders, bool overwriteIfExists = false)
        {
            // Act
            _fileContainer.CopyFile(sourceFileName, sourceParentFolders, targetFileName, targetParentFolders, overwriteIfExists);

            CloudBlockBlob cloudBlockBlobReference = _cloudBlobContainer.GetCloudBlockBlobReference(targetFileName, path: targetParentFolders);

            bool result = TaskUtilities.ExecuteSync(cloudBlockBlobReference.ExistsAsync());

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(new []{ NEW_DIRECTORY }, NEW_DIRECTORY , null, DisplayName = "Create directory without path")]
        [DataRow(new []{ TARGET_DIRECTORY, NEW_DIRECTORY }, NEW_DIRECTORY,  new[] { TARGET_DIRECTORY }, DisplayName = "Create directory with path")]
        public void CreateDirectoryTest(string[] expectedResult, string directoryName, string[] path)
        {
            // Act
            string[] actualResult = _fileContainer.CreateDirectory(directoryName, path);

            // Assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, new byte[] { 0x46, 0x6F, 0x6F, 0x42, 0x61, 0x72 }, null, DisplayName = "Create file with byte array without path")]
        [DataRow(FILE_NAME, new byte[] { 0x46, 0x6F, 0x6F, 0x42, 0x61, 0x72 }, new[] { TARGET_DIRECTORY, SOURCE_DIRECTORY }, DisplayName = "Create file with byte array with path")]
        public void CreateFileWithByteArrayTest(string fileName, byte[] fileContent, string[] path)
        {
            // Act
            _fileContainer.CreateFile(fileName, fileContent, path);

            CloudBlob cloudBlob = _cloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlob.FetchAttributesAsync());

            var actualResult = new byte[cloudBlob.Properties.Length];

            CloudBlockBlob cloudBlockBlobReference = _cloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlockBlobReference.DownloadToByteArrayAsync(actualResult, 0));

            // Assert
            CollectionAssert.AreEqual(fileContent, actualResult);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Create file with stream without path")]
        [DataRow(FILE_NAME, new object[] { new[] { TARGET_DIRECTORY, SOURCE_DIRECTORY } }, DisplayName = "Create file with stream with path")]
        public void CreateFileWithStreamTest(string fileName, string[] path)
        {
            // Act
            _fileContainer.CreateFile(fileName, Stream.Null, path);

            CloudBlockBlob cloudBlockBlobReference = _cloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            bool result = TaskUtilities.ExecuteSync(cloudBlockBlobReference.ExistsAsync());

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(SOURCE_DIRECTORY, null, DisplayName = "Delete directory without path")]
        [DataRow(DUMMY_DIRECTORY, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Delete directory with path")]
        public void DeleteDirectoryTest(string directoryName, string[] path)
        {
            // Act
            _fileContainer.DeleteDirectory(directoryName, path);

            bool result = _fileContainer.ExistsDirectory(directoryName, path);

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Delete file without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Delete file with path")]
        public void DeleteFileTest(string fileName, string[] path)
        {
            // Act
            _fileContainer.DeleteFile(fileName, path);

            CloudBlockBlob cloudBlockBlobReference = _cloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            bool result = TaskUtilities.ExecuteSync(cloudBlockBlobReference.ExistsAsync());

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(SOURCE_DIRECTORY, null, DisplayName = "Exists directory without path")]
        [DataRow(DUMMY_DIRECTORY, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Exists directory with path")]
        public void ExistsDirectoryTest(string directoryName, string[] path)
        {
            // Act
            bool result = _fileContainer.ExistsDirectory(directoryName, path);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Exists file without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Exists file with path")]
        public void ExistsFileTest(string fileName, string[] path)
        {
            // Act
            bool result = _fileContainer.ExistsFile(fileName, path);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExistsPathTest()
        {
            // Act
            bool result = _fileContainer.ExistsPath(new[] { SOURCE_DIRECTORY });

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(new[] { "https://bmwrcvdev.blob.core.windows.net/automatedtests/SourceDirectory", "https://bmwrcvdev.blob.core.windows.net/automatedtests/TargetDirectory" }, null, DisplayName = "Get directories without path")]
        [DataRow(new[] { "https://bmwrcvdev.blob.core.windows.net/automatedtests/SourceDirectory/DummyDirectory" }, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get directories with path")]
        public void GetDirectoriesTest(string[] expectedResult, string[] path)
        {
            // Act
            string[] actualResult = _fileContainer
                .GetDirectories(path)
                .ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(new[] { SOURCE_DIRECTORY, TARGET_DIRECTORY }, "*", null, DisplayName = "Get directory names without path")]
        [DataRow(new[] { DUMMY_DIRECTORY }, "*", new[] { SOURCE_DIRECTORY }, DisplayName = "Get directory names with path")]
        public void GetDirectoryNamesTest(string[] expectedResult, string searchPattern, string[] path)
        {
            // Act
            string[] actualResult = _fileContainer.GetDirectoryNames(searchPattern, path).ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Get file content without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get file content with path")]
        public void GetFileContentTest(string fileName, string[] path)
        {
            // Act
            string result = _fileContainer.GetFileContent(fileName, path);

            // Assert
            Assert.AreEqual(FILE_CONTENT, result);
        }

        [DataTestMethod]
        [DataRow(new[] { FILE_NAME }, "*", null, DisplayName = "Get file names without path and with extension")]
        [DataRow(new[] { FILE_NAME }, "*", new[] { SOURCE_DIRECTORY }, DisplayName = "Get file names with path and with extension")]
        public void GetFileNamesTest(string[] expectedResult, string searchPattern, string[] path)
        {
            // Act
            string[] actualResult = _fileContainer.GetFileNames(searchPattern, path).ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(new[] { "https://bmwrcvdev.blob.core.windows.net/automatedtests/CopyFile.txt" }, null, DisplayName = "Get files without path")]
        [DataRow(new[] { "https://bmwrcvdev.blob.core.windows.net/automatedtests/SourceDirectory/CopyFile.txt" }, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get files with path")]
        public void GetFilesTest(string[] expectedResult, string[] path)
        {
            // Act
            string[] actualResult = _fileContainer
                .GetFiles(path)
                .ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Get file stream without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get file stream with path")]
        public void GetFileStreamTest(string fileName, string[] path)
        {
            // Arrange
            bool result;

            // Act
            using (Stream stream = _fileContainer.GetFileStream(fileName, new[] { SOURCE_DIRECTORY }))
            {
                result = stream.CanRead && !stream.CanWrite;
            }

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Get write stream without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get write stream with path")]
        public void GetWriteStreamTest(string fileName, string[] path)
        {
            // Arrange
            bool result;

            // Act
            using (Stream stream = _fileContainer.GetWriteStream(FILE_NAME, path))
            {
                result = !stream.CanRead && stream.CanWrite;
            }

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(SOURCE_DIRECTORY, null, new[] { TARGET_DIRECTORY }, DisplayName = "Move directory without source path")]
        [DataRow(DUMMY_DIRECTORY, new[] { SOURCE_DIRECTORY }, null, DisplayName = "Move directory without target path")]
        [DataRow(DUMMY_DIRECTORY, new[] { SOURCE_DIRECTORY }, new[] { TARGET_DIRECTORY }, DisplayName = "Move directory with both paths")]
        public void MoveDirectoryTest(string directoryName, string[] sourcePath, string[] targetPath)
        {
            // Arrange
            string relativeAddress = BlobUtilities.GetPath(directoryName, sourcePath);

            CloudBlobDirectory sourceCloudBlobDirectory = _cloudBlobContainer.GetDirectoryReference(relativeAddress);

            int expectedChildCount = TaskUtilities
                .ExecuteSync(
                    sourceCloudBlobDirectory.ListBlobsSegmentedAsync(
                        useFlatBlobListing: true,
                        blobListingDetails: BlobListingDetails.None,
                        maxResults: null,
                        currentToken: new BlobContinuationToken(),
                        options: new BlobRequestOptions(),
                        operationContext: new OperationContext()
                    )
                )
                .Results.Count();

            // Act
            _fileContainer.MoveDirectory(directoryName, sourcePath, targetPath);

            string path = BlobUtilities.GetPath(directoryName, targetPath);

            CloudBlobDirectory targetCloudBlobDirectory = _cloudBlobContainer.GetDirectoryReference(path);

            int actualChildCount = TaskUtilities
                .ExecuteSync(
                    targetCloudBlobDirectory.ListBlobsSegmentedAsync(
                        useFlatBlobListing: true,
                        blobListingDetails: BlobListingDetails.None,
                        maxResults: null,
                        currentToken: new BlobContinuationToken(),
                        options: new BlobRequestOptions(),
                        operationContext: new OperationContext()
                    )
                )
                .Results.Count();

            // Assert
            Assert.IsFalse(_fileContainer.ExistsDirectory(directoryName, sourcePath));

            Assert.AreEqual(expectedChildCount, actualChildCount);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Get write stream without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get write stream with path")]
        public void ReadAllBytesTest(string fileName, string[] path)
        {
            // Arrange
            var expectedResult = new byte[] { 0x46, 0x6F, 0x6F, 0x42, 0x61, 0x72 };

            // Act
            byte[] actualResult = _fileContainer.ReadAllBytes(fileName, path);

            // Assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, FILE_CONTENT, null, DisplayName = "Set file content without path")]
        [DataRow(FILE_NAME, FILE_CONTENT, new[] { SOURCE_DIRECTORY }, DisplayName = "Set file content with path")]
        public void SetFileContentTest(string fileName, string fileContent, string[] path)
        {
            // Act
            _fileContainer.SetFileContent(fileName, fileContent, path);

            var target = new byte[6];

            CloudBlockBlob cloudBlockBlobReference = _cloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlockBlobReference.DownloadToByteArrayAsync(target, 0));

            string result = Encoding.UTF8.GetString(target);

            // Assert
            Assert.AreEqual(fileContent, result);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Set file stream without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Set file stream with path")]
        public void SetFileStreamTest(string fileName, string[] path)
        {
            // Arrange
            using (var streamContent = new MemoryStream(Encoding.Default.GetBytes(FILE_CONTENT)))
            {
                _fileContainer.SetFileStream(fileName, streamContent, path);
            }

            var target = new byte[6];

            CloudBlockBlob cloudBlockBlobReference = _cloudBlobContainer.GetCloudBlockBlobReference(fileName, path: path);

            TaskUtilities.ExecuteSync(cloudBlockBlobReference.DownloadToByteArrayAsync(target, 0));

            string result = Encoding.UTF8.GetString(target);

            // Assert
            Assert.AreEqual(FILE_CONTENT, result);
        }

        #endregion
    }
}