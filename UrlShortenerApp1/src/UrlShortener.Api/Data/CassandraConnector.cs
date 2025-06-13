using Cassandra;
using System;
using System.Threading.Tasks;

namespace UrlShortener.API.Data
{
    public class CassandraConnector
    {
        private Cassandra.ISession? _session;
        private readonly string _contactPoint;
        private readonly int _port;
        private readonly string _keyspace;

        public CassandraConnector(string contactPoint = "cassandra", int port = 9042, string keyspace = "url_shortener")
        {
            _contactPoint = contactPoint;
            _port = port;
            _keyspace = keyspace;
        }

        public async Task<Cassandra.ISession> ConnectAsync()
        {
            if (_session is null) // Fixed CS0019 by using 'is' operator for nullable type comparison
            {
                try
                {
                    var cluster = Cluster.Builder()
                        .AddContactPoint(_contactPoint)
                        .WithPort(_port)
                        .WithSocketOptions(new SocketOptions())
                        .WithLoadBalancingPolicy(new RoundRobinPolicy())
                        .Build();

                    var setupSession = await cluster.ConnectAsync();
                    await setupSession.ExecuteAsync(new SimpleStatement(
                        $"CREATE KEYSPACE IF NOT EXISTS {_keyspace} WITH replication = {{'class': 'SimpleStrategy', 'replication_factor': 1}}"));

                    _session = await cluster.ConnectAsync(_keyspace);

                    await CreateTablesIfNotExistAsync(_session);
                    Console.WriteLine("Connected to Cassandra and ensured schema.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to Cassandra: {ex.Message} - StackTrace: {ex.StackTrace}");
                    throw;
                }
            }

            return _session;
        }

        private static async Task CreateTablesIfNotExistAsync(Cassandra.ISession session)
        {
            // Create 'urls' table
            var createUrlsTable = @"
                    CREATE TABLE IF NOT EXISTS urls (
                        short_code text PRIMARY KEY,
                        original_url text,
                        created_at timestamp,
                        expiration_date timestamp,
                        click_count int,
                        is_active boolean,
                        last_accessed timestamp
                    );";
            await session.ExecuteAsync(new SimpleStatement(createUrlsTable));

            // Create 'url_analytics' table
            var createAnalyticsTable = @"
                    CREATE TABLE IF NOT EXISTS url_analytics (
                        short_code text,
                        click_date timestamp,
                        ip_address text,
                        user_agent text,
                        PRIMARY KEY (short_code, click_date)
                    ) WITH CLUSTERING ORDER BY (click_date DESC);";
            await session.ExecuteAsync(new SimpleStatement(createAnalyticsTable));
        }
    }
}