namespace Tellurium.VisualAssertions.Screenshots.Service
{
    public class ContinousIntegration
    {
        public ContinousIntegration(string branchName, string commitHash, string commitTitle)
        {
            BranchName = branchName;
            CommitHash = commitHash;
            CommitTitle = commitTitle;
        }

        public string BranchName { get; }
        public string CommitTitle { get; }
        public string CommitHash { get; }
    }
}