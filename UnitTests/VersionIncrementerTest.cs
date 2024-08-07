using System.IO;
using System.Text;
using Xunit.Sdk;

namespace UnitTests
{
    public class VersionIncrementerTest
    {
        private readonly string targetFile = "VersionIncrementer_TestFile.txt";
        private readonly string initialVersion = "1.0.101.202";
        private readonly string expectedBugFixVersion = "1.0.101.203";
        private readonly string expectedFeatureVersion = "1.0.102.0";

        [Fact]
        public void VersionIncrementer_IncrementsBugFixVersion()
        {
            using (FileStream fs = File.Create(targetFile))
            {
                byte[] text = new UTF8Encoding(true).GetBytes(initialVersion);
                fs.Write(text, 0, text.Length);
            }

            Program.ReleaseType releaseType = Program.ReleaseType.BugFix;
            string[] versions = VersionIncrementer.getOriginalAndIncrementedVersions(releaseType, targetFile);

            Assert.Equal(initialVersion, versions[0]);
            Assert.Equal(expectedBugFixVersion, versions[1]);
        }

        [Fact]
        public void VersionIncrementer_IncrementsFeatureVersion()
        {
            using (FileStream fs = File.Create(targetFile))
            {
                byte[] text = new UTF8Encoding(true).GetBytes(initialVersion);
                fs.Write(text, 0, text.Length);
            }

            Program.ReleaseType releaseType = Program.ReleaseType.Feature;
            string[] versions = VersionIncrementer.getOriginalAndIncrementedVersions(releaseType, targetFile);

            Assert.Equal(initialVersion, versions[0]);
            Assert.Equal(expectedFeatureVersion, versions[1]);
        }

        [Fact]
        public void VersionIncrementer_DoesNotAlterRestOfFile()
        {
            string preVersionText = "Some filler content that should remain unchanged\n";
            string postVersionText = "\nPlus some other content that should not change";
            string content = string.Format("{0}{1}{2}", preVersionText, initialVersion, postVersionText);

            using (FileStream fs = File.Create(targetFile))
            {
                byte[] text = new UTF8Encoding(true).GetBytes(content);
                fs.Write(text, 0, text.Length);
            }

            Program.ReleaseType releaseType = Program.ReleaseType.BugFix;
            string[] versions = VersionIncrementer.getOriginalAndIncrementedVersions(releaseType, targetFile);

            content = File.ReadAllText(targetFile);
            content = content.Replace(initialVersion, versions[1]);

            Assert.Equal(string.Format("{0}{1}{2}", preVersionText, versions[1], postVersionText), content);
        }
    }
}
