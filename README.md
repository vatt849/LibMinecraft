LibMinecraft
============

LibMinecraft is a .NET library for Minecraft clients and servers, compatible with the Minecraft 1.2.3 protocol.

Using LibMinecraft is easy - here's an example of connecting to a server:

    MulitplayerClient client = new MultiplayerClient();
    MinecraftServer server = new MinecraftServer("127.0.0.1");
    User user = new User("username", "password");

    client.Connect(server);
    client.LogIn(user); // Now you're playing on the server!

But perhaps you want to run a server instead?

    MinecraftServer server = new MinecraftServer();
    MultiplayerServer service = new MultiplayerServer(server);

    service.AddWorld(new World());
    service.Start(); // Players can now connect to your server!

Questions? Join [#libminecraft](http://irc//irc.freenode.net#libminecraft) on irc.freenode.net

Supported Languages
-------------------

* C#
* VB.NET
* IronPython
* IronRuby
* And [many more](http://en.wikipedia.org/wiki/List_of_CLI_languages)

Featured Projects
-----------------

[PartyCraft](https://github.com/LibMinecraft/PartyCraft/) is a Minecraft server built on LibMinecraft, developed by the LibMinecraft team. Check it out!

Tell us about your project, maybe we'll feature it!

Bugs?
-----

Both bugs and feature requests go [here](https://github.com/LibMinecraft/LibMinecraft/issues).

Want to Contribute?
-------------------

Follow our guide: [PullRequestGuide.md](https://github.com/LibMinecraft/LibMinecraft/blob/master/PullRequestGuide.md).

Links
-----

You can get Minecraft at [minecraft.net](http://minecraft.net)

LibMinecraft depends on [SharpZipLib](http://www.icsharpcode.net/OpenSource/SharpZipLib/Default.aspx) and [LibNBT](https://github.com/aphistic/libnbt).

Our documentation was created with the help of [GhostDoc](http://submain.com/products/ghostdoc.aspx), who graciously provided all of our developers with free licenses.

LibMinecraft is not affiliated with or endorsed by Mojang.