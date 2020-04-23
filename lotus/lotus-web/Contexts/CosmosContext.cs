using System;
using System.Collections.Generic;
using System.Text;

namespace lotus_web.Contexts
{
    public class CosmosContext
    {

        public string HostName { get; set; }

        public int Port { get; set; }
        public string AuthKey { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
 
    }
}
