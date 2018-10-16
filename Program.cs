using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace HeliosBot
{
    public class Program
    {
        public static void Main(string[] args) => new HeliosBot().Start().GetAwaiter().GetResult();
    }
}
