using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Classic.Model;
using LibMinecraft.Classic.Server;

namespace ClassicServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MinecraftClassicServer model = new MinecraftClassicServer();
            model.Port = 25565;
            model.MaxPlayers = 25;
            model.Name = "Test LibMinecraft Classic Server";
            model.Private = false;

            ClassicServer server = new ClassicServer();
            server.OnPlayerConnectionChanged += new EventHandler<PlayerConnectionEventArgs>(server_OnPlayerConnectionChanged);
            Console.WriteLine(server.Start(model));

            while (true)
            {
                string input = Console.ReadLine();
                if (input.StartsWith("motd "))
                    model.MotD = input.Substring(5);
            }
        }

        static void server_OnPlayerConnectionChanged(object sender, PlayerConnectionEventArgs e)
        {
            if (e.ConnectionState == ConnectionState.Connected)
                Console.WriteLine(e.Client.UserName + " joined the game.");
            else
                Console.WriteLine(e.Client.UserName + " left the game.");
        }
    }
}
