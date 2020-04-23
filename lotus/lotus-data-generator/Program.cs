using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GremlinNetSample
{
    
    class Program
    {
        // Azure Cosmos DB Configuration variables
        // Replace the values in these variables to your own.
        static string hostname = "";
        static int port = 0;
        static string authKey = "";
        static string database = "";
        static string collection = "";

        static void ExecuteCmd(GremlinClient gremlinClient, KeyValuePair<string, string> command)
        {
            try
            {
                var resultSet = SubmitRequest(gremlinClient, command).Result;
                if (resultSet.Count > 0)
                {
                    Console.WriteLine("\tResult:");
                    foreach (var result in resultSet)
                    {
                        
                        string output = JsonConvert.SerializeObject(result);
                        Console.WriteLine($"\t{output}");
                    }
                    Console.WriteLine();
                }

                
                PrintStatusAttributes(resultSet.StatusAttributes);
                Console.WriteLine();
                
            }
            catch (Exception e)
            {
                var currentColour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine($"Exception: {e.Message}");
                if (e.InnerException != null) 
                {
                    
                    Console.WriteLine($"InnerException: {e.InnerException.Message}");
                }
                Console.ForegroundColor = currentColour;
            }
        }

        // Starts a console application that executes every Gremlin query in the gremlinQueries dictionary. 
        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            IConfiguration configuration = configBuilder.Build();
                        
            try
            {
                hostname = configuration["HostName"];
                port = Int32.Parse(configuration["Port"]);
                authKey = configuration["AuthKey"];
                database = configuration["Database"];
                collection = configuration["Collection"];
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception caught while accessing configuration: {e.Message}");
                return;
            }

            string[] names = {
                                 "Joshua", "Daniel", "Robert", "Noah", "Anthony", "Elizabeth", "Addison", "Alexis", "Ella", "Samantha",
                                 "Joseph", "Scott", "James", "Ryan", "Benjamin",
                                 "Walter", "Gabriel", "Christian", "Nathan", "Simon",
                                 "Isabella", "Emma", "Olivia", "Sophia", "Ava",
                                 "Emily", "Madison", "Tina", "Elena", "Mia",
                                 "Jacob", "Ethan", "Michael", "Alexander", "William",
                                 "Natalie", "Grace", "Lily", "Alyssa", "Ashley",
                                 "Sarah", "Taylor", "Hannah", "Brianna", "Hailey",
                                 "Christopher", "Aiden", "Matthew", "David", "Andrew",
                                 "Kaylee", "Juliana", "Leah", "Anna", "Allison",
                                 "John", "Samuel", "Tyler", "Dylan", "Jonathan"
            };

            var gremlinServer = new GremlinServer(hostname, port, enableSsl: true, 
                                                    username: "/dbs/" + database + "/colls/" + collection, 
                                                    password: authKey);

            using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
            {
                

                ExecuteCmd(gremlinClient, new KeyValuePair<string, string> ("Cleanup", "g.V().drop()" ));


                foreach (var name in names)
                {
                    string query = $"g.addV('person').property('id', '{name}').property('firstName', '{name}').property('age', 44)";
                    ExecuteCmd(gremlinClient, new KeyValuePair<string, string>("CreatePerson", query));
                    
                }

                // Now generate some relationships
                Random r = new Random();
                for (int i = 0; i < names.Length * 3; i++)
                {
                    int from = r.Next(0, names.Length);
                    int to = r.Next(0, names.Length);
                    if (from == to) from++;
                    string query = $"g.V('{names[from]}').addE('knows').to(g.V('{names[to]}'))";
                    ExecuteCmd(gremlinClient, new KeyValuePair<string, string>("CreateRelationship", query));
                }
            }

            // Exit program
            Console.WriteLine("Done. Press enter to exit...");
            Console.ReadLine();
        }

        private static Task<ResultSet<dynamic>> SubmitRequest(GremlinClient gremlinClient, KeyValuePair<string, string> query)
        {
            try
            {
                return gremlinClient.SubmitAsync<dynamic>(query.Value);
            }
            catch (ResponseException e)
            {
                Console.WriteLine("\tRequest Error!");

                // Print the Gremlin status code.
                Console.WriteLine($"\tStatusCode: {e.StatusCode}");

                PrintStatusAttributes(e.StatusAttributes);
                Console.WriteLine($"\t[\"x-ms-retry-after-ms\"] : { GetValueAsString(e.StatusAttributes, "x-ms-retry-after-ms")}");
                Console.WriteLine($"\t[\"x-ms-activity-id\"] : { GetValueAsString(e.StatusAttributes, "x-ms-activity-id")}");

                throw;
            }
        }

        private static void PrintStatusAttributes(IReadOnlyDictionary<string, object> attributes)
        {
            Console.WriteLine($"\tStatusAttributes:");
            Console.WriteLine($"\t[\"x-ms-status-code\"] : { GetValueAsString(attributes, "x-ms-status-code")}");
            Console.WriteLine($"\t[\"x-ms-total-request-charge\"] : { GetValueAsString(attributes, "x-ms-total-request-charge")}");
        }

        public static string GetValueAsString(IReadOnlyDictionary<string, object> dictionary, string key)
        {
            return JsonConvert.SerializeObject(GetValueOrDefault(dictionary, key));
        }

        public static object GetValueOrDefault(IReadOnlyDictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return null;
        }
    }
}
