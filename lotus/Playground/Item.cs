using System;
using System.Collections.Generic;
using System.Text;

namespace Playground
{
    public class Item
    {
        private Item() { }
        public Item( string name) 
        {
            Name = name;
            Children = new List<Item>();
        }
        public string Name { get; set; }
        public List<Item> Children { get; set; }
    }
}
