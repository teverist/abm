using AbmMarket.OrderBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbmMarket.Test.OrderBookTests
{
    public class OrderBookTests
    {
        [Fact]
        public void BuyOrder_ShouldMatchWithExistingSellOrder()
        {
            var book = new OrderBook.OrderBook();

            var sellOrder = new Order
            {
                OrderType = OrderType.Sell,
                Price = 100,
                Quantity = 5,
                Timestamp = DateTime.UtcNow.AddSeconds(-10) // older order
            };

            var buyOrder = new Order
            {
                OrderType = OrderType.Buy,
                Price = 105,
                Quantity = 5,
                Timestamp = DateTime.UtcNow
            };

            book.AddOrder(sellOrder);
            book.AddOrder(buyOrder);

        }
    }
}
