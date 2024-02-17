using SciChatProject.Models;
using System;
using System.Text.RegularExpressions;

namespace SciChatApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot { BotName="testbot", id=9};
            bot.UpdateReceivedMessages(); // gets all the messages that were ever posted within all conversations the bot is in
            while(true)
            {
                var newmsg = bot.UpdateReceivedMessages();
                foreach(var msg in newmsg)
                {
                    Match m;
                    if((m = Regex.Match(msg.Content, @"\/forloop (?<amount>\d+)")).Success)
                    {
                        var amount = int.Parse(m.Groups["amount"].Value);
                        var content = string.Join("\n", Enumerable.Range(0, amount));
                        bot.SendMessage(content +"&a=b", msg.ConversationID);
                        Console.WriteLine($"SEND MESSAGE:\n{content}\nto Conversation {Conversation.GetConversationByID(msg.ConversationID).ConversationName}");
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}