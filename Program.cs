using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchTools;
namespace TwitchBot
{
   class Program
    {
        // To Generate new token go here => https://twitchapps.com/tmi/
        static bool startBot = false;
        static TwitchChatHandle chatHandle;
       
        static void Main(string[] args)
        {

            chatHandle = new TwitchChatHandle(GetAuthToken(), GetStreamName());
           
            while (startBot)
            {
                var chatEntry = chatHandle.Read();
               //log chat to console
                Console.WriteLine($"{chatEntry.sender}:{chatEntry.message}");
                
                if (chatEntry.message.Contains('!'))
                {
                    ParseCommands(chatEntry);
                }
                Thread.Sleep(50);
            }



        }

        private static string GetAuthToken()
        {
            string oAuth = "";
            Console.WriteLine("Enter oAuth token filepath: ");
            string authTokenPath = Console.ReadLine();
            oAuth = File.ReadAllText(authTokenPath);
            return oAuth;
        }

        static string GetStreamName()
        {
            Console.WriteLine("Enter stream name: ");
            string streamName = Console.ReadLine();
            if (streamName != null)
            {
                startBot = true;
                return streamName;
            }
            else { return "" ; }
            
        }

        
        static void ParseCommands(ChatEntry chatEntry)
        { 
            string chatMessage = chatEntry.message;
            var startIndex = chatMessage.IndexOf('!')+1;
            string chatCommand = chatMessage.Substring(startIndex);
            Console.WriteLine($"{"Command Issued: "}{chatEntry.sender}:{chatCommand}");
            if (chatEntry.sender == "its_holtzzy") { chatHandle.Write("Yes Sir!"); }
            switch (chatCommand)
            {
                case"cauldron":
                    chatHandle.Write($"{"Creature Soup @"}{chatEntry.sender}");
                    break;
                case"hello":
                    chatHandle.Write($"{"Hello there. @"}{chatEntry.sender}");
                    break;
                case"time":
                    chatHandle.Write($"{DateTime.UtcNow}{" UTC @"}{chatEntry.sender}");
                    break;
                case "lurk":
                    chatHandle.Write("OMG LOL OK");
                    break;
                case "itch":
                    chatHandle.Write("https://its-holtzzy.itch.io/");
                    break;
                case"github":
                    chatHandle.Write("https://github.com/holtzzy223");
                    break;
                case "bye":
                    chatHandle.Write("Good riddance!");
                    break;
                case "nft":
                    chatHandle.Write("Right click, save as...");
                    break;
                case "bot":
                    chatHandle.Write("BEHOLD BOTZZY");
                    break;
                    
            }
            
        }

    }
}
