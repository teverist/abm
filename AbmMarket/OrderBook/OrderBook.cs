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

        private readonly SortedDictionary<decimal, SortedSet<Order>> _bids = new(Comparer<decimal>.Create((a,b) => b.CompareTo(a)));
        private readonly SortedDictionary<decimal, SortedSet<Order>> _asks = new();


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

        private static void MatchOrder(Order incoming, SortedDictionary<decimal, SortedSet<Order>> oppositeBook, SortedDictionary<decimal, SortedSet<Order>> sameSideBook, Func<Order, Order, bool> matchCondition)
        {
            // While our incoming order still has quantity to match
            // and the opposite book still has orders to match with
            while (incoming.Quantity > 0 && TryGetBestOrder(oppositeBook, out var priceLevel, out var bestOrder))
            {
                if (bestOrder == null || !matchCondition(incoming, bestOrder))
                {
                    break;
                }

                int tradedQty = Math.Min(incoming.Quantity, bestOrder.Quantity);

                incoming.Quantity -= tradedQty;
                bestOrder.Quantity -= tradedQty;

                if (bestOrder.Quantity == 0)
                {
                    oppositeBook[priceLevel].Remove(bestOrder);
                }

                if (oppositeBook.TryGetValue(priceLevel, out SortedSet<Order>? value) && value.Count == 0)
                {
                    oppositeBook.Remove(priceLevel);
                }
            }

            if (incoming.Quantity > 0) 
            {
                AddToBook(sameSideBook, incoming);
            }
        }

        private static void AddToBook(SortedDictionary<decimal, SortedSet<Order>> book, Order order)
        {
            if (!book.TryGetValue(order.Price, out SortedSet<Order>? value))
            {
                value = [];
                book[order.Price] = value;
            }

            value.Add(order);
        }

        private static bool TryGetBestOrder(
            SortedDictionary<decimal, SortedSet<Order>> book,
            out decimal priceLevel,
            out Order? bestOrder)
        {
            bestOrder = null;
            priceLevel = default;

            while (book.Count > 0)
            {
                var topLevel = book.First();

                if (topLevel.Value == null || topLevel.Value.Count == 0)
                {
                    // Defensive cleanup of empty price level
                    book.Remove(topLevel.Key);
                    continue;
                }

                var candidate = topLevel.Value.Min;
                if (candidate == null)
                {
                    book.Remove(topLevel.Key);
                    continue;
                }

                priceLevel = topLevel.Key;
                bestOrder = candidate;
                return true;
            }

            return false;
        }

    }
}
