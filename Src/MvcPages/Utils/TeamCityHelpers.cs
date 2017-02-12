using System;
using System.IO;

namespace Tellurium.MvcPages.Utils
{
    internal static class TeamCityHelpers
    {
        private const string TeamcityProjectName = "TEAMCITY_PROJECT_NAME";
        private const string TeamcityBuilNumber = "BUILD_NUMBER";
        private const string TeamcityConfigurationFile = "TEAMCITY_BUILD_PROPERTIES_FILE";
        private static readonly Lazy<string> BuildId = new Lazy<string>(GetBuildExtId);

        public static string GetArtifactPath(string fileName)
        {
            return $"/repository/download/{BuildId.Value}/{BuildNumber}/{fileName}";
        }

        public static bool IsAvailable()
        {
            return GetTeamcityVariable(TeamcityProjectName) != null;
        }

        private static string GetTeamcityVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
        }

        private static string BuildNumber => GetTeamcityVariable(TeamcityBuilNumber);

        private static string GetBuildExtId()
        {
            return GetBuildProperty("teamcity.buildType.id");
        }

        private static string GetBuildProperty(string propertyName)
        {
            var configFilePath = GetTeamcityVariable(TeamcityConfigurationFile);
            foreach (var configLine in File.ReadAllLines(configFilePath))
            {
                if (configLine.StartsWith(propertyName))
                {
                    return configLine.Split('=')[1].Trim();
                }
            }
            return string.Empty;
        }

        public static string UploadFileAsArtifact(string filePath)
        {
            Console.WriteLine($"##teamcity[publishArtifacts '{filePath}']");
            var fileName = Path.GetFileName(filePath);
            return GetArtifactPath(fileName);
        }
    }
}