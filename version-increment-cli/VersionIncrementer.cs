using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Program;

namespace version_increment_cli
{
    internal class VersionIncrementer
    {
        private static readonly string versionPattern = "(?<=\\s|\"|^)(1\\.0\\.\\d+\\.\\d+)(?=\\s|\"|$)";

        private readonly ReleaseType releaseType;
        private readonly string targetFile;

        private string? originalVersion;
        private string? version;
        private string[]? parts;

        public static string[] getOriginalAndIncrementedVersions(ReleaseType releaseType, string targetFile)
        {
            VersionIncrementer incrementer = new VersionIncrementer(releaseType, targetFile);
            incrementer.updateVersion();
            incrementer.updateVersionParts();
            incrementer.incrementVersionPart();
            incrementer.updateVersionFromParts();

            if (incrementer.version == null)
            {
                throw new Exception("Failed to increment version: An unknown error occurred.");
            }

            return [incrementer.originalVersion, incrementer.version];
        }

        private VersionIncrementer(ReleaseType releaseType, string targetFile)
        {
            this.releaseType = releaseType;
            this.targetFile = targetFile;
        }

        private void updateVersion()
        {
            try
            {
                string text = File.ReadAllText(@targetFile);

                Regex versionRegex = new Regex(versionPattern, RegexOptions.Multiline);
                Match match = versionRegex.Match(text);

                if (match.Success)
                {
                    originalVersion = match.Groups[1].Value;
                    version = originalVersion;
                }
                else
                {
                    throw new VersionNotFoundException("Could not find a version number in the file at the path provided.");
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private void updateVersionParts()
        {
            parts = version.Split(".");
        }

        private void incrementVersionPart()
        {
            if (releaseType == ReleaseType.BugFix)
            {
                parts[3] = (int.Parse(parts[3]) + 1).ToString();
            }
            else
            {
                parts[2] = (int.Parse(parts[2]) + 1).ToString();
                parts[3] = "0";
            }
        }

        private void updateVersionFromParts()
        {
            version = String.Join(".", parts);
        }
    }
}
