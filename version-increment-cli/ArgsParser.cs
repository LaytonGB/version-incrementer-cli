using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace version_increment_cli
{
    internal class ArgsParser
    {
        public readonly ReleaseType releaseType;
        public readonly string targetFile;

        public ArgsParser(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            Enum.TryParse(config["release-type"], out releaseType);

            if (config["target"] == null)
            {
                throw new ArgumentException("No target file provided. Program version unchanged. Please provide a target file using the \"--target\" argument.");
            }
            targetFile = config["target"];

            if (releaseType == ReleaseType.None)
            {
                throw new ArgumentException("No release type provided. Program version unchanged. Please provide a release type of either \"Feature\" or \"BugFix\" using the \"--release-type\" argument.");
            }
        }


    }
}
