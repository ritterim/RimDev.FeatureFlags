using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using RimDev.AspNetCore.FeatureFlags.Tests.Testing.Configuration;

namespace RimDev.AspNetCore.FeatureFlags.Tests.Testing.Database
{
    /// <summary>Base class for any System.Data.SqlClient database fixture.  This fixture only
    /// takes care of database creation/disposal and not initialization (migrations, etc.).
    /// If a connection string is not provided, it will make use of <see cref="TestConfigurationHelpers"/>
    /// methods to figure it out.</summary>
    public abstract class TestSqlClientDatabaseFixture : IDisposable
    {
        private readonly bool deleteBefore;
        private readonly bool deleteAfter;

        /// <summary>The PRNG is (somewhat) expensive to created and the default seed is the current
        /// instant.  So it's best to create one and only one instance of the Random class.</summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// <para>In order for the fixture to create other databases, it must be able to connect
        /// to an existing database within the SQL server.  By custom, this is usually the 'master'
        /// database.  The connection string is then mutated to replace 'master' with the
        /// dbName parameter.</para>
        /// </summary>
        /// <param name="masterConnectionString">SQL server connection string to 'master' database.</param>
        /// <param name="dbName">Optional database name to be created and used for tests.  If a database
        /// name is not provided, it will be randomly generated and it is strongly recommended that
        /// the deleteBefore and deleteAfter arguments be set to true.</param>
        /// <param name="deleteBefore">Whether the database should be dropped prior to tests.</param>
        /// <param name="deleteAfter">Whether the database should be dropped after tests.</param>
        /// <param name="dbNameSuffix">Optional suffix to be tacked onto the randomly generated database
        /// name.  This can be used in multi-database situations to keep track of which is which.</param>
        /// <exception cref="ArgumentNullException"></exception>
        protected TestSqlClientDatabaseFixture(
            string masterConnectionString = null,
            string dbName = null,
            bool deleteBefore = true,
            bool deleteAfter = true,
            string dbNameSuffix = null
            )
        {
            Console.WriteLine($"Creating {nameof(TestSqlClientDatabaseFixture)} instance...");

            if (string.IsNullOrEmpty(masterConnectionString))
            {
                var testConfiguration = TestConfigurationHelpers.GetRimDevTestsConfiguration();
                masterConnectionString = testConfiguration?.Sql?.MasterConnectionString;
            }

            if (string.IsNullOrEmpty(dbName))
            {
                var now = DateTimeOffset.UtcNow;
                dbName = $"test-{now:yyyyMMdd}-{now:HHmm}-{UpperCaseAlphanumeric(6)}";
            }

            if (!string.IsNullOrEmpty(dbNameSuffix))
            {
                dbName = $"{dbName}-{dbNameSuffix}";
            }

            MasterConnectionString = masterConnectionString;
            DbName = dbName;
            ConnectionString = SwitchMasterToDbNameInConnectionString(masterConnectionString, dbName);
            this.deleteBefore = deleteBefore;
            this.deleteAfter = deleteAfter;

            Console.WriteLine(MasterConnectionStringDebug);
            Console.WriteLine(ConnectionStringDebug);
            RecreateDatabase();

            // Databases do not always wake up right away after the CREATE DATABASE call
            WaitUntilDatabaseIsHealthy();

            Console.WriteLine($"{nameof(TestSqlClientDatabaseFixture)} instance created.");
        }

        /// <summary>Test fixtures need to point at the "master" database initially, because the database
        /// to be used in the tests may not exist yet.</summary>
        private string MasterConnectionString { get; }

        /// <summary>Returns a parsed connection string, listing key details about it.
        /// This debug string will not output the database password, which makes it safe-ish
        /// for output in console logs / build logs.</summary>
        public string MasterConnectionStringDebug
        {
            get
            {
                try
                {
                    var b = new SqlConnectionStringBuilder(MasterConnectionString);
                    return $"MASTER-DB: DataSource={b.DataSource}, InitialCatalog={b.InitialCatalog}, UserID={b.UserID}";
                }
                catch
                {
                    return "MASTER-DB: Bad connection string, unable to parse!";
                }
            }
        }

        /// <summary>The temporary database name that was created by the fixture.</summary>
        public string DbName { get; }

        /// <summary>The connection string to get to the database which will be used for tests.</summary>
        public string ConnectionString { get; }

        /// <summary>Returns a parsed connection string, listing key details about it.
        /// This debug string will not output the database password, which makes it safe-ish
        /// for output in console logs / build logs.</summary>
        public string ConnectionStringDebug
        {
            get
            {
                try
                {
                    var b = new SqlConnectionStringBuilder(ConnectionString);
                    return $"TEST-DB: DataSource={b.DataSource}, InitialCatalog={b.InitialCatalog}, UserID={b.UserID}";
                }
                catch
                {
                    return "TEST-DB: Bad connection string, unable to parse!";
                }
            }
        }

        /// <summary>Returns a SqlConnection object to the test database for the fixture.
        /// Since the SqlConnection implements IDisposable, it should be paired with "using".
        /// </summary>
        public SqlConnection CreateSqlConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>Convert from the "master" database SQL connection string over to the specific database name
        /// that is being used for a particular database fixture.  This relies on the connectionString being
        /// something that can be parsed by System.Data.SqlClient.</summary>
        private string SwitchMasterToDbNameInConnectionString(string connectionString, string dbName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (string.IsNullOrWhiteSpace(dbName))
                throw new ArgumentNullException(nameof(dbName));

            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = dbName
            };

            return builder.ConnectionString;
        }

        private static string UpperCaseAlphanumeric(int size)
        {
            string input = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var chars = Enumerable.Range(0, size)
                .Select(x => input[Random.Next(0, input.Length)]);
            return new string(chars.ToArray());
        }

        /// <summary>Tracks whether the fixture thinks that it has already initialized
        /// (created) the database, or decided that the database already exists.</summary>
        private bool databaseInitialized;

        private readonly object @lock = new object();

        private void RecreateDatabase()
        {
            // https://en.wikipedia.org/wiki/Double-checked_locking#Usage_in_C#
            if (databaseInitialized) return;
            lock (@lock)
            {
                if (databaseInitialized) return;

                if (deleteBefore)
                {
                    try
                    {
                        if (DatabaseExists()) DropDatabase();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"ERROR: Failed to drop database {DbName} before the run!");
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }

                try
                {
                    if (!DatabaseExists()) CreateDatabase();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: Failed to create database {DbName}!");
                    Console.WriteLine(e.Message);
                    throw;
                }

                databaseInitialized = true;
            }
        }

        public void Dispose()
        {
            // https://en.wikipedia.org/wiki/Double-checked_locking#Usage_in_C#
            if (!databaseInitialized) return;
            lock (@lock)
            {
                if (!databaseInitialized) return;

                if (deleteAfter)
                {
                    if (DatabaseExists()) DropDatabase();
                }

                databaseInitialized = false;
            }
        }

        private bool DatabaseExists()
        {
            //TODO: Consider querying the standard SQL metadata tables instead, but this works okay
            using var connection = CreateSqlConnection();
            var command = new SqlCommand("select 1;", connection);
            try
            {
                connection.Open();
                var result = (int)command.ExecuteScalar();
                return (result == 1);
            }
            catch (SqlException)
            {
                return false;
            }
        }

        /// <summary>Is the database accepting connections and will it run queries? </summary>
        private bool DatabaseIsHealthy()
        {
            using var connection = CreateSqlConnection();
            var command = new SqlCommand("select 1;", connection);
            try
            {
                connection.Open();
                var result = (int)command.ExecuteScalar();
                return (result == 1);
            }
            catch (SqlException)
            {
                return false;
            }
        }

        private void CreateDatabase()
        {
            Console.WriteLine($"Creating database '{DbName}'...");

            using (var connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = FormattableString.Invariant(
                $"CREATE DATABASE [{DbName}];");
                command.ExecuteNonQuery();

                Console.WriteLine($"Created database '{DbName}'.");
            }
        }

        private void WaitUntilDatabaseIsHealthy()
        {
            Console.WriteLine($"Begin waiting for '{DbName}' to accept queries.");
            var timer = new Stopwatch();
            timer.Start();
            var attempt = 1;
            while (!DatabaseIsHealthy())
            {
                attempt++;
                var sleepMilliseconds = Math.Min((int) (Math.Pow(1.1, attempt) * 150), 1000);
                Thread.Sleep(sleepMilliseconds);
                if (attempt > 60)
                    throw new Exception(
                        $"Database '{DbName}' refused to execute queries!"
                    );
            }

            timer.Stop();
            Console.WriteLine($"The '{DbName}' is healthy after {timer.ElapsedMilliseconds}ms and {attempt} attempts.");
        }

        private void DropDatabase()
        {
            Console.WriteLine($"Dropping database '{DbName}'...");

            using var connection = new SqlConnection(MasterConnectionString);

            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText = FormattableString.Invariant(
                $"ALTER DATABASE [{DbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DbName}];");

            command.ExecuteNonQuery();

            Console.WriteLine($"Dropped database '{DbName}'.");
        }
    }
}
