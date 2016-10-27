using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xtc.Common.Cli.Commands;
using Microsoft.Xtc.TestCloud.ObjectModel;
using Xunit;

namespace Microsoft.Xtc.TestCloud.Tests.Services
{
    public class WorkspaceDirectoryTests: IDisposable
    {
        private const string DependencyJarsDirectory = "dependency-jars";
        private const string TestClassesDirectory = "test-classes";
        private const string ClassFileRelativePath = "test-classes/com/xamarin/TestClass.class";

        private readonly AppiumWorkspace _workspace;

        public WorkspaceDirectoryTests()
        {
            var workspacePath = Path.Combine(Path.GetTempPath(), $"appium_workspace_{Path.GetRandomFileName()}");
            Directory.CreateDirectory(workspacePath);

            _workspace = new AppiumWorkspace(workspacePath);
        }

        public void Dispose()
        {
            Directory.Delete(_workspace.WorkspacePath(), true);
        }

        [Fact]
        public async Task ValidationShouldFailWhenThereIsNoPomFile()
        {
            await CreateDirectoryInWorkspace(DependencyJarsDirectory);
            await CreateDirectoryInWorkspace(TestClassesDirectory);
            await CreateClassFile();

            Assert.Throws<CommandException>(() => _workspace.Validate());
        }

        [Fact]
        public async Task ValidationShouldFailWhenThereIsNoDepedndenciesJarDirectory()
        {
            await CreateDirectoryInWorkspace(TestClassesDirectory);
            CreatePomFile();
            await CreateClassFile();

            Assert.Throws<CommandException>(() => _workspace.Validate());
        }

        [Fact]
        public async Task ValidationShouldFailWhenThereIsNoTestClassesDirectory()
        {
            await CreateDirectoryInWorkspace(DependencyJarsDirectory);
            CreatePomFile();

            Assert.Throws<CommandException>(() => _workspace.Validate());
        }

        [Fact]
        public async Task ValidationShouldFailWhenThereIsNoClassFile()
        {
            await CreateDirectoryInWorkspace(DependencyJarsDirectory);
            await CreateDirectoryInWorkspace(TestClassesDirectory);
            CreatePomFile();

            Assert.Throws<CommandException>(() => _workspace.Validate());
        }

        [Fact]
        public async Task ValidationShouldPassWithAllRequiredFilesAndDirectories()
        {
            await CreateDirectoryInWorkspace(DependencyJarsDirectory);
            await CreateDirectoryInWorkspace(TestClassesDirectory);
            CreatePomFile();
            await CreateClassFile();

            _workspace.Validate();
        }

        private async Task CreateDirectoryInWorkspace(string path)
        {
            var fullPath = Path.Combine(_workspace.WorkspacePath(), path);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);

                while (!Directory.Exists(fullPath))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                }
            }
        }

        private async Task CreateFileInWorkspace(string path, string content)
        {
            await CreateDirectoryInWorkspace(Path.GetDirectoryName(path));
            File.WriteAllText(Path.Combine(_workspace.WorkspacePath(), path), content);
        }

        private void CreatePomFile()
        {
            var testDir = Path.GetDirectoryName(this.GetType().GetTypeInfo().Assembly.Location);
            var sourcePath = Path.Combine(testDir, "Services", "pom.xml");
            var targetPath = Path.Combine(_workspace.WorkspacePath(), "pom.xml");
            File.Copy(sourcePath, targetPath); 
        }

        private async Task CreateClassFile()
        {
            await CreateFileInWorkspace(ClassFileRelativePath, "This pretends to be JVM class file.");
        }
    }
}