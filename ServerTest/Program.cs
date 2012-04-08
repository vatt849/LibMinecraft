using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Model;
using LibMinecraft.Model.Entities;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Model.Items;

namespace ServerTest
{
    class Program
    {
        static MultiplayerServer service;
        static MinecraftServer server;

        static void Main(string[] args)
        {
            string input;
            server = new MinecraftServer();
            while (true)
            {
                Console.Write(">");
                input = Console.ReadLine();

                if (input == "q" || input == "quit")
                    return;
                if (input == "?" || input == "help")
                    OutputHelp();
                if (input == "motd")
                    Console.WriteLine(server.MotD);
                if (input.StartsWith("motd "))
                    server.MotD = input.Substring(5);
                if (input == "clear" || input == "cls")
                    Console.Clear();
                if (input.StartsWith("time "))
                    service.Levels[0].Time = long.Parse(input.Substring(5));
                if (input.StartsWith("gamemode "))
                {
                    string[] parameters = input.Split(' ');
                    bool creative = parameters[2].ToLower() == "creative" || parameters[2].ToLower() == "1";
                    service.GetClient(parameters[1]).PlayerEntity.GameMode = creative ? GameMode.Creative : GameMode.Survival;
                }
                if (input.StartsWith("send "))
                {
                    string[] parameters = input.Split(' ');
                    Dimension d = Dimension.Overworld;
                    switch (parameters[2].ToLower())
                    {
                        case "nether":
                            d = Dimension.Nether;
                            break;
                        case "end":
                            d = Dimension.TheEnd;
                            break;
                    }
                    service.GetClient(parameters[1]).PlayerEntity.Dimension = d;
                }
                if (input.StartsWith("location "))
                {
                    string[] parameters = input.Split(' ');
                    PlayerEntity p = service.GetClient(parameters[1]).PlayerEntity;
                    service.SendChat(p.Location.X + ", " + p.Location.Y + ", " + p.Location.Z);
                }
                if (input.StartsWith("start "))
                {
                    server.Port = int.Parse(input.Substring(6));
                    service = new MultiplayerServer(server);
                    Level level = new Level(new DefaultGenerator());
                    level.Save("world", service);
                    service.AddLevel(level);
                    Block.OverrideBlock(new TestDirt());
                    Item.OverrideItem(new TestStick());
                    //service.LogEnabled = true;
                    service.OnChat += new EventHandler<ChatEventArgs>(service_OnChat);
                    service.OnPlayerJoin += new EventHandler<PlayerEventArgs>(service_OnPlayerJoin);
                    service.OnPlayerLeave += new EventHandler<PlayerEventArgs>(service_OnPlayerLeave);
                    service.Start();
                }
                if (input == "stop")
                    service.Stop();
                if (input.StartsWith("online-mode "))
                {
                    server.OnlineMode = input == "online-mode true";
                }
                if (input.StartsWith("chat "))
                    service.SendChat(input.Substring(5));
                
            }
        }

        static void service_OnPlayerLeave(object sender, PlayerEventArgs e)
        {
            Console.WriteLine(e.Player + "left");
        }

        static void service_OnPlayerJoin(object sender, PlayerEventArgs e)
        {
            Console.WriteLine(e.Player + " joined");
        }

        static void service_OnChat(object sender, ChatEventArgs e)
        {
            Console.WriteLine(e.RawText);
            if (e.Message.StartsWith("/"))
            {
                e.Handled = true;
                if (e.Message.StartsWith("/get "))
                {
                    string[] parts = e.Message.Split(' ');
                    switch (parts[1])
                    {
                        case "weatherduetime":
                            e.RemoteClient.SendChat(service.GetLevel(e.RemoteClient).WeatherManager.TicksUntilUpdate.ToString());
                            break;
                        case "thunderduetime":
                            e.RemoteClient.SendChat(service.GetLevel(e.RemoteClient).WeatherManager.TicksUntilThunder.ToString());
                            break;
                    }
                }
                else if (e.Message.StartsWith("/set "))
                {
                    string[] parts = e.Message.Split(' ');
                    switch (parts[1])
                    {
                        case "weatherduetime":
                            service.GetLevel(e.RemoteClient).WeatherManager.TicksUntilUpdate = int.Parse(parts[2]);
                            break;
                        case "thunderduetime":
                            service.GetLevel(e.RemoteClient).WeatherManager.TicksUntilThunder = int.Parse(parts[2]);
                            break;
                    }
                }
            }
        }

        private static void OutputHelp()
        {
            Console.WriteLine("Commands:\n" +
                "quit (or q): Quit\n" +
                "help (or ?): Display this message\n" +
                "motd: Display Message of the Day\n" +
                "motd [value]: Set Message of the Day to value\n" +
                "start [port]: Starts the server on the specified port\n" +
                "stop: Stops the server\n" +
                "clear (or cls): Clear the console\n" +
                "online-mode [true|false]: Sets the online-mode property");
        }
    }

    public class TestDirt : DirtBlock
    {
        public override bool BlockPlaced(World world, Vector3 position, Vector3 blockClicked, byte facing, Entity placedBy)
        {
            world.SetBlock(position + Vector3.Up, new GlassBlock());
            return true;
        }
    }
    public class TestStick : StickItem
    {
        public override void PlayerUseItem(World world, PlayerEntity player, Vector3 location, Vector3 targetBlock, byte facing)
        {
            world.SetBlock(location, new NetherrackBlock());
            world.SetBlock(location + Vector3.Up, new NetherrackBlock());
            world.SetBlock(location + Vector3.Left + Vector3.Forward, new GoldBlock());
            world.SetBlock(location + Vector3.Left, new GoldBlock());
            world.SetBlock(location + Vector3.Left + Vector3.Backward, new GoldBlock());
            world.SetBlock(location + Vector3.Forward, new GoldBlock());
            world.SetBlock(location + Vector3.Backward, new GoldBlock());
            world.SetBlock(location + Vector3.Right + Vector3.Forward, new GoldBlock());
            world.SetBlock(location + Vector3.Right, new GoldBlock());
            world.SetBlock(location + Vector3.Right + Vector3.Backward, new GoldBlock());
            location += Vector3.Up;
            world.SetBlock(location + Vector3.Right + Vector3.Forward, new TorchBlock());
            world.SetBlock(location + Vector3.Left + Vector3.Forward, new TorchBlock());
            world.SetBlock(location + Vector3.Left + Vector3.Backward, new TorchBlock());
            world.SetBlock(location + Vector3.Right + Vector3.Backward, new TorchBlock());
        }
    }
}
