using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_appliaction
{
    internal class Item
    {
        public Item() { }
        public int ID { get; set; }
        public string Item_Name { get; set; }
        public string Item_Amount { get; set; }
        public string Item_Price { get; set; }
        public string Item_cost { get; set; }
        public string Item_description {  get; set; }

    }
}
