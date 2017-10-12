using CsvHelper;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GitHubUpForGrabs
{
    class Program
    {
        static GitHubUpForGrabsOptions options = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().Get<GitHubUpForGrabsOptions>();

        static void Main(string[] args)
        {
            CallGitHub().GetAwaiter().GetResult();

            Console.WriteLine("OK");
            Console.ReadLine();
        }

        private static async Task CallGitHub()
        {
            GitHubClient client = CreateGitHubClient();

            foreach (var repository in options.GitHub.Repositories)
            {
                RepositoryIssueRequest issueRequest = GenerateLabelsFilter(repository.Label);
                var issues = await client.Issue.GetAllForRepository(repository.Owner, repository.Name, issueRequest);

                Console.WriteLine($"{repository.Owner}/{repository.Name} total: {issues.Count}");

                using (TextWriter textWriter = new StreamWriter($"{repository.Owner}_{repository.Name}.csv"))
                {
                    var csv = new CsvWriter(textWriter);
                    csv.WriteRecords(issues);
                }
            }
        }

        private static GitHubClient CreateGitHubClient()
        {
            var client = new GitHubClient(new ProductHeaderValue("GitHubUpForGrabs"));
            client.Credentials = new Credentials(options.GitHub.PersonalAccessTokens);
            return client;
        }

        private static RepositoryIssueRequest GenerateLabelsFilter(string label)
        {
            var issueRequest = new RepositoryIssueRequest();
            issueRequest.Labels.Add(label);

            return issueRequest;
        }
    }
}
