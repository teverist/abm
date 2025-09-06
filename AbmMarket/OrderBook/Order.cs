using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbmMarket.OrderBook
{
    /// <summary>
    /// Represents an Order in the OrderBook
    /// </summary>
    public class Order
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public OrderType OrderType { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
