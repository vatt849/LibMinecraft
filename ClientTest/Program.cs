using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model;
using LibMinecraft.Client;
using System.Globalization;
using LibMinecraft.Model.Blocks;

namespace ClientTest
{
    class Program
    {
        static MultiplayerClient multiplayerService;

        static List<string> PrivledgedPlayers;

        static char SHChar = '~';

        static Vector3 Movement = new Vector3(0); // Blocks per second speed

        static DateTime LastTick = DateTime.Now;

        static void Main(string[] args)
        {
            PrivledgedPlayers = new List<string>();
            multiplayerService = new MultiplayerClient();
            multiplayerService.OnPlayerDeath += new EventHandler(multiplayerService_OnPlayerDeath);
            multiplayerService.OnChat += new EventHandler<ChatEventArgs>(multiplayerService_OnChat);
            multiplayerService.OnTick += new EventHandler(multiplayerService_OnTick);
            multiplayerService.DebugMode = true;
            string input = "";
            while (input != "quit" && input != "q")
            {
                Console.Write(">");
                input = Console.ReadLine();
                if (!ParseOPInput(input))
                    ParseInput(input);
            }
        }

        static void multiplayerService_OnTick(object sender, EventArgs e)
        {
            TimeSpan Tick = DateTime.Now - LastTick;
            multiplayerService.Player.Location += Movement * new Vector3(Tick.TotalSeconds);
            LastTick = DateTime.Now;
        }

        static void multiplayerService_OnPlayerDeath(object sender, EventArgs e)
        {
            multiplayerService.Respawn();
        }

        /// <summary>
        /// Input any player can make
        /// </summary>
        /// <param name="input"></param>
        private static void ParseInput(string input)
        {
            if (input.ToLower().StartsWith("info "))
            {
                MinecraftServer resp = MinecraftServer.GetServer(input.Substring("info ".Length));
                WriteLine(resp.Hostname);
                WriteLine(resp.MotD);
                WriteLine(resp.ConnectedPlayers + "/" + resp.MaxPlayers);
                WriteLine("Ping time: " + resp.PingTime + " milliseconds");
                MirrorToChat = false;
                return;
            }
            if (input.ToLower() == "under")
            {
                int y = (int)multiplayerService.Player.Location.Y;
                Block b = new AirBlock();
                while (y > -1 && multiplayerService.World.GetBlock(new Vector3(
                    multiplayerService.Player.Location.X, y, multiplayerService.Player.Location.Z)).GetType() == typeof(AirBlock))
                {
                    y--;
                    b = multiplayerService.World.GetBlock(new Vector3(
                        multiplayerService.Player.Location.X, y, multiplayerService.Player.Location.Z));
                }
                if (y != -1)
                {
                    multiplayerService.SendChat(b.GetType().Name.Replace("Block", "") + " (" + b.BlockID + ":" + b.Metadata.ToString("x") + ")");
                }
            }
        }

        /// <summary>
        /// Input that requires special privledges to use
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static bool ParseOPInput(string input)
        {
            if (input.ToLower() == "clear" || input.ToLower() == "cls")
            {
                Console.Clear();
                MirrorToChat = false;
                return true;
            }
            if (input.ToLower() == "help" || input.ToLower() == "h" || input.ToLower() == "?")
            {
                OutputCommandList();
                MirrorToChat = false;
                return true;
            }
            if (input.ToLower().StartsWith("connect "))
            {
                MinecraftServer server = new MinecraftServer();
                server.Hostname = input.Substring("connect ".Length);
                try
                {
                    multiplayerService.Connect(server);
                }
                catch (Exception e)
                {
                    WriteLine("Connection failed: " + e.Message);
                }
                MirrorToChat = false;
                return true;
            }
            if (input.ToLower() == "disconnect" || input.ToLower() == "dc")
            {
                try
                {
                    multiplayerService.Disconnect();
                }
                catch (Exception e)
                {
                    WriteLine(e.Message);
                }
                MirrorToChat = false;
                return true;
            }
            if (input.ToLower().StartsWith("login "))
            {
                try
                {
                    User user = new User();
                    user.UserName = input.Substring("login ".Length).Split(' ')[0];
                    if (input.Substring("login ".Length).Split(' ').Length == 2)
                        user.Password = input.Substring("login ".Length).Split(' ')[1];
                    multiplayerService.LogIn(user);
                    LastTick = DateTime.Now;

                    WriteLine("Login successful!");
                    WriteLine("Player Entity ID: " + multiplayerService.Player.ID);
                    WriteLine("World Seed: " + multiplayerService.World.Level.Seed);
                    WriteLine("World Mode: " + multiplayerService.World.Level.GameMode.ToString());
                    WriteLine("Current Dimension: " + multiplayerService.Player.Dimension.ToString());
                    WriteLine("Server Difficulty: " + multiplayerService.CurrentServer.Difficulty.ToString());
                }
                catch (Exception e)
                {
                    WriteLine("Login failed: " + e.Message);
                }
                MirrorToChat = false;
                return true;
            }
            if (input.ToLower().StartsWith("chat "))
            {
                try
                {
                    multiplayerService.SendChat(input.Substring("chat ".Length));
                }
                catch (Exception e)
                {
                    WriteLine("Login failed: " + e.Message);
                }
                MirrorToChat = false;
                return true;
            }
            if (input.ToLower().StartsWith("move "))
            {
                string[] inputs = input.Split(' ');
                double amount = double.Parse(inputs[2]);
                switch (inputs[1].ToLower())
                {
                    case "x":
                        multiplayerService.Player.Location.X += amount;
                        break;
                    case "y":
                        multiplayerService.Player.Location.Y += amount;
                        break;
                    case "z":
                        multiplayerService.Player.Location.Z += amount;
                        break;
                    default:
                        WriteLine("Invalid axis of movement specified.");
                        break;
                }
                MirrorToChat = false;
                multiplayerService.SendPlayerPositionAndLook();
                return true;
            }
            if (input.ToLower().StartsWith("walk "))
            {
                string[] inputs = input.Split(' ');
                double amount = double.Parse(inputs[2]);
                switch (inputs[1].ToLower())
                {
                    case "x":
                        Movement.X = amount;
                        break;
                    case "y":
                        Movement.Y = amount;
                        break;
                    case "z":
                        Movement.Z = amount;
                        break;
                    default:
                        WriteLine("Invalid axis of movement specified.");
                        break;
                }
                MirrorToChat = false;
                return true;
            }
            if (input.ToLower().StartsWith("set "))
            {
                string[] inputs = input.Split(' ');
                double amount = double.Parse(inputs[2]);
                switch (inputs[1].ToLower())
                {
                    case "x":
                        multiplayerService.Player.Location.X = amount;
                        break;
                    case "y":
                        multiplayerService.Player.Location.Y = amount;
                        break;
                    case "z":
                        multiplayerService.Player.Location.Z = amount;
                        break;
                    default:
                        WriteLine("Invalid axis of movement specified.");
                        break;
                }
                MirrorToChat = false;
                multiplayerService.SendPlayerPositionAndLook();
                return true;
            }
            if (input.ToLower() == "location")
            {
                WriteLine("X: {0}, Y: {1}, Z: {2}", multiplayerService.Player.Location.X.ToString(),
                    multiplayerService.Player.Location.Y.ToString(), multiplayerService.Player.Location.Z.ToString());
                MirrorToChat = false;
                return true;
            }
            if (input.ToLower() == "crash")
                throw new Exception();
            if (input.ToLower().StartsWith("op "))
            {
                string player = input.Substring("op ".Length);
                if (PrivledgedPlayers.Contains(player))
                {
                    WriteLine("That player already has operator status!");
                    MirrorToChat = false;
                    return true;
                }
                PrivledgedPlayers.Add(player);
                multiplayerService.SendChat("/tell " + player + " You are now a bot operator.");
                MirrorToChat = false;
                return true;
            }
            if (input.StartsWith("deop "))
            {
                string player = input.Substring("deop ".Length);
                if (!PrivledgedPlayers.Contains(player))
                {
                    WriteLine("That player isn't an operator!");
                    MirrorToChat = false;
                    return true;
                }
                PrivledgedPlayers.Remove(player);
                MirrorToChat = false;
                return true;
            }
            if (input.StartsWith("public "))
            {
                if (!ParseOPInput(input.Substring("public ".Length)))
                    ParseInput(input.Substring("public ".Length));
                MirrorToChat = false;
                return true;
            }
            if (input.StartsWith("shchar "))
            {
                char c = input["shchar ".Length];
                if (!"~!@#$%^&".Contains(c))
                {
                    WriteLine("Invalid shorthand character.  Valid characters: \"~!@#$%^&\"");
                    MirrorToChat = false;
                    return true;
                }
                MirrorToChat = false;
                return true;
            }
            if (input.StartsWith("sc "))
            {
                multiplayerService.SendChat("/" + input.Substring("sc ".Length));
                MirrorToChat = false;
                return true;
            }
            if (input == "health")
            {
                WriteLine(multiplayerService.Player.Health.ToString() + "/20");
                MirrorToChat = false;
                return true;
            }
            if (input == "food")
            {
                WriteLine(multiplayerService.Player.Food.ToString() + "/20");
                MirrorToChat = false;
                return true;
            }
            if (input == "suicide")
            {
                multiplayerService.SendChat("/kill");
                MirrorToChat = false;
                return true;
            }
            if (input.StartsWith("look "))
            {
                string[] inputs = input.Split(' ');
                double amount;
                if (inputs.Length < 3 || !double.TryParse(inputs[2], out amount))
                    return false;
                switch (inputs[1].ToLower())
                {
                    case "x":
                        multiplayerService.Player.Rotation.X = amount;
                        break;
                    case "y":
                        multiplayerService.Player.Rotation.Y = amount;
                        break;
                    default:
                        WriteLine("Invalid look axis specified.");
                        break;
                }
                MirrorToChat = false;
                return true;
            }
            return false;
        }

        static bool MirrorToChat = false;
        static string SendToPrivate = null;

        static void WriteLine(string Text)
        {
            Console.WriteLine(Text);
            if (MirrorToChat)
            {
                if (SendToPrivate == null)
                    multiplayerService.SendChat(Text);
                else
                    multiplayerService.SendChat("/tell " + SendToPrivate + " " + Text);
            }
        }

        static void WriteLine(string Text, params string[] obj)
        {
            Console.WriteLine(Text, obj);
            if (MirrorToChat)
            {
                multiplayerService.SendChat(string.Format(Text, obj));
            }
        }

        static void multiplayerService_OnChat(object sender, ChatEventArgs e)
        {
            List<ConsoleColor> ColorValues = new List<ConsoleColor>();
            ColorValues.Add(ConsoleColor.White); // Black
            ColorValues.Add(ConsoleColor.DarkBlue);
            ColorValues.Add(ConsoleColor.DarkGreen);
            ColorValues.Add(ConsoleColor.DarkCyan);
            ColorValues.Add(ConsoleColor.DarkRed);
            ColorValues.Add(ConsoleColor.Magenta);
            ColorValues.Add(ConsoleColor.DarkYellow);
            ColorValues.Add(ConsoleColor.Gray);
            ColorValues.Add(ConsoleColor.DarkGray);
            ColorValues.Add(ConsoleColor.Blue);
            ColorValues.Add(ConsoleColor.Green);
            ColorValues.Add(ConsoleColor.Cyan);
            ColorValues.Add(ConsoleColor.Red);
            ColorValues.Add(ConsoleColor.Red);
            ColorValues.Add(ConsoleColor.Yellow);
            ColorValues.Add(ConsoleColor.White);
            if (!e.RawText.StartsWith("§"))
                e.RawText = "§1" + e.RawText;
            string[] colors = e.RawText.Split('§');
            ConsoleColor old = Console.ForegroundColor;
            foreach (string str in colors)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                try
                {
                    Console.ForegroundColor = ColorValues[int.Parse(str.Remove(1), NumberStyles.HexNumber)];
                    Console.Write(str.Substring(1));
                }
                catch { }
            }
            Console.Write('\n');
            Console.ForegroundColor = old;

            try
            {
                if (e.Message.StartsWith(multiplayerService.Player.Name + ": ") || e.Private || e.Message.StartsWith(SHChar.ToString()))
                {
                    if (e.Message.StartsWith(SHChar.ToString()))
                        e.Message = multiplayerService.Player.Name + ": " + e.Message.Substring(1);
                    bool parseNormal = true;
                    MirrorToChat = true;
                    if (e.Private)
                        SendToPrivate = e.UserName;
                    bool isPriv = false;
                    foreach (string priv in PrivledgedPlayers)
                    {
                        if (e.UserName.Contains(priv))
                            isPriv = true;
                    }
                    if (isPriv)
                    {
                        if (!e.Private)
                            parseNormal = ParseOPInput(e.Message.Substring(multiplayerService.Player.Name.Length + 2));
                        else
                            parseNormal = ParseOPInput(e.Message);
                    }
                    if (!parseNormal)
                    {
                        if (!e.Private)
                            ParseInput(e.Message.Substring(multiplayerService.Player.Name.Length + 2));
                        else
                            ParseInput(e.Message);
                    }
                }
            }
            catch
            {
                if (e.RawText.Contains(multiplayerService.Player.Name + ": "))
                {
                    MirrorToChat = true;
                    ParseOPInput(e.RawText.Substring(e.RawText.LastIndexOf(":") + 1).Trim());
                }
            }
        }

        private static void OutputCommandList()
        {
            if (MirrorToChat && SendToPrivate == null)
            {
                WriteLine("Output too long to mirror to server chat.");
                WriteLine("Tip: Use /tell and I'll reply in kind.");
                MirrorToChat = false;
            }
            WriteLine("Command List:");
            WriteLine("clear: Clear the console (alt: cls)");
            WriteLine("chat [message]: Sends [message] to the server");
            WriteLine("crash: Crashes the client (useful for server testing)");
            WriteLine("connect [hostname]: Connects to a server");
            WriteLine("deop [player]: Removes bot operator status from [player]");
            WriteLine("disconnect: Disconnects from the connected server (alt: dc)");
            WriteLine("help: Display this message (alt: h, ?)");
            WriteLine("info [hostname]: Retrieves information about the specified server");
            WriteLine("login [username] (password): Logs into the connected server, using [username]");
            WriteLine("move [axis] [amount]: Move player [amount] across [axis]");
            WriteLine("op [player]: Grants [player] bot operator status");
            WriteLine("public [command]: Executes [command] and copies the output to public chat");
            WriteLine("quit: Quits this program (alt: q)");
            WriteLine("sc [command]: Executes a server command (Example: sc kick [player])");
            WriteLine("set [axis] [value]: Moves the player's [axis] location to [value]");
            WriteLine("shcar [char]: Change shorthand character to [char] (allowed: ~!@#$%^&)");
            WriteLine("\nNote: Bot operator status is unrelated to server operator status.");
            WriteLine("If the bot is a server op, you can perform server operator actions by");
            WriteLine("using the \"chat\" command.");
            WriteLine("\nConnecting to a server:");
            WriteLine("In order to completely connect to a server, you need to");
            WriteLine("perform a connect [hostname] followed by login [username].");
            WriteLine("Note: Does not work on servers with offline-mode disabled.");
        }
    }
}