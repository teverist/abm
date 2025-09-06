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
        public Guid OrderId { get; } = Guid.NewGuid();
        public OrderType OrderType { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public int CompareTo(Order? other)
        {
            if (other == null) 
            { 
                return 1; 
            }
            
            int cmp = Timestamp.CompareTo(other.Timestamp);

            if (cmp == 0)
            {
                // If they have the same timestamp, tiebreaker with their Order ID
                cmp = OrderId.CompareTo(other.OrderId);
            }

            return cmp;
        }
    }
}

