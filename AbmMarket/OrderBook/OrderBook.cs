using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbmMarket.OrderBook
{
    public class OrderBook
    {
        public long Id { get; set; }

        private readonly SortedDictionary<decimal, Queue<Order>> _bids = new(Comparer<decimal>.Create((a,b) => b.CompareTo(a)));
        private readonly SortedDictionary<decimal, Queue<Order>> _asks = new();


        /// <summary>
        /// Matches order in the book, if there is any quantity left, then adds to the book.
        /// </summary>
        /// <param name="order">The incoming order to match against</param>
        public void AddOrder(Order order)
        {
            if (order.OrderType == OrderType.Buy)
            {
                MatchOrder(order, _asks, _bids, (buy, sell) => buy.Price >= sell.Price);
            }
            else if (order.OrderType == OrderType.Sell)
            {
                MatchOrder(order, _bids, _asks, (buy, sell) => sell.Price >= buy.Price);
            }
        }

        public void PrintBook()
        {
            Console.WriteLine("Order Book:");
            Console.WriteLine(" Asks:");
            foreach (var level in _asks)
            {
                Console.WriteLine($"  {level.Key} x {level.Value.Sum(o => o.Quantity)}");

            }

            Console.WriteLine(" Bids:");
            foreach (var level in _bids)
            {
                Console.WriteLine($"  {level.Key} x {level.Value.Sum(o => o.Quantity)}");
            }
        }

        private static void MatchOrder(Order incoming, SortedDictionary<decimal, Queue<Order>> oppositeBook, SortedDictionary<decimal, Queue<Order>> sameSideBook, Func<Order, Order, bool> matchCondition)
        {
            // While our incoming order still has quantity to match
            // and the opposite book still has orders to match with
            while (incoming.Quantity > 0 && oppositeBook.Count != 0)
            {
                var topLevel = oppositeBook.First();
                var bestOrder = topLevel.Value.Peek();

                if (!matchCondition(incoming, bestOrder))
                {
                    // No match possible
                    break;
                }

                int tradedQty = Math.Min(incoming.Quantity, bestOrder.Quantity);

                incoming.Quantity -= tradedQty;
                bestOrder.Quantity -= tradedQty;

                if (bestOrder.Quantity == 0)
                {
                    // best order has been filled, remove from the order book
                    topLevel.Value.Dequeue();
                }

                if (topLevel.Value.Count == 0)
                {
                    // Top level is empty, remove
                    oppositeBook.Remove(topLevel.Key);
                }

            }

            if (incoming.Quantity > 0) 
            {
                AddToBook(sameSideBook, incoming);
            }
        }

        private static void AddToBook(SortedDictionary<decimal, Queue<Order>> book, Order order)
        {
            if (!book.TryGetValue(order.Price, out Queue<Order>? value))
            {
                value = new Queue<Order>();
                book[order.Price] = value;
            }

            value.Enqueue(order);
        }
    }
}
