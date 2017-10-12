namespace GitHubUpForGrabs
{
    public class GitHubUpForGrabsOptions
    {
        public GithubOptions GitHub { get; set; }
    }

    public class GithubOptions
    {
        public string PersonalAccessTokens { get; set; }
        public Repository[] Repositories { get; set; }
    }

    public class Repository
    {
        public string Owner { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
    }
}

