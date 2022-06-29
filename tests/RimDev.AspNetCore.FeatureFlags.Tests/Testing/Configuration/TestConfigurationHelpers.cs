using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.Configuration
{
    public static class TestConfigurationHelpers
    {
        public const string AppSettingsJsonFileName = "appsettings";
        public const string TestSettingsJsonFileName = "testsettings";

        public static string TestSettingsFileName(string environmentName = null)
        {
            if (string.IsNullOrEmpty(environmentName)) return $"{TestSettingsJsonFileName}.json";
            return $"{TestSettingsJsonFileName}.{environmentName}.json";
        }

        public static string TestSettingsFilePath(
            string baseDirectory,
            string environmentName = null
        )
        {
            if (string.IsNullOrEmpty(baseDirectory)) throw new ArgumentNullException(nameof(baseDirectory));
            var testSettingsEnvironmentFileName = TestSettingsFileName(environmentName);
            return Path.Combine(baseDirectory, testSettingsEnvironmentFileName);
        }

        public static bool IsRunningOnAppVeyor() =>
            // https://www.appveyor.com/docs/environment-variables/
            bool.TryParse(
                Environment.GetEnvironmentVariable("APPVEYOR"),
                out var parsedAppVeyorVariable
            ) && parsedAppVeyorVariable;

        private const string RimDevTestsSectionName = "RimDevTests";

        /// <summary>
        /// <para>Returns a <see cref="GetRimDevTestsConfiguration"/> instance that is built up by
        /// looking at appsettings/testsettings JSON files in the output directory.</para>
        /// <para>It is recommended that you keep your test configuration in testsettings.json
        /// (or testsettings.Development.json or testsettings.AppVeyor.json) instead of putting it
        /// into the appsettings(.ENVIRONMENT).json file(s).  That avoids any weirdness where your
        /// appsettings.json under "project.Tests" overwrites the appsettings.json under "project".</para>
        /// </summary>
        public static RimDevTestsConfiguration GetRimDevTestsConfiguration(
            string baseDirectory = null
            )
        {
            Console.WriteLine("GetRimDevTestsConfiguration:");
            if (string.IsNullOrEmpty(baseDirectory)) baseDirectory = AppContext.BaseDirectory;
            Console.WriteLine($"  baseDirectory: {baseDirectory}");
            var testEnvironmentName = GetRimDevTestEnvironmentName();
            Console.WriteLine($"  testEnvironmentName: {testEnvironmentName}");

            var appSettingsFileName = $"{AppSettingsJsonFileName}.json";
            var appSettingsFilePath = Path.Combine(baseDirectory, appSettingsFileName);
            Console.WriteLine($"  Loading ({File.Exists(appSettingsFilePath)}): {appSettingsFileName}");

            var appSettingsEnvironmentFileName = $"{AppSettingsJsonFileName}.{testEnvironmentName}.json";
            var appSettingsEnvironmentFilePath = Path.Combine(baseDirectory, appSettingsEnvironmentFileName);
            Console.WriteLine($"  Loading ({File.Exists(appSettingsEnvironmentFilePath)}): {appSettingsEnvironmentFileName}");

            var testSettingsFileName = TestSettingsFileName();
            var testSettingsFilePath = TestSettingsFilePath(baseDirectory);
            Console.WriteLine($"  Loading ({File.Exists(testSettingsFilePath)}): {testSettingsFileName}");

            var testSettingsEnvironmentFileName = TestSettingsFileName(testEnvironmentName);
            var testSettingsEnvironmentFilePath = TestSettingsFilePath(baseDirectory, testEnvironmentName);
            Console.WriteLine($"  Loading ({File.Exists(testSettingsEnvironmentFilePath)}): {testSettingsEnvironmentFileName}");

            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)
                // support the old approach where we recommended appsettings(.ENV).json
                .AddJsonFile($"appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{testEnvironmentName}.json", optional: true)
                // this is the better approach, so allow this to override the old approach
                .AddJsonFile(testSettingsFilePath, optional: true)
                .AddJsonFile(testSettingsEnvironmentFilePath, optional: true)
                .AddEnvironmentVariables()
                .Build();

            var configuration = new RimDevTestsConfiguration();
            configurationRoot
                .GetSection(RimDevTestsSectionName)
                .Bind(configuration);

            Console.WriteLine("  Finished GetRimDevTestsConfiguration()");
            return configuration;
        }

        /// <summary><para>Figure out which additional environment-specific JSON configuration files
        /// to load over top of the base file settings.</para>
        /// <para>Returns "AppVeyor" if running in CI/CD, otherwise it looks at RIMDEVTEST_ENVIRONMENT,
        /// with a fallback to "Development".</para>
        /// <para>We specifically do *not* look at the DOTNET_ENVIRONMENT variable or the ASPNETCORE_ENVIRONMENT
        /// variable because the environment needed to setup your test harness can differ from
        /// how the application needs to be configured for tests.</para>
        /// </summary>
        public static string GetRimDevTestEnvironmentName()
        {
            if (IsRunningOnAppVeyor()) return "AppVeyor";

            var rimDevTestEnvironment = Environment.GetEnvironmentVariable("RIMDEVTEST_ENVIRONMENT");
            return string.IsNullOrWhiteSpace(rimDevTestEnvironment)
                ? Environments.Development
                : rimDevTestEnvironment;
        }
    }
}
