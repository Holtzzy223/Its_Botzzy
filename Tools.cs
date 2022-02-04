
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchTools
{
    class TwitchChatHandle
    {
  

        //OAuth can be retrieved from https://twitchapps.com/tmi/

        string channel;

        TcpClient tcpClient;
        StreamReader inputStream;
        StreamWriter outputStream;

        /// <summary>
        /// Allows you to read and write Twitch chat messages.
        /// </summary>
        /// <param name="oauth">OAuth token.</param>
        /// <param name="channel">Streamer's channel.</param>
        public TwitchChatHandle(string oauth, string channel)
        {
            this.channel = channel;

            tcpClient = new TcpClient("irc.twitch.tv", 6667);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine($"PASS {oauth}");
            outputStream.WriteLine($"NICK bot");
            outputStream.WriteLine($"USER bot 8 * :bot");
            outputStream.WriteLine($"JOIN #{channel}");
            outputStream.Flush();

            Task.Run(() => Ping());
        }

        /// <summary>
        /// Returns the next Twitch chat entry.
        /// </summary>
        /// <returns></returns>
        public ChatEntry Read()
        {
            string chatEntry;
        retry:
            try
            {
                chatEntry = inputStream.ReadLine();
            }
            catch
            {
                goto retry;
            }
            if (chatEntry == null || !chatEntry.Contains("PRIVMSG")) goto retry;       //Not Chat Message

            //Get sender's name
            int startIndex = chatEntry.IndexOf(":") + 1;
            int endIndex = chatEntry.IndexOf("!");
            int length = endIndex - startIndex;
            string chatSender = chatEntry.Substring(startIndex, length);

            //Get chat message
            startIndex = chatEntry.IndexOf(":", 1) + 1;
            string chatMessage = chatEntry.Substring(startIndex);
            return new ChatEntry(chatSender, chatMessage);
        }

        /// <summary>
        /// Writes a message to chat.
        /// </summary>
        /// <param name="chatMessage">Message to write.</param>
        public void Write(string chatMessage)
        {
            Send($":bot!bot@bot.tmi.twitch.tv PRIVMSG #{channel} :{chatMessage}");
        }

        void Send(string text)    //Sends data via TCP (Doesn't print to chat)
        {
        retry:
            try
            {
                outputStream.WriteLine(text);
                outputStream.Flush();
            }
            catch
            {
                goto retry;
            }
        }

        void Ping()     //Pings server every 5 minutes to prevent disconnection
        {
            while (true)
            {
                Send("PING irc.twitch.tv");
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
    struct ChatEntry
    {
        public string sender;
        public string message;
        public ChatEntry(string Sender, string Message)
        {
            sender = Sender;
            message = Message;
        }
    }
}