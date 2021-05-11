using System.Collections.Generic;
using System.IO;
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
            _logger.LogInformation("Hello world");
            
            if (!SetupFilepath())
            {
                _logger.LogCritical("An error occurred setting up the destination folder: {Destination}, exiting", _config.GetValue<string>("SaveDirectory"));
                return;
            }

            var repos = GetRepos();
            if (repos == null || repos.Count <= 0)
            {
                _logger.LogWarning("No repos found, exiting");
                return;
            }
                
            // TODO - Get/Parse Repo List
            // TODO - Clone from list
        }

        private bool SetupFilepath()
        {
            var filepath = _config.GetValue<string>("SaveDirectory");
            return Directory.Exists(filepath) || CreateDirectory(filepath);
        }

        private bool CreateDirectory(string path)
        {
            if (!Directory.Exists(path) && _config.GetValue<bool>("CreateIfNotExists"))
            {
                Directory.CreateDirectory(path);
                return true;
            }

            return false;
        }

        private List<GitRepo> GetRepos()
        {
            return null;
        }
    }
}