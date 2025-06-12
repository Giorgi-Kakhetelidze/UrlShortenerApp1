using Cassandra;
using System;

namespace UrlShortener.API.Data
{
    public class CassandraConnector
    {
        private static Cassandra.ISession? _session;

        public static Cassandra.ISession Connect()
        {
            if (_session == null)
            {
                try
                {
                    var cluster = Cluster.Builder()
                        .AddContactPoint("cassandra")
                        .WithPort(9042)
                        .WithSocketOptions(new SocketOptions()) 
                        .WithLoadBalancingPolicy(new RoundRobinPolicy())
                        .Build();

                    _session = cluster.Connect("url_shortener");
                    Console.WriteLine("Connected to Cassandra successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to Cassandra: {ex.Message} - StackTrace: {ex.StackTrace}");
                    throw;
                }
            }
            return _session;
        }
    }
}