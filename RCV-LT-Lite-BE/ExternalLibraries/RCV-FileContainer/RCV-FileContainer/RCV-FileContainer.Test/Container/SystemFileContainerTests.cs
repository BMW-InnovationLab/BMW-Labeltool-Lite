using Microsoft.VisualStudio.TestTools.UnitTesting;
using RCV.FileContainer.Container;
using RCV.FileContainer.Contracts;
using RCV.FileContainer.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RCV_FileContainer.Test.Container
{
    [TestClass]
    public class SystemFileContainerTests
    {
        #region private fields

        private FileUtilities _fileUtilities;

        private IFileContainer _fileContainer;

        private DirectoryInfo _rootDirectoryInfo;

        private const string FILE_NAME = "CopyFile.txt";

        private const string FILE_CONTENT = "FooBar";

        private const string ROOT_DIRECTORY = @".\TestRoot";

        private const string NEW_DIRECTORY = "NewDirectory";

        private const string SOURCE_DIRECTORY = "SourceDirectory";

        private const string TARGET_DIRECTORY = "TargetDirectory";

        private const string DUMMY_DIRECTORY = "DummyDirectory";

        #endregion

        #region test initialization

        [TestInitialize]
        public void Initialize()
        {
            _fileUtilities = new FileUtilities(ROOT_DIRECTORY);

            _fileContainer = new SystemFileContainer(ROOT_DIRECTORY);

            _rootDirectoryInfo = new DirectoryInfo(ROOT_DIRECTORY);

            _rootDirectoryInfo.Create();

            _rootDirectoryInfo.CreateSubdirectory(SOURCE_DIRECTORY);

            _rootDirectoryInfo.CreateSubdirectory($@"{SOURCE_DIRECTORY}\{DUMMY_DIRECTORY}");

            File.WriteAllText($@"{ROOT_DIRECTORY}\{FILE_NAME}", FILE_CONTENT, Encoding.Unicode);

            File.WriteAllText($@"{ROOT_DIRECTORY}\{SOURCE_DIRECTORY}\{FILE_NAME}", FILE_CONTENT, Encoding.Unicode);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _rootDirectoryInfo.Delete(true);

            _rootDirectoryInfo = null;

            _fileContainer = null;
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
            string fullSourcePath = _fileUtilities.GetPath(directoryName, sourcePath);

            string fullTargetPath = _fileUtilities.GetPath(directoryName, targetPath);

            // Act
            _fileContainer.CopyDirectory(directoryName, sourcePath, targetPath);

            // Assert
            Assert.IsTrue(Directory.Exists(fullTargetPath));

            Assert.AreEqual(Directory.GetFiles(fullSourcePath).Length, Directory.GetFiles(fullTargetPath).Length);

            Assert.AreEqual(Directory.GetDirectories(fullSourcePath).Length, Directory.GetDirectories(fullTargetPath).Length);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, FILE_NAME, new[] { TARGET_DIRECTORY, SOURCE_DIRECTORY }, true, DisplayName = "Copy file with target directory")]
        [DataRow(FILE_NAME, new[] { SOURCE_DIRECTORY }, FILE_NAME, null, true, DisplayName = "Copy file with source directory")]
        [DataRow(FILE_NAME, new[] { SOURCE_DIRECTORY }, FILE_NAME, new[] { TARGET_DIRECTORY, SOURCE_DIRECTORY }, true, DisplayName = "Copy file with both directories")]
        public void CopyFileTest(string sourceFileName, string[] sourceParentFolders, string targetFileName, string[] targetParentFolders, bool overwriteIfExists = false)
        {
            // Arrange
            string targetFilePath = _fileUtilities.GetPath(targetFileName, targetParentFolders);

            // Act
            _fileContainer.CopyFile(sourceFileName, sourceParentFolders, targetFileName, targetParentFolders, overwriteIfExists);

            bool result = File.Exists(targetFilePath);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(NEW_DIRECTORY, null, DisplayName = "Create directory without path")]
        [DataRow(NEW_DIRECTORY, new object[] { new[] { TARGET_DIRECTORY } }, DisplayName = "Create directory with path")]
        public void CreateDirectoryTest(string directoryName, string[] path)
        {
            // Act
            _fileContainer.CreateDirectory(directoryName, path);

            bool existsDirectory = _fileContainer.ExistsDirectory(directoryName, path);

            // Assert
            Assert.IsTrue(existsDirectory);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, new byte[] { }, null, DisplayName = "Create file with byte array without path")]
        [DataRow(FILE_NAME, new byte[] { }, new[] { TARGET_DIRECTORY, SOURCE_DIRECTORY }, DisplayName = "Create file with byte array with path")]
        public void CreateFileWithByteArrayTest(string fileName, byte[] fileContent, string[] path)
        {
            // Arrange
            string targetFilePath = _fileUtilities.GetPath(fileName, path);

            // Act
            _fileContainer.CreateFile(fileName, fileContent, path);

            bool result = File.Exists(targetFilePath);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Create file with stream without path")]
        [DataRow(FILE_NAME, new object[] { new[] { TARGET_DIRECTORY, SOURCE_DIRECTORY } }, DisplayName = "Create file with stream with path")]
        public void CreateFileWithStreamTest(string fileName, string[] path)
        {
            // Arrange
            string targetFilePath = _fileUtilities.GetPath(fileName, path);

            // Act
            _fileContainer.CreateFile(fileName, Stream.Null, path);

            bool result = File.Exists(targetFilePath);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow(SOURCE_DIRECTORY, null, DisplayName = "Delete directory without path")]
        [DataRow(DUMMY_DIRECTORY, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Delete directory with path")]
        public void DeleteDirectoryTest(string directoryName, string[] path)
        {
            // Arrange
            string targetDirectoryPath = _fileUtilities.GetPath(directoryName, path);

            // Act
            _fileContainer.DeleteDirectory(directoryName, path);

            bool result = Directory.Exists(targetDirectoryPath);

            // Assert
            Assert.IsFalse(result);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Delete file without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Delete file with path")]
        public void DeleteFileTest(string fileName, string[] path)
        {
            // Arrange
            string targetFilePath = _fileUtilities.GetPath(fileName, path);

            // Act
            _fileContainer.DeleteFile(fileName, path);

            bool result = File.Exists(targetFilePath);

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
        [DataRow(null, null, DisplayName = "Get directories without path")]
        [DataRow(new[] { SOURCE_DIRECTORY }, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get directories with path")]
        public void GetDirectoriesTest(string[] directoryPath, string[] path)
        {
            // Arrange
            string directoryPathAsString = _fileUtilities.GetPath(directoryPath);

            var directoryInfo = new DirectoryInfo(directoryPathAsString);

            List<string> expectedResult = directoryInfo.GetDirectories().Select(d => d.FullName).ToList();

            // Act
            List<string> actualResult = _fileContainer.GetDirectories(path).ToList();

            // Assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(SOURCE_DIRECTORY, "*", null, DisplayName = "Get directory names without path")]
        [DataRow(DUMMY_DIRECTORY, "*", new[] { SOURCE_DIRECTORY }, DisplayName = "Get directory names with path")]
        public void GetDirectoryNamesTest(string expectedDirectoryName, string searchPattern, string[] path)
        {
            // Arrange
            var expectedResult = new List<string> { expectedDirectoryName };

            // Act
            List<string> actualResult = _fileContainer.GetDirectoryNames(searchPattern, path).ToList();

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
        [DataRow(FILE_NAME, "*", null, DisplayName = "Get file names without path and with extension")]
        [DataRow(FILE_NAME, "*", new[] { SOURCE_DIRECTORY }, DisplayName = "Get file names with path and with extension")]
        public void GetFileNamesTest(string expectedFileName, string searchPattern, string[] path)
        {
            // Arrange
            var expectedResult = new List<string> { expectedFileName };

            // Act
            List<string> actualResult = _fileContainer.GetFileNames(searchPattern, path).ToList();

            // Assert
            CollectionAssert.AreEqual(expectedResult, actualResult);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Get files without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get files with path")]
        public void GetFilesTest(string fileName, string[] path)
        {
            // Arrange
            string filePathAsString = _fileUtilities.GetPath(fileName, path);

            var fileInfo = new FileInfo(filePathAsString);

            var expectedResult = new List<string> { fileInfo.FullName };

            // Act
            List<string> actualResult = _fileContainer.GetFiles(path).ToList();

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
            using (Stream stream = _fileContainer.GetFileStream(fileName, path))
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
            string fullSourcePath = _fileUtilities.GetPath(directoryName, sourcePath);

            string fullTargetPath = _fileUtilities.GetPath(directoryName, targetPath);

            string parentDirectoryPath = _fileUtilities.GetPath(targetPath);

            Directory.CreateDirectory(parentDirectoryPath);

            int expectedFileCount = Directory.GetFiles(fullSourcePath).Length;

            int expectedDirectoryCount = Directory.GetDirectories(fullSourcePath).Length;

            // Act
            _fileContainer.MoveDirectory(directoryName, sourcePath, targetPath);

            // Assert
            Assert.IsFalse(Directory.Exists(fullSourcePath));

            Assert.IsTrue(Directory.Exists(fullTargetPath));

            Assert.AreEqual(expectedFileCount, Directory.GetFiles(fullTargetPath).Length);

            Assert.AreEqual(expectedDirectoryCount, Directory.GetDirectories(fullTargetPath).Length);

        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Get write stream without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Get write stream with path")]
        public void ReadAllBytesTest(string fileName, string[] path)
        {
            // Arrange
            string sourceFilePath = _fileUtilities.GetPath(fileName, path);

            byte[] expectedResult = File.ReadAllBytes(sourceFilePath);

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
            // Arrange
            string filePath = _fileUtilities.GetPath(fileName, path);

            // Act
            _fileContainer.SetFileContent(fileName, fileContent, path);

            string result = File.ReadAllText(filePath);

            // Assert
            Assert.AreEqual(FILE_CONTENT, result);
        }

        [DataTestMethod]
        [DataRow(FILE_NAME, null, DisplayName = "Set file stream without path")]
        [DataRow(FILE_NAME, new object[] { new[] { SOURCE_DIRECTORY } }, DisplayName = "Set file stream with path")]
        public void SetFileStreamTest(string fileName, string[] path)
        {
            // Arrange
            string filePath = _fileUtilities.GetPath("SourceFile.txt", path);

            File.WriteAllText(filePath, FILE_CONTENT, Encoding.Unicode);

            var file = new FileInfo(filePath);

            // Act
            using (FileStream streamContent = file.OpenRead())
            {
                _fileContainer.SetFileStream(fileName, streamContent, path);
            }

            string result = File.ReadAllText(filePath);

            // Assert
            Assert.AreEqual(FILE_CONTENT, result);
        }

        #endregion
    }
}