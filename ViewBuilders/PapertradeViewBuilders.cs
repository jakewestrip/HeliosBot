using Discord;
using HeliosBot.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HeliosBot.ViewBuilders
{
    public static class PapertradeViewBuilders
    {
        public static EmbedBuilder BuildPortfolioView(PapertradePortfolioResult data)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Your portfolio")
                .AddField(fb => fb.WithName(":moneybag: " + Format.Bold("Money")).WithValue(data.Money));

            foreach (var v in data.Stocks)
            {
                embed.AddField(fb => fb.WithName(":bar_chart: " + Format.Bold(v.Key)).WithValue(v.Value).WithIsInline(true));
            }

            return embed.WithOkColor(); ;
        }

        public static EmbedBuilder BuildBuyView(PapertradeBuyResult data)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"Bought {data.Amount} shares of {data.StockCode} at {data.Price}")
                .AddField(fb => fb.WithName(":money_with_wings: " + Format.Bold("Total cost")).WithValue(data.Cost))
                .AddField(fb => fb.WithName(":moneybag: ").WithValue($"You now have a total of {data.TotalAmount} shares of {data.StockCode}").WithIsInline(true))
                .WithOkColor();

            return embed;
        }

        public static EmbedBuilder BuildSellView(PapertradeSellResult data)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"Sold {data.Amount} shares of {data.StockCode} at {data.Price}")
                .AddField(fb => fb.WithName(":money_with_wings: " + Format.Bold("Total gain")).WithValue(data.Cost))
                .AddField(fb => fb.WithName(":moneybag: ").WithValue($"You now have a total of {data.TotalAmount} shares of {data.StockCode}").WithIsInline(true))
                .WithOkColor();

            return embed;
        }
    }
}
