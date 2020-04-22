using System;
using System.Collections.Generic;
using System.Drawing;

namespace lotus_web
{
    public class LotusNetwork
    {
        private List<NodeData> _nodes = new List<NodeData>();
        private List<LinkData> _links = new List<LinkData>();

        public LotusNetwork() 
        {
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
            Random r = new Random();
            for (var i = 0; i < names.Length; i++)
            {
               
                var colorString = String.Format("#{0:X}{1:X}{2:X}", (byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
                _nodes.Add(new NodeData(i, $"x{names[i]}", colorString));
            }

            var num = Nodes.Length;
            for (var i = 0; i < num * 2; i++)
            {
                var a = Math.Floor(r.NextDouble() * num);
                var b = Math.Floor(r.NextDouble() * num / 4) + 1;
                var colorString = String.Format("#{0:X}{1:X}{2:X}", (byte)r.Next(0, 256), (byte)r.Next(0, 256), (byte)r.Next(0, 256));
                _links.Add(new LinkData(a, (a + b) % num, colorString));
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