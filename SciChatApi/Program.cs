using SciChatProject.Models;
using System;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace SciChatApi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot { id=1, Password="123"};
            bot.UpdateReceivedMessages();

            foreach(var conv in bot.GetConversations())
            {
                bot.SendMessage("Bot started!", conv.id);
            }

            while (true)
            {
                var newmsg = bot.UpdateReceivedMessages();
                foreach(var msg in newmsg)
                {
                    Match m;
                    if((m = Regex.Match(msg.Content, @"\/forloop (?<amount>\d+)")).Success)
                    {
                        var amount = int.Parse(m.Groups["amount"].Value);
                        var content = string.Join("\n", Enumerable.Range(0, amount));
                        bot.SendMessage(content, msg.ConversationID);
                        Console.WriteLine($"SEND MESSAGE:\n{content}\nto Conversation {Conversation.GetConversationByID(msg.ConversationID).ConversationName}");
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}