using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.Eventing.Reader;
using TraderWebApp.Models;

namespace TraderWebApp.Utils
{
    public class ExecutionPlanUtils
    {
        static List<double> GetBalance(Dictionary<string, List<double>> balances, string name, OrderType orderType)
        {
            if (balances.TryGetValue(name, out List<double> balanceList))
            {
                return balanceList;
            }
            else
            {
                throw new KeyNotFoundException($"No balance found for name: {name}");
            }
        }
        static void UpdateBalance(Dictionary<string, List<double>> balances, string name, OrderType ordertype, double newBalance)
        {
            if (balances.TryGetValue(name, out List<double> balanceList))
            {
                switch (ordertype)
                {
                    case OrderType.Buy:
                        balanceList[0] = newBalance; //  BTC balance is at index 0
                        break;
                    case OrderType.Sell:
                        balanceList[1] = newBalance; //  EUR balance is at index 1
                        break;
                }
            }
            else
            {
                throw new KeyNotFoundException($"No balance found for name: {name}");
            }
        }

        public static List<OrderPlan> GetFinalExecutionPlan(OrderType orderType, List<Order> orders, double remainingAmount)
        {
            if (orders == null || orders.Count == 0)
            {
                throw new Exception("We can't give you a plan.");
            }
            List<OrderPlan> retVal = new List<OrderPlan>();
            if (OrderType.Buy.Equals(orderType))
            {
                orders = orders.OrderBy(x => x.Price).ToList();
            }
            else
            {
                orders = orders.OrderByDescending(x => x.Price).ToList();
            }
            Dictionary<string, List<double>> balances = new Dictionary<string, List<double>>();
            
            for (int i = 0; i < 10; i++)
            {
                List<double> balance = new List<double>() { 10, 5000 };// list btc balance, eur balance
                balances.Add($"ExchangeName_{i + 1}", balance);

            }
            List<double> exchangeBalance;//available balance
            double maxAllowedAmountFromOrder;
            foreach (Order order in orders)
            {
                if (remainingAmount <= 0)
                    break;

                if (order.Amount <= 0) 
                {
                    continue;
                }
                maxAllowedAmountFromOrder = order.Amount;
                
                exchangeBalance = GetBalance(balances, order.Exchange, orderType); // gets balance of the exchange
                //if need less then order has amount
                if (order.Amount > remainingAmount)
                {
                    maxAllowedAmountFromOrder = remainingAmount;
                }
                //buy
                if (OrderType.Buy.Equals(orderType))
                {
                    double balanceRemaining = exchangeBalance[0];
                    if (balanceRemaining.Equals(0))
                    {
                        continue;
                    }
                    if (exchangeBalance[0] < maxAllowedAmountFromOrder)//exchange has less balance as we want to take
                    {
                        maxAllowedAmountFromOrder = exchangeBalance[0];
                    }
                    //add to plan
                    retVal.Add(new OrderPlan
                    {
                        Exchange = order.Exchange,
                        Amount = maxAllowedAmountFromOrder,
                        Price = order.Price
                    });
                    remainingAmount -= maxAllowedAmountFromOrder;
                    balanceRemaining -= maxAllowedAmountFromOrder;
                    UpdateBalance(balances, order.Exchange, orderType, balanceRemaining);
                }
                // sell
                else
                {
                    double balanceRemaining = exchangeBalance[1];
                    if (balanceRemaining.Equals(0))
                    {
                        continue;
                    }
                    if (maxAllowedAmountFromOrder * order.Price > exchangeBalance[1])
                    {
                        maxAllowedAmountFromOrder = exchangeBalance[1] / order.Price;
                    }
                    retVal.Add(new OrderPlan
                    {
                        Exchange = order.Exchange,
                        Amount = maxAllowedAmountFromOrder,
                        Price = order.Price
                    });
                    remainingAmount -= maxAllowedAmountFromOrder;
                    balanceRemaining -= maxAllowedAmountFromOrder * order.Price;
                    UpdateBalance(balances, order.Exchange, orderType, balanceRemaining);
                }
            }
            if (remainingAmount > 0)
            {
                throw new Exception("Based on your requests, we can't provide a plan for the amount of BTC you want to buy/sell");
            }
            return retVal;
        }
    }
}
