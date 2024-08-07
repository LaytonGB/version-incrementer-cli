using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using version_increment_cli;

class Program
{
    public enum ReleaseType
    {
        None,
        Feature,
        BugFix,
    }

    static void Main(string[] args)
    {
        ArgsParser parser = new ArgsParser(args);
        string[] versions = VersionIncrementer.getOriginalAndIncrementedVersions(parser.releaseType, parser.targetFile);
        writeNumberBackIntoFile(parser.targetFile, versions[0], versions[1]);

        Console.WriteLine("Version number successfully incremented: {0} -> {1}", versions[0], versions[1]);
    }

    static void writeNumberBackIntoFile(string filePath, string oldNumber, string newNumber)
    {
        string text = File.ReadAllText(@filePath);

        // TODO: Do not replace versions with leading or trailing characters
        text = text.Replace(oldNumber, newNumber);

        File.WriteAllText(@filePath, text);
    }
}