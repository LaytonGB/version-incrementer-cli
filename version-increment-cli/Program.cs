using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

class Program
{
    enum ReleaseType
    {
        None,
        Feature,
        BugFix,
    }

    static void Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        Enum.TryParse(config["release-type"], out ReleaseType releaseType);
        string? targetFile = config["target"];

        if (targetFile == null)
        {
            throw new ArgumentException("No target file provided. Program version unchanged. Please provide a target file using the \"--target\" argument.");
        }

        if (releaseType == ReleaseType.None)
        {
            throw new ArgumentException("No release type provided. Program version unchanged. Please provide a release type of either \"Feature\" or \"BugFix\" using the \"--release-type\" argument.");
        }

        string? fullNumber = getVersionNumber(targetFile);
        if (fullNumber == null)
        {
            return;
        }

        int number = getVersionPart(fullNumber, releaseType);
        string? outNumber = collectVersionNumber(number + 1, fullNumber, releaseType);
        if (outNumber == null)
        {
            throw new Exception("Failed to increment version number.");
        }

        writeNumberBackIntoFile(targetFile, fullNumber, outNumber);

        Console.WriteLine("Version number successfully incremented: {0} -> {1}", fullNumber, outNumber);
    }

    static string? getVersionNumber(string targetFile)
    {
        try
        {
            string text = File.ReadAllText(@targetFile);

            Regex versionRegex = new Regex("(?<=\\s|\"|^)(1\\.0\\.\\d+\\.\\d+)(?=\\s|\"|$)", RegexOptions.Multiline);
            Match match = versionRegex.Match(text);
            string? newestVersion = null;
            while (match.Success)
            {
                string version = match.Groups[1].Value;
                if (newestVersion == null)
                {
                    newestVersion = version;
                }
                else if (isNewerVersion(version, newestVersion))
                {
                    newestVersion = version;
                }

                match = match.NextMatch();
            }

            if (newestVersion == null)
            {
                throw new VersionNotFoundException("Could not find a version number in the file at the path provided.");
            }

            return newestVersion;
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        return null;
    }

    static bool isNewerVersion(string version, string newestVersion)
    {
        string[] contenderParts = version.Split('.');
        string[] newestParts = newestVersion.Split('.');

        // Start from index 2 because versions always start with "1.0."
        for (int i = 2; i < newestParts.Length; i++)
        {
            int contenderPart = int.Parse(contenderParts[i]);
            int newestPart = int.Parse(newestParts[i]);

            if (contenderPart > newestPart)
            {
                return true;
            }
        }

        return false;
    }

    static int getVersionPart(string version, ReleaseType releaseType)
    {
        string[] versionParts = version.Split('.');
        if (releaseType == ReleaseType.BugFix)
        {
            return int.Parse(versionParts[3]);
        }
        else
        {
            return int.Parse(versionParts[2]);
        }
    }

    static string? collectVersionNumber(int newVersionPart, string oldVersion, ReleaseType releaseType)
    {
        string[] parts = oldVersion.Split('.');

        if (releaseType == ReleaseType.BugFix)
        {
            parts[3] = newVersionPart.ToString();
        }
        else
        {
            parts[2] = newVersionPart.ToString();
            parts[3] = "0";
        }

        return String.Join(".", parts);
    }

    static void writeNumberBackIntoFile(string filePath, string oldNumber, string newNumber)
    {
        string text = File.ReadAllText(@filePath);

        // TODO: Do not replace versions with leading or trailing characters
        text = text.Replace(oldNumber, newNumber);

        File.WriteAllText(@filePath, text);
    }
}