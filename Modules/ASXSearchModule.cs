using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HeliosBot.Services;
using HeliosBot.ViewBuilders;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HeliosBot.Modules
{
    [Group("asx")]
    public class ASXSearchModule : ModuleBase
    {
        private readonly IASXService _asxService;

        public ASXSearchModule(IASXService asxService)
        {
            _asxService = asxService;
        }

        [Command(""), Summary("Perform an ASX lookup")]
        public async Task ASXSearch(string stockCode, string option = "def", [Remainder] string parameter = "")
        {
            if(option != "def")
            {
                switch (option)
                {
                    case "company":
                    case "info":
                    case "i":
                        await ASXCompany(stockCode);
                        return;
                    case "annualreport":
                    case "annual":
                    case "report":
                    case "r":
                    case "ar":
                        await ASXAnnualReport(stockCode);
                        return;
                    case "announce":
                    case "announcements":
                    case "an":
                    case "ann":
                        await ASXAnnouncements(stockCode);
                        return;
                    case "similar":
                    case "sim":
                    case "s":
                        await ASXSimilar(stockCode);
                        return;
                    case "upcomingevents":
                    case "e":
                    case "agm":
                    case "events":
                        await ASXUpcomingEvents(stockCode);
                        return;
                    default:
                        break;
                }
            }

            if(stockCode.ToUpper() == "XAO")
            {
                await ASXIndex();
                return;
            }

            var dataResult = await _asxService.Lookup(stockCode);

            if (!dataResult.IsSuccess)
            {
                await ReplyAsync(dataResult.Message);
                return;
            }

            var data = dataResult.Payload;

            var embed = ASXSearchViewBuilders.BuildLookupView(data);

            await ReplyAsync("", embed: embed);
        }

        [Command("company"), Summary("Retrieves information about a company, so you can learn more about them."), Alias("info", "i")]
        public async Task ASXCompany(string stockCode, [Remainder] string parameter = "")
        {
            var dataResult = await _asxService.Company(stockCode);

            if (!dataResult.IsSuccess)
            {
                await ReplyAsync(dataResult.Message);
                return;
            }

            var data = dataResult.Payload;

            var embed = ASXSearchViewBuilders.BuildCompanyView(data);

            await ReplyAsync("", embed: embed);
        }

        [Command("annualreport"), Summary("Retrieves the latest annual report released by a company."), Alias("annual", "report", "r", "ar")]
        public async Task ASXAnnualReport(string stockCode, [Remainder] string parameter = "")
        {
            var dataResult = await _asxService.AnnualReport(stockCode);

            if (!dataResult.IsSuccess)
            {
                await ReplyAsync(dataResult.Message);
                return;
            }

            var data = dataResult.Payload;

            var embed = ASXSearchViewBuilders.BuildAnnualReportView(data);

            await ReplyAsync("", embed: embed);
        }

        [Command("announcements"), Summary("Retrieves the last 20 announcements."), Alias("announce", "an", "ann")]
        public async Task ASXAnnouncements(string stockCode, [Remainder] string parameter = "")
        {
            var dataResult = await _asxService.Announcements(stockCode);

            if (!dataResult.IsSuccess)
            {
                await ReplyAsync(dataResult.Message);
                return;
            }

            var data = dataResult.Payload;

            var embed = ASXSearchViewBuilders.BuildAnnouncementsView(data);

            await ReplyAsync("", embed: embed);
        }

        [Command("similar"), Summary("Retrieves similar companies."), Alias("sim", "s")]
        public async Task ASXSimilar(string stockCode, [Remainder] string parameter = "")
        {
            var dataResult = await _asxService.Similar(stockCode);

            if (!dataResult.IsSuccess)
            {
                await ReplyAsync(dataResult.Message);
                return;
            }

            var data = dataResult.Payload;

            var embed = ASXSearchViewBuilders.BuildSimilarView(data);

            await ReplyAsync("", embed: embed);
        }

        [Command("upcomingevents"), Summary("Retrieves similar companies."), Alias("events", "agm", "e")]
        public async Task ASXUpcomingEvents(string stockCode, [Remainder] string parameter = "")
        {
            var dataResult = await _asxService.UpcomingEvents(stockCode);

            if (!dataResult.IsSuccess)
            {
                await ReplyAsync(dataResult.Message);
                return;
            }

            var data = dataResult.Payload;

            var embed = ASXSearchViewBuilders.BuildUpcomingEventsView(data);

            await ReplyAsync("", embed: embed);
        }

        [Command("index"), Summary("Retrieves market indices"), Alias("ind", "xao")]
        public async Task ASXIndex([Remainder] string parameter = "")
        {
            var chartDataResult = await _asxService.Highchart("XAO");

            if (!chartDataResult.IsSuccess)
            {
                await ReplyAsync(chartDataResult.Message);
                return;
            }

            var data = chartDataResult.Payload;
            var lastPoint = data.OrderBy(x => x.Date).Last();
            var secondLastPoint = data.Last(x => x.Date != lastPoint.Date);

            var embed = ASXSearchViewBuilders.BuildIndexView(lastPoint, secondLastPoint);

            await ReplyAsync("", embed: embed);
        }
    }
}