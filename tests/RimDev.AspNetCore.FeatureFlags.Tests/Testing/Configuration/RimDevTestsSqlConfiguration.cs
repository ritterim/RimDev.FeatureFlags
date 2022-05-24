namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.Configuration
{
    public class RimDevTestsSqlConfiguration
    {
        /// <summary>This is usually "localhost", but in the case of SQL Server Developer 2019
        /// edition on AppVeyor, it would be set as "(local)\SQL2019".</summary>
        public string Hostname { get; set; } = "localhost";

        /// <summary>Port that SQL Server will be listening on.  This is kept separate for ease of
        /// use in the build scripts if Docker SQL is used.  The default assumption is that the
        /// developer is running SQL for Docker on port 11433.</summary>
        public int Port { get; set; } = 11433;

        public string InitialCatalog { get; set; } = "master";
        public string UserId { get; set; } = "sa";

        /// <summary>See: https://github.com/ritterim/hub/blob/master/docker-compose.yml
        /// The assumption is that the developer is running a 'hub' Docker container.</summary>
        public string Password { get; set; } = "Pass123!";

        private bool HostnameOnlyWithoutPort => Hostname?.Contains("\\") == true;

        /// <summary>If the Hostname contains a "\" (backslash), we output only the hostname (without the port)
        /// because that's probably SQL Server Developer that is not running on a specific TCP port.
        /// </summary>
        public string DataSource => HostnameOnlyWithoutPort
            ? $"{Hostname}"
            : $"{Hostname},{Port}";

        public string MasterConnectionString =>
            $@"server={DataSource};database={InitialCatalog};user={UserId};password={Password};";
    }
}
