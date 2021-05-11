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
            // Read from config
            // Check/Create dirs
            // Get/Parse Repo List
            // Cloning
            _logger.LogInformation("Hello world");
        }
    }
}