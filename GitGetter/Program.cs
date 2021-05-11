using System;
using System.IO;
using GitGetter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GitGetter
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Starting GitGetter...");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices
                (
                    (context, services) =>
                    {
                        // Config Services
                        services.AddTransient<IGitRepoService, GitRepoService>();
                    }
                )
                .UseSerilog()
                .Build();

            var serv = ActivatorUtilities.CreateInstance<GitRepoService>(host.Services);
            serv.Run();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile
                (
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    true, true
                )
                .AddEnvironmentVariables();
        }
    }
}