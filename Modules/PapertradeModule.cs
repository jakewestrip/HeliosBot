using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using HeliosBot.Services;
using HeliosBot.ViewBuilders;

namespace HeliosBot.Modules
{
    public class PapertradeModule : ModuleBase
    {
        private readonly IPapertradeService _papertradeService;

        public PapertradeModule(IPapertradeService papertradeService)
        {
            _papertradeService = papertradeService;
        }

        [Command("portfolio"), Alias("money")]
        public async Task Portfolio([Remainder] string parameter = "")
        {
            var result = await _papertradeService.GetMoney((long) Context.Message.Author.Id);

            if (result.IsSuccess)
            {
                var embed = PapertradeViewBuilders.BuildPortfolioView(result.Payload);

                await ReplyAsync("", embed: embed);
            }
            else
            {
                await ReplyAsync(result.Message);
            }
        }

        [Command("buy"), Summary("GET SHARES")]
        public async Task BuyShares(string ticker, long amount, [Remainder] string parameter = "")
        {
            var result = await _papertradeService.BuyShares((long)Context.Message.Author.Id, ticker, amount);

            if (result.IsSuccess)
            {
                var embed = PapertradeViewBuilders.BuildBuyView(result.Payload);

                await ReplyAsync("", embed: embed);
            }
            else
            {
                await ReplyAsync(result.Message);
            }
        }

        [Command("sell"), Summary("SELL SHARES")]
        public async Task SellShares(string ticker, long amount, [Remainder] string parameter = "")
        {
            var result = await _papertradeService.SellShares((long)Context.Message.Author.Id, ticker, amount);

            if (result.IsSuccess)
            {
                var embed = PapertradeViewBuilders.BuildSellView(result.Payload);

                await ReplyAsync("", embed: embed);
            }
            else
            {
                await ReplyAsync(result.Message);
            }
        }
    }
}
