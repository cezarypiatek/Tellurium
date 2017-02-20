using System;
using System.IO;

namespace Tellurium.MvcPages.Reports.ErrorReport
{
    internal class TeamCityAdapter:ICIAdapter
    {
        private readonly Action<string> writeOutput;
        private const string TeamcityProjectName = "TEAMCITY_PROJECT_NAME";
        private const string TeamcityBuilNumber = "BUILD_NUMBER";
        private const string TeamcityConfigurationFile = "TEAMCITY_BUILD_PROPERTIES_FILE";
        private static readonly Lazy<string> BuildId = new Lazy<string>(GetBuildExtId);
        private static string BuildNumber => GetTeamcityVariable(TeamcityBuilNumber);

        public TeamCityAdapter(Action<string> writeOutput)
        {
            this.writeOutput = writeOutput;
        }

        public bool IsAvailable()
        {
            return GetTeamcityVariable(TeamcityProjectName) != null;
        }

        public void SetEnvironmentVariable(string name, string value)
        {
            writeOutput($"##teamcity[setParameter name='env.{name}' value='{value}']");
        }

        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }

        public string UploadFileAsArtifact(string filePath)
        {
            writeOutput($"##teamcity[publishArtifacts '{filePath}']");
            var fileName = Path.GetFileName(filePath);
            return GetArtifactPath(fileName);
        }

        private static string GetBuildExtId()
        {
            return GetBuildProperty("teamcity.buildType.id");
        }

        private static string GetTeamcityVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName);
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

        private static string GetArtifactPath(string fileName)
        {
            return $"/repository/download/{BuildId.Value}/{BuildNumber}/{fileName}";
        }
    }
}