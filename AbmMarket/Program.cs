using AbmMarket.OrderBook;

var book = new OrderBook();

book.AddOrder(new Order { OrderType = OrderType.Buy, Price = 100, Quantity = 10 });
book.AddOrder(new Order { OrderType = OrderType.Sell, Price = 105, Quantity = 5 });
book.AddOrder(new Order { OrderType = OrderType.Sell, Price = 99, Quantity = 7 });

book.PrintBook();