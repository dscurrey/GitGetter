using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GitGetter.Services
{
    public class GitRepoService : IGitRepoService
    {
        private readonly ILogger<GitRepoService> _logger;
        private readonly IConfiguration _config;

        public GitRepoService(ILogger<GitRepoService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public void Run()
        {
            _logger.LogDebug("GitRepo service started");
            if (!SetupFilepath())
            {
                _logger.LogCritical
                (
                    "An error occurred setting up the destination folder: {Destination}, exiting",
                    _config.GetValue<string>("SaveDirectory")
                );
                return;
            }

            var repos = GetRepos();
            if (repos is not {Count: > 0})
            {
                _logger.LogWarning("No repos found, exiting");
                return;
            }

            CloneRepos(repos);
        }

        private bool SetupFilepath()
        {
            var filepath = _config.GetValue<string>("SaveDirectory");
            return Directory.Exists(filepath) || CreateDirectory(filepath);
        }

        private bool CreateDirectory(string path)
        {
            if (Directory.Exists(path) || !_config.GetValue<bool>("CreateIfNotExists")) return false;
            _logger.LogInformation("Creating directory: {Dir}", path);
            Directory.CreateDirectory(path);
            return true;
        }

        private List<GitRepo> GetRepos()
        {
            List<GitRepo> gitRepos = new List<GitRepo>();
            var filename = _config.GetValue<string>("RepoListFilename");
            _logger.LogDebug("Reading from file: {File}", filename);
            StreamReader file = new StreamReader(@filename);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                if (CheckUrlValid(line))
                {
                    var gitRepo = new GitRepo
                    {
                        Url = line
                    };
                    gitRepos.Add(gitRepo);
                }
                else
                {
                    _logger.LogWarning("Repo URL: {Url} is invalid", line);
                }
            }

            return gitRepos;
        }

        private void CloneRepos(List<GitRepo> gitRepos)
        {
            var destination = _config.GetValue<string>("SaveDirectory");
            foreach (var repo in gitRepos)
            {
                var regex = new Regex(@"[^\/]+[A-Za-z]*[.]git$");
                var match = regex.Match(repo.Url);
                if (!match.Success)
                    return;

                _logger.LogInformation("Repo: {Repo} found", match.Value);
                var dest = destination + "\\" + match.Value;
                if (!Directory.Exists(dest))
                {
                    _logger.LogInformation("Cloning {Repo} to {Path}", match.Value, destination);
                    var repoPath = LibGit2Sharp.Repository.Clone(repo.Url, @dest);
                    repo.IsSaved = true;
                }
                else
                {
                    _logger.LogWarning("Repository already exists at: {Path}, not cloned", dest);
                }
            }
        }

        private static bool CheckUrlValid(string source)
        {
            return Uri.TryCreate
                       (source, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}