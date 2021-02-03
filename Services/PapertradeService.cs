using Discord;
using HeliosBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace HeliosBot.Services
{
    public interface IPapertradeService
    {
        Task<Result<PapertradePortfolioResult>> GetMoney(long userId);
        Task<Result<PapertradeBuyResult>> BuyShares(long userId, string stockCode, long amount);
        Task<Result<PapertradeSellResult>> SellShares(long userId, string stockCode, long amount);
    }

    public class PapertradeService : IPapertradeService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IASXService _asxService;

        public PapertradeService(DatabaseContext databaseContext, IASXService asxService)
        {
            _databaseContext = databaseContext;
            _asxService = asxService;
        }

        private async Task<PapertradeUser> GetOrCreateUser(long userId)
        {
            var c = _databaseContext.PapertradeUser
                .Include(x => x.OwnedStocks)
                .Include(x => x.Transactions)
                .Where(x => x.UserId == userId)
                .ToList();

            if (c.Count > 0)
                return c.First();

            var newUser = new PapertradeUser()
            {
                UserId = userId,
                Money = 50000,
                OwnedStocks = new List<PapertradeOwnedStock>(),
                Transactions = new List<PapertradeTransaction>()
            };

            _databaseContext.PapertradeUser.Add(newUser);
            await _databaseContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<Result<PapertradePortfolioResult>> GetMoney(long userId)
        {
            var user = await GetOrCreateUser(userId);

            if (user.Transactions != null)
            {
                var holdings = new List<PapertradePortfolioHolding>();
                
                var transactionsByStock = user.Transactions.GroupBy(x => x.StockCode);

                foreach (var stock in transactionsByStock)
                {
                    var code = stock.Key;
                    var buys = stock.Where(x => x.TransactionType == PapertradeTransactionType.BUY).ToList();
                    var bought = buys.Sum(x => x.Shares);
                    var sold = stock.Where(x => x.TransactionType == PapertradeTransactionType.SELL)
                        .Sum(x => x.Shares);
                    var totalHeld = bought - sold;

                    if (totalHeld == 0)
                        continue;

                    var orderedBuys = buys.OrderBy(x => x.Price).ToList();
                    
                    var numberToReconcile = totalHeld;
                    int smallestBuyIndex = 0;
                    decimal totalSpent = 0;
                    while (numberToReconcile != 0)
                    {
                        var smallestBuy = orderedBuys[smallestBuyIndex];
                        var reconcilingHere = Math.Min(numberToReconcile, smallestBuy.Shares);
                        totalSpent += (reconcilingHere * smallestBuy.Price);
                        numberToReconcile -= reconcilingHere;
                        smallestBuyIndex++;
                    }

                    var lookup = await _asxService.Lookup(code);
                    if (!lookup.IsSuccess)
                        return new Result<PapertradePortfolioResult> { IsSuccess = false, Message = lookup.Message };

                    var data = lookup.Payload;

                    if (!data.Last_Price.HasValue)
                        return new Result<PapertradePortfolioResult>() { IsSuccess = false, Message = "Current price could not be retrieved." };

                    var lastPrice = (decimal)data.Last_Price.Value;
                    var value = lastPrice * totalHeld;
                    var totalGain = (totalHeld * lastPrice) - totalSpent;
                    
                    var holding = new PapertradePortfolioHolding()
                    {
                        StockCode = code,
                        Quantity = totalHeld,
                        PricePaid = totalSpent,
                        Value = $"{value:F2}",
                        LastPrice = lastPrice,
                        TotalGain = $"${totalGain:F2}",
                        TotalGainPercent = $"{(totalGain / totalSpent):P}"
                    };
                    
                    holdings.Add(holding);
                }
                
                return new Result<PapertradePortfolioResult>()
                {
                    IsSuccess = true,
                    Message = String.Empty,
                    Payload = new PapertradePortfolioResult()
                    {
                        Holdings = holdings,
                        Money = user.Money,
                        UserId = user.UserId
                    }
                };
            }

            return new Result<PapertradePortfolioResult>()
            {
                IsSuccess = false,
                Message = String.Empty
            };
        }

        public async Task<Result<PapertradeBuyResult>> BuyShares(long userId, string stockCode, long amount)
        {
            var stockCodeUpper = stockCode.ToUpper();
            var lookup = await _asxService.Lookup(stockCodeUpper);

            if (!lookup.IsSuccess)
                return new Result<PapertradeBuyResult> { IsSuccess = false, Message = lookup.Message };

            var data = lookup.Payload;

            if (!data.Last_Price.HasValue)
                return new Result<PapertradeBuyResult>() { IsSuccess = false, Message = "Current price could not be retrieved." };

            var price = data.Last_Price.Value;
            var cost = amount * price;
            var user = await GetOrCreateUser(userId);

            var result = new PapertradeBuyResult()
            {
                UserId = userId,
                Amount = amount,
                StockCode = stockCodeUpper,
                Cost = cost,
                Price = price
            };

            if (user.Money < cost)
                return new Result<PapertradeBuyResult>() { IsSuccess = false, Message = "You don't have enough money for that." };

            user.Money -= cost;
            result.Money = user.Money;

            if (user.OwnedStocks != null && user.OwnedStocks.Any(x => x.StockCode == stockCodeUpper))
            {
                user.OwnedStocks.First(x => x.StockCode == stockCodeUpper).Shares += amount;
                result.TotalAmount = user.OwnedStocks.First(x => x.StockCode == stockCodeUpper).Shares;
            }
            else
            {
                if (user.OwnedStocks == null)
                    user.OwnedStocks = new List<PapertradeOwnedStock>();

                user.OwnedStocks.Add(new PapertradeOwnedStock()
                {
                    StockCode = stockCodeUpper,
                    Shares = amount,
                    User = user
                });

                result.TotalAmount = amount;
            }
            
            user.Transactions.Add(new PapertradeTransaction()
            {
                StockCode = stockCodeUpper,
                Shares = amount,
                TransactionType = PapertradeTransactionType.BUY,
                User = user,
                Price = (decimal)price
            });

            await _databaseContext.SaveChangesAsync();

            return new Result<PapertradeBuyResult>()
            {
                IsSuccess = true,
                Payload = result
            };
        }

        public async Task<Result<PapertradeSellResult>> SellShares(long userId, string stockCode, long amount)
        {
            var stockCodeUpper = stockCode.ToUpper();
            var lookup = await _asxService.Lookup(stockCodeUpper);

            if (!lookup.IsSuccess)
                return new Result<PapertradeSellResult> { IsSuccess = false, Message = lookup.Message };

            var data = lookup.Payload;

            if (!data.Last_Price.HasValue)
                return new Result<PapertradeSellResult>() { IsSuccess = false, Message = "Current price could not be retrieved." };

            var price = data.Last_Price.Value;
            var cost = amount * price;
            var user = _databaseContext.PapertradeUser.First(x => x.UserId == userId);

            var result = new PapertradeSellResult()
            {
                UserId = userId,
                Amount = amount,
                StockCode = stockCodeUpper,
                Cost = cost,
                Price = price
            };

            if (user.OwnedStocks == null || user.OwnedStocks.All(x => x.StockCode != stockCodeUpper) || 
            user.OwnedStocks.First(x => x.StockCode == stockCodeUpper).Shares < amount)
                return new Result<PapertradeSellResult> { IsSuccess = false, Message = "You don't have those shares to sell." };

            user.Money += cost;
            result.Money = user.Money;

            var ownedStock = user.OwnedStocks.First(x => x.StockCode == stockCodeUpper);
            ownedStock.Shares -= amount;
            result.TotalAmount = user.OwnedStocks.First(x => x.StockCode == stockCodeUpper).Shares;

            if (result.TotalAmount == 0)
                user.OwnedStocks.Remove(ownedStock);
            
            user.Transactions.Add(new PapertradeTransaction()
            {
                StockCode = stockCodeUpper,
                Shares = amount,
                TransactionType = PapertradeTransactionType.SELL,
                User = user,
                Price = (decimal)price
            });

            await _databaseContext.SaveChangesAsync();

            return new Result<PapertradeSellResult>()
            {
                IsSuccess = true,
                Payload = result
            };
        }
    }
}
