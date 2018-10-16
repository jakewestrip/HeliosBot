using Discord;
using HeliosBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HeliosBot.Services
{
    public interface IChartService
    {
        string ChartAndSave(List<ASXHighchartResult> data);
    }

    public class ChartService : IChartService
    {
        private readonly IDiscordClient _discordClient;

        public ChartService(IDiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public string ChartAndSave(List<ASXHighchartResult> data)
        {
            return "NOT IMPLEMENTED";
        }
    }
}
