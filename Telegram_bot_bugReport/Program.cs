using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using MihaZupan;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net;

namespace Telegram_bot_bugReport
{
    
    class Program
    {
        private static TelegramBotClient Bot;
        static void Main(string[] args)
        {
            //var proxy = new HttpToSocks5Proxy("98.143.145.29", 62354);
            var proxyObject = new WebProxy("157.245.224.29:3128");
            BotActions Bot = new BotActions("", "");
            
           
            Bot.InitializationBot(proxyObject);
            Bot.Bot.StartReceiving();
            Console.ReadKey();
            Bot.Bot.StopReceiving();
            
   
        }

     


    }
}
