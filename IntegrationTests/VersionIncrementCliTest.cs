using System.Diagnostics;
using System.Text;

namespace IntegrationTests
{
    public class VersionIncrementCliTest
    {
        private readonly string projectRoot = "C:/Users/layto/Documents/Coding/cs/version-increment-cli";
        private readonly string targetFile = "VersionIncrementCli_TestFile.txt";
        private readonly string initialVersion = "1.0.101.202";
        private readonly string expectedBugFixVersion = "1.0.101.203";
        private readonly string expectedFeatureVersion = "1.0.102.0";

        [Fact]
        public void VersionIncrementCli_IncrementsBugFixVersion()
        {
            using (FileStream fs = File.Create(targetFile))
            {
                byte[] text = new UTF8Encoding(true).GetBytes(initialVersion);
                fs.Write(text, 0, text.Length);
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {projectRoot}/version-increment-cli --release-type BugFix --target {targetFile}",
                RedirectStandardError = true,
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.True(process.ExitCode == 0, $"ERROR: {error}");

            string content = File.ReadAllText(targetFile);
            Assert.Equal(expectedBugFixVersion, content);
        }

        [Fact]
        public void VersionIncrementCli_IncrementsFeatureVersion()
        {
            using (FileStream fs = File.Create(targetFile))
            {
                byte[] text = new UTF8Encoding(true).GetBytes(initialVersion);
                fs.Write(text, 0, text.Length);
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {projectRoot}/version-increment-cli --release-type Feature --target {targetFile}",
                RedirectStandardError = true,
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.True(process.ExitCode == 0, $"ERROR: {error}");

            string content = File.ReadAllText(targetFile);
            Assert.Equal(expectedFeatureVersion, content);
        }

        [Fact]
        public void VersionIncrementCli_DoesNotAlterRestOfFile()
        {
            string preVersionText = "Some filler content that should remain unchanged\n";
            string postVersionText = "\nPlus some other content that should not change";
            string content = $"{preVersionText}{initialVersion}{postVersionText}";

            using (FileStream fs = File.Create(targetFile))
            {
                byte[] text = new UTF8Encoding(true).GetBytes(content);
                fs.Write(text, 0, text.Length);
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {projectRoot}/version-increment-cli --release-type BugFix --target {targetFile}",
                RedirectStandardError = true,
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            Assert.True(process.ExitCode == 0, $"ERROR: {error}");

            content = File.ReadAllText(targetFile);
            Assert.Equal($"{preVersionText}{expectedBugFixVersion}{postVersionText}", content);
        }
    }
}