using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Gremlin.Net.Structure.IO.GraphSON;
using lotus_web.Contexts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace lotus_web
{
    public class LotusNetwork
    {
        private List<NodeData> _nodes = new List<NodeData>();
        private List<LinkData> _links = new List<LinkData>();

        
        
        private static GremlinServer gremlinServer;

        private LotusNetwork() { }

        public LotusNetwork(CosmosContext context) 
        {

            gremlinServer = new GremlinServer(context.HostName, context.Port, enableSsl: true,
                                                 username: "/dbs/" + context.Database + "/colls/" + context.Collection,
                                                 password: context.AuthKey);
        }

        public async Task FillFromCosmos() 
        {
            Random r = new Random();

            using (var gremlinClient = new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
            {
                KeyValuePair<string, string> query = new KeyValuePair<string, string>("GetVertices", "g.V()");
                var vertices = await SubmitRequest(gremlinClient, query);
                int i = 0;
                foreach (var v in vertices)
                {
                    JObject vertex = JObject.Parse(JsonConvert.SerializeObject(v));
                    string name = vertex["id"].ToString();
                    _nodes.Add(new NodeData(i, $"{name}", RandomColor(r)));
                    i++;
                }

                query = new KeyValuePair<string, string>("GetEdges", "g.E()");
                var edges = await SubmitRequest(gremlinClient, query);
                i = 0;
                foreach (var e in edges)
                {
                    JObject edge = JObject.Parse(JsonConvert.SerializeObject(e));
                    string inV = edge["inV"].ToString();
                    string outV = edge["outV"].ToString();
                    var fromNode = _nodes.Where(n => n.Text == outV).FirstOrDefault();
                    var toNode = _nodes.Where(n => n.Text == inV).FirstOrDefault();
                    _links.Add(new LinkData(fromNode.Key, toNode.Key, RandomColor(r)));
                    i++;
                }

            }
        }

        private string RandomColor(Random r) 
        {
            return String.Format("#{0:X}{1:X}{2:X}", (byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
        }

        

        private Task<ResultSet<dynamic>> SubmitRequest(GremlinClient gremlinClient, KeyValuePair<string, string> query)
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

               

                throw;
            }
        }
        public NodeData[] Nodes { 
            get 
            {
                return _nodes.ToArray();
            } 
        }
        public LinkData[] Links
        {
            get
            {
                return _links.ToArray();
            }
        }
    }

    public class NodeData 
    {
        public NodeData(int key, string text, string colour) 
        {
            Key = key;
            Text = text;
            Colour = colour;
        }
        public int Key { get; set; }
        public string Text { get; set; }
        public string Colour { get; set; }
    }
    public class LinkData 
    {
        public LinkData(double from, double to, string colour)
        {
            From = from;
            To = to;
            Colour = colour;
        }
        public double From { get; set; }
        public double To { get; set; }
        public string Colour { get; set; }
    }
}