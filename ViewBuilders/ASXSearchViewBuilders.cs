using Discord;
using HeliosBot.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HeliosBot.ViewBuilders
{
    public static class ASXSearchViewBuilders
    {
        public static EmbedBuilder WithOkColor(this EmbedBuilder embed)
        {
            return embed.WithColor(Color.Green);
        }

        public static EmbedBuilder WithErrorColor(this EmbedBuilder embed)
        {
            return embed.WithColor(Color.Red);
        }

        public static EmbedBuilder BuildLookupView(ASXLookupResult data)
        {
            var embed = new EmbedBuilder()
                .AddField(fb => fb.WithName(":earth_asia: " + Format.Bold("Code")).WithValue($"[{data.Code}]({data.Endpoint})"))
                .AddField(fb => fb.WithName(":moneybag: " + Format.Bold("Last Price")).WithValue($"${data.Last_Price}").WithIsInline(true))
                .WithOkColor()
                .WithFooter(efb => efb.WithText(data.Endpoint));

            if (data.Day_High_Price != 0 && data.Day_Low_Price != 0)
            {
                embed
                .AddField(fb => fb.WithName(":chart_with_upwards_trend: " + Format.Bold("Daily High")).WithValue($"${data.Day_High_Price}").WithIsInline(true))
                .AddField(fb => fb.WithName(":chart_with_downwards_trend: " + Format.Bold("Daily Low")).WithValue($"${data.Day_Low_Price}").WithIsInline(true))
                .AddField(fb => fb.WithName("🌄 " + Format.Bold("Open")).WithValue($"${data.Open_Price}").WithIsInline(true));

                if (data.Change_Price != null)
                {
                    embed.AddField(fb => fb.WithName(":money_with_wings: " + Format.Bold("Todays Change")).WithValue($"${data.Change_Price} ({data.Change_In_Percent})").WithIsInline(true));
                }
            }

            if (!String.IsNullOrEmpty(data.Previous_Day_Percentage_Change))
            {
                embed.AddField(fb => fb.WithName(":money_with_wings: " + Format.Bold("Prev. Daily Change")).WithValue($"{data.Previous_Day_Percentage_Change}").WithIsInline(true));
            }

            if (data.Previous_Close_Price != null && data.Previous_Close_Price != 0)
            {
                embed.AddField(fb => fb.WithName(":night_with_stars: " + Format.Bold("Prev. Close")).WithValue($"${data.Previous_Close_Price}").WithIsInline(true));
            }

            if (data.Year_High_Price != null && data.Year_High_Price != 0)
                embed.AddField(fb => fb.WithName(":chart_with_upwards_trend: " + Format.Bold("Yearly High")).WithValue($"${data.Year_High_Price}").WithIsInline(true));

            if (data.Year_Low_Price != null && data.Year_Low_Price != 0)
            {
                embed.AddField(fb => fb.WithName(":chart_with_downwards_trend: " + Format.Bold("Yearly Low")).WithValue($"${data.Year_Low_Price}").WithIsInline(true));
            }

            if (!String.IsNullOrEmpty(data.Year_Change_In_Percentage))
            {
                embed.AddField(fb => fb.WithName(":gem: " + Format.Bold("Yearly % Change")).WithValue($"{data.Year_Change_In_Percentage}").WithIsInline(true));
            }

            if (data.Volume != null && data.Volume != 0)
            {
                var daysVolume = string.Format(CultureInfo.InvariantCulture, "{0:N0}", data.Volume);
                embed.AddField(fb => fb.WithName(":bar_chart:  " + Format.Bold("Todays Volume")).WithValue($"{daysVolume}").WithIsInline(true));
            }

            if (data.Average_Daily_Volume != null && data.Average_Daily_Volume != 0)
            {
                var avgDailyVol = string.Format(CultureInfo.InvariantCulture, "{0:N0}", data.Average_Daily_Volume);
                embed.AddField(fb => fb.WithName(":bar_chart:  " + Format.Bold("Avg Daily Vol")).WithValue($"{avgDailyVol}").WithIsInline(true));
            }

            var totalVol = string.Format(CultureInfo.InvariantCulture, "{0:N0}", data.Number_Of_Shares);
            embed.AddField(fb => fb.WithName(":bar_chart:  " + Format.Bold("Total Vol")).WithValue($"{totalVol}").WithIsInline(true));

            if (data.Market_Cap != null && data.Market_Cap != 0)
            {
                var marketCap = string.Format(CultureInfo.InvariantCulture, "{0:N0}", Convert.ToInt64(data.Market_Cap));
                embed.AddField(fb => fb.WithName(":dollar:  " + Format.Bold("Market Cap")).WithValue($"${marketCap}").WithIsInline(true));
            }

            if (data.EPS != 0)
            {
                string peValue = (data.PE != 0 ? data.PE.ToString() : "N/A (Negative EPS)");
                embed
                    .AddField(fb => fb.WithName(":dollar:  " + Format.Bold("EPS")).WithValue($"${data.EPS}").WithIsInline(true))
                    .AddField(fb => fb.WithName(":scales:  " + Format.Bold("PE Ratio")).WithValue($"{peValue}").WithIsInline(true));
            }

            return embed;
        }

        public static EmbedBuilder BuildCompanyView(ASXCompanyResult data)
        {
            var embed = new EmbedBuilder()
                    .WithTitle($"{data.Code} - {data.Name_Full}")
                    .WithOkColor()
                    .WithFooter(efb => efb.WithText(data.Endpoint));

            if (!String.IsNullOrEmpty(data.Principal_Activities))
            {
                embed.AddField(fb => fb.WithName(Format.Bold("Activities")).WithValue($"{data.Principal_Activities}").WithIsInline(true));
            }

            if (!String.IsNullOrEmpty(data.Industry_Group_Name) && !String.IsNullOrEmpty(data.Sector_Name))
            {
                embed.AddField(fb => fb.WithName(":office:  " + Format.Bold("Industry/Sector")).WithValue($"{data.Industry_Group_Name}/{data.Sector_Name}").WithIsInline(true));
            }

            if (!String.IsNullOrEmpty(data.Industry_Group_Name) && !String.IsNullOrEmpty(data.Sector_Name))
            {
                embed.AddField(fb => fb.WithName(":calendar_spiral: " + Format.Bold("Listing Date")).WithValue($"{data.Listing_Date}").WithIsInline(true));
            }

            if (!String.IsNullOrEmpty(data.Investor_Relations_Url))
            {
                embed
                    .AddField(fb => fb.WithName(":link: " + Format.Bold("Investor Relations")).WithValue($"[URL]({data.Investor_Relations_Url})").WithIsInline(true));
            }

            if (data.Recent_Announcement.ToLower() == "true")
            {
                embed
                    .AddField(fb => fb.WithName(":loudspeaker:  " + Format.Bold("Has Recent Announcement?")).WithValue("Yes").WithIsInline(true));
            }
            else if (data.Recent_Announcement.ToLower() == "false")
            {
                embed
                    .AddField(fb => fb.WithName(":mute:  " + Format.Bold("Has Recent Announcement?")).WithValue("No").WithIsInline(true));
            }

            return embed;
        }

        public static EmbedBuilder BuildAnnualReportView(ASXAnnualReportResult data)
        {
            Report report = data.Latest_Annual_Reports.FirstOrDefault();
            var embed = new EmbedBuilder();
            if (report != null)
            {
                embed
                    .WithTitle($"Latest Annual Report for {data.Code}")
                    .AddField(fb => fb.WithName(Format.Bold($"{data.Name_Full}")).WithValue($":link: [{report.Header}]({report.Url}) ({report.Size}) - {report.Number_Of_Pages} Pages"))
                    //.AddField(fb => fb.WithName(Format.Bold("Date")).WithValue($"{report.document_date}").WithIsInline(true))
                    .AddField(fb => fb.WithName(":calendar_spiral: " + Format.Bold("Release Date")).WithValue($"{report.Document_Release_Date}").WithIsInline(true))
                    .WithOkColor()
                    .WithFooter(efb => efb.WithText(data.Endpoint));
            }
            else
            {
                embed
                    .WithTitle($"No Annual Report for {data.Code} found!")
                    .WithErrorColor()
                    .WithFooter(efb => efb.WithText(data.Endpoint));
            }

            return embed;
        }

        public static EmbedBuilder BuildAnnouncementsView(ASXAnnouncementsResult data)
        {
            var embed = new EmbedBuilder()
                    .WithTitle($":loudspeaker:  Annoucements for {data.StockCode}")
                    .WithOkColor()
                    .WithFooter(efb => efb.WithText(data.Endpoint));

            foreach (var announcement in data.Data)
            {
                embed
                    .AddField(fb => fb.WithName(":calendar_spiral:  " + Format.Bold($"{announcement.Document_Date}")).WithValue($"[{announcement.Header}]({announcement.Url})"));
            }
            return embed;
        }

        public static EmbedBuilder BuildSimilarView(ASXSimilarResult data)
        {
            var embed = new EmbedBuilder();

            embed
                .WithTitle($":earth_asia: Similar to {data.StockCode}")
                .WithOkColor()
                .WithFooter(efb => efb.WithText(data.Endpoint));

            foreach (var company in data.Companies)
            {
                embed
                    .AddField(fb => fb.WithName(Format.Bold($"{company.Code}")).WithValue($"{company.Name_Full} - ${company.Primary_Share.Last_Price}"));
                //.AddField(fb => fb.WithName(Format.Bold("Industry/Sector")).WithValue($"{company.industry_group_name}/{company.sector_name}").WithIsInline(true))
                //.AddField(fb => fb.WithName(Format.Bold("Activities")).WithValue($"{company.principal_activities}").WithIsInline(true))
                //.AddField(fb => fb.WithName(Format.Bold("Title")).WithValue($"${company.primary_share.code}").WithIsInline(true))
                //.AddField(fb => fb.WithName(":moneybag: " + Format.Bold("Last Price")).WithValue($""));
                //.AddField(fb => fb.WithName(Format.Bold("Open Price")).WithValue($"${company.primary_share.open_price}").WithIsInline(true))
                //.AddField(fb => fb.WithName(Format.Bold("Change %")).WithValue($"${company.primary_share.change_in_percent}").WithIsInline(true))
                //.AddField(fb => fb.WithName(Format.Bold("Avg Daily Vol")).WithValue($"${company.primary_share.average_daily_volume}").WithIsInline(true))
                //.AddField(fb => fb.WithName(Format.Bold("Total Shares")).WithValue($"${company.primary_share.number_of_shares}").WithIsInline(true))
                //.AddField(fb => fb.WithName(Format.Bold("Market Cap")).WithValue($"{company.primary_share.market_cap}").WithIsInline(true));
            }
            return embed;
        }

        public static EmbedBuilder BuildUpcomingEventsView(ASXUpcomingEventsResult data)
        {
            var embed = new EmbedBuilder();

            if (data.Data.FirstOrDefault() == null)
            {
                embed
                    .WithTitle($"No Upcoming Events for {data.StockCode} found!")
                    .WithErrorColor()
                    .WithFooter(efb => efb.WithText(data.Endpoint));
            }
            else
            {
                embed
                    .WithTitle($":speaking_head:  Upcoming Events for {data.StockCode}")
                    .WithOkColor()
                    .WithFooter(efb => efb.WithText(data.Endpoint));
                foreach (var e in data.Data)
                {
                    embed
                        .AddField(fb => fb.WithName(":calendar_spiral:  " + Format.Bold($"{e.Start_Date}")).WithValue($"{e.Type} - {e.Name}"));
                }
            }

            return embed;
        }

        public static EmbedBuilder BuildIndexView(ASXHighchartResult last, ASXHighchartResult secondLast)
        {
            var change = secondLast.Price - last.Price;
            var changePercent = (change / secondLast.Price) * 100;
            var embed = new EmbedBuilder()
                .WithTitle($"No Upcoming Events for {last.StockCode} found!")
                .AddField(fb => fb.WithName(":moneybag: " + Format.Bold("Last Price")).WithValue($"${last.Price}").WithIsInline(true))
                .AddField(fb => fb.WithName(":money_with_wings: " + Format.Bold("Change")).WithValue($"${change} ({changePercent})").WithIsInline(true))
                .WithOkColor()
                .WithFooter(efb => efb.WithText(last.Endpoint));

            return embed;
        }
    }
}
