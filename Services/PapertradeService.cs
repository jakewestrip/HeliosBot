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
                .Where(x => x.UserId == userId)
                .ToList();

            if (c.Count > 0)
                return c.First();

            var newUser = new PapertradeUser()
            {
                UserId = userId,
                Money = 50000
            };

            _databaseContext.PapertradeUser.Add(newUser);
            await _databaseContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<Result<PapertradePortfolioResult>> GetMoney(long userId)
        {
            var user = await GetOrCreateUser(userId);

            var result = new PapertradePortfolioResult()
            {
                UserId = user.UserId,
                Money = user.Money,
                Stocks = user.OwnedStocks.ToDictionary(x => x.StockCode, x => x.Shares)
            };

            return new Result<PapertradePortfolioResult>()
            {
                IsSuccess = true,
                Message = String.Empty,
                Payload = result
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

            await _databaseContext.SaveChangesAsync();

            return new Result<PapertradeSellResult>()
            {
                IsSuccess = true,
                Payload = result
            };
        }
    }
}
