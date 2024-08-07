namespace UnitTests
{
    public class ArgsParserTest
    {
        [Fact]
        public void ArgsParser_GetsCorrectArgs()
        {
            string[] args = ["--release-type", "BugFix", "--target", "\"C:\\Users\\layto\\Documents\\Coding\\cs\\version-increment-cli\\version-increment-cli\\TestFile.cs\""];
            ArgsParser parser = new ArgsParser(args);
            Assert.Equal(Program.ReleaseType.BugFix, parser.releaseType);
            Assert.Equal("\"C:\\Users\\layto\\Documents\\Coding\\cs\\version-increment-cli\\version-increment-cli\\TestFile.cs\"", parser.targetFile);
        }

        [Fact]
        public void ArgsParser_ThrowsErrorIfRequiredVariableInvalid()
        {
            string[] args = ["--release-type", "InvalidType", "--target", "\"C:\\Users\\layto\\Documents\\Coding\\cs\\version-increment-cli\\version-increment-cli\\TestFile.cs\""];
            Assert.Throws<ArgumentException>(() => new ArgsParser(args));

            args = ["--release-type", "BugFix", "--target"];
            Assert.Throws<ArgumentException>( () => new ArgsParser(args));
        }

        [Fact]
        public void ArgsParser_IgnoresAdditionalArguments()
        {
            string[] args = ["--release-type", "BugFix", "--target", "\"C:\\Users\\layto\\Documents\\Coding\\cs\\version-increment-cli\\version-increment-cli\\TestFile.cs\"", "--extra-argument", "Hopefully this is ignored."];
            ArgsParser parser = new ArgsParser(args); // does not throw an exception
        }
    }
}