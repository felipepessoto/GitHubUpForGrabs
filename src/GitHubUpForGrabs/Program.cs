using CsvHelper;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GitHubUpForGrabs
{
    class Program
    {
        private static Tuple<string, string, string>[] repositories = new[] {
            new Tuple<string, string, string>("dotnet", "coreclr", "up-for-grabs"),
            new Tuple<string, string, string>("dotnet", "corefx", "up for grabs"),
            new Tuple<string, string, string>("aspnet", "mvc", "up-for-grabs"),
            new Tuple<string, string, string>("aspnet", "EntityFramework", "up-for-grabs"),
        };

        static void Main(string[] args)
        {
            CallGitHub().GetAwaiter().GetResult();

            Console.WriteLine("OK");
            Console.ReadLine();
        }

        private static async Task CallGitHub()
        {
            GitHubClient client = CreateGitHubClient();            

            List<Issue> allIssues = new List<Issue>();

            foreach (var repository in repositories)
            {
                RepositoryIssueRequest issueRequest = GenerateLabelsFilter(repository.Item3);
                var issues = await client.Issue.GetAllForRepository(repository.Item1, repository.Item2, issueRequest);
                allIssues.AddRange(issues);

                Console.WriteLine($"{repository.Item1}/{repository.Item2} total: {issues.Count}");
                Console.WriteLine();

                foreach (var issue in issues)
                {
                    //Console.WriteLine($"Issue: {issue.Title}, Labels: {string.Join(", ", issue.Labels.Select(x => x.Name))}");
                }
            }

            using (TextWriter textWriter = new StreamWriter("output.csv"))
            {
                var csv = new CsvWriter(textWriter);
                csv.WriteRecords(allIssues);
            }
        }

        private static GitHubClient CreateGitHubClient()
        {
            var client = new GitHubClient(new ProductHeaderValue("GitHubUpForGrabs"));
            client.Credentials = new Credentials("a9a646e25be5d45decb58bd810e8d0aafa0baf37");
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
