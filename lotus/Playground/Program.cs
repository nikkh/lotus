using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
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

            Dictionary<string, string> nodeDataArray = new Dictionary<string, string>();


            for (var i = 0; i < names.Length; i++)
            {
                nodeDataArray.Add(i.ToString(), names[i]);
            }

            Dictionary<string, Link> linkDataArray = new Dictionary<string, Link>();
            Random r = new Random();

            var num = nodeDataArray.Count;
            for (var i = 0; i < num * 2; i++)
            {
                var a = Math.Floor(r.NextDouble() * num);
                var b = Math.Floor(r.NextDouble() * num / 4) + 1;
                Link l = new Link();
                l.From = a;
                l.To = (a + b) % num;
                linkDataArray.Add(i.ToString(), l);
                
            }

            Console.WriteLine("Hello World!");

            Item p = new Item("Playground Equipment");
            Item swing = new Item("Swing");
            Item parents = new Item("Parents");
            Item slide = new Item("Slide");
            Item teeterTotter = new Item("Teeter Totter");
            Item tireSwing = new Item("Tyre Swing");
            Item jungleGym = new Item("Jungle Gym");
            Item merryGoRound = new Item("Merry-Go-Round");
            Item springHorse = new Item("Spring Horse");
            p.Children.Add(swing);
            p.Children.Add(parents);
            p.Children.Add(slide);
            p.Children.Add(teeterTotter);
            p.Children.Add(tireSwing);
            p.Children.Add(jungleGym);
            p.Children.Add(merryGoRound);
            p.Children.Add(springHorse);

            var test = JsonConvert.SerializeObject(p);

            Console.WriteLine($"{test}");

        }
    }

   
}

