using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model;
using LibMinecraft.Model.Blocks;

namespace LibMinecraft.Server
{
    public class WeatherManager
    {
        public bool WeatherEnabled { get; set; }
        public bool WeatherOccuring { get; set; }
        public int TicksUntilUpdate { get; set; }
        public int TicksUntilThunder { get; set; }
        public int TicksUntilAccumulation { get; set; }

        public int MinTicksBetweenThunder { get; set; }
        public int MaxTicksBetweenThunder { get; set; }
        public int MinTicksBetweenUpdate { get; set; }
        public int MaxTicksBetweenUpdate { get; set; }
        public int MinTicksBetweenAccumunation { get; set; }
        public int MaxTicksBetweenAccumulation { get; set; }

        public bool EnableAccumulation { get; set; }
        public bool EnableThunder { get; set; }
        public bool EnableBetterAccumulation { get; set; }

        public WeatherManager()
        {
            WeatherEnabled = true;
            WeatherOccuring = false;
            EnableAccumulation = true;
            EnableThunder = true;
            EnableBetterAccumulation = false;

            MinTicksBetweenUpdate = 1000;
            MaxTicksBetweenUpdate = 5000;
            MinTicksBetweenThunder = 250;
            MaxTicksBetweenThunder = 750;
            MinTicksBetweenAccumunation = 0;
            MaxTicksBetweenAccumulation = 100;

            TicksUntilUpdate = MultiplayerServer.Random.Next(MinTicksBetweenUpdate, MaxTicksBetweenUpdate);
            TicksUntilThunder = MultiplayerServer.Random.Next(MinTicksBetweenThunder, MaxTicksBetweenThunder);
            TicksUntilAccumulation = MultiplayerServer.Random.Next(MinTicksBetweenAccumunation, MaxTicksBetweenAccumulation);
        }

        public virtual void Tick(Level level, MultiplayerServer server)
        {
            TicksUntilUpdate--;
            TicksUntilThunder--;
            TicksUntilAccumulation--;
            if (TicksUntilThunder < 0)
            {
                TicksUntilThunder = MultiplayerServer.Random.Next(MinTicksBetweenThunder, MaxTicksBetweenThunder);
                if (WeatherEnabled && WeatherOccuring && EnableThunder)
                {
                    Vector3 strikeLocation = new Vector3(
                        MultiplayerServer.Random.Next(-500, 500),
                        MultiplayerServer.Random.Next(-500, 500),
                        MultiplayerServer.Random.Next(-500, 500));
                    server.CreateLightning(strikeLocation, level.Overworld);
                }
            }
            if (TicksUntilUpdate < 0)
            {
                if (WeatherEnabled)
                {
                    WeatherOccuring = !WeatherOccuring;
                    TicksUntilUpdate = MultiplayerServer.Random.Next(MinTicksBetweenUpdate, MaxTicksBetweenUpdate);
                    server.SetWeather(WeatherOccuring, level.Overworld);
                }
            }
            if (TicksUntilAccumulation < 0)
            {
                if (WeatherEnabled && WeatherOccuring && EnableAccumulation)
                {
                    for (int i = 0; i < level.Overworld.Regions.Count; i++)
                    {
                        for (int j = 0; j < level.Overworld.Regions[i].MapColumns.Count; j++)
                        {
                            if (MultiplayerServer.Random.Next(3) != 0)
                                continue;
                            int randBlock = MultiplayerServer.Random.Next(255);
                            if (level.Overworld.Regions[i].MapColumns[j].Biomes[randBlock] == (byte)Biome.IcePlains ||
                                level.Overworld.Regions[i].MapColumns[j].Biomes[randBlock] == (byte)Biome.IceMountains ||
                                level.Overworld.Regions[i].MapColumns[j].Biomes[randBlock] == (byte)Biome.FrozenRiver ||
                                level.Overworld.Regions[i].MapColumns[j].Biomes[randBlock] == (byte)Biome.FrozenOcean ||
                                level.Overworld.Regions[i].MapColumns[j].Biomes[randBlock] == (byte)Biome.Taiga ||
                                level.Overworld.Regions[i].MapColumns[j].Biomes[randBlock] == (byte)Biome.TaigaHills)
                            {
                                Block b = level.Overworld.Regions[i].MapColumns[j].GetBlock(new Vector3(randBlock / 16,
                                    level.Overworld.Regions[i].MapColumns[j].HeightMap[randBlock], randBlock % 16));
                                if (b is SnowCapBlock)
                                {
                                    if (b.Metadata < 15)
                                        b.Metadata++;
                                    level.Overworld.SetBlock(new Vector3(randBlock / 16, level.Overworld.Regions[i].MapColumns[j].HeightMap[randBlock], randBlock % 16) +
                                        level.Overworld.Regions[i].MapColumns[j].Location + level.Overworld.Regions[i].MapColumns[j].Region.Location, b);
                                }
                                else if (b.Transparent == BlockOpacity.Opaque)
                                    level.Overworld.SetBlock(new Vector3(randBlock / 16, level.Overworld.Regions[i].MapColumns[j].HeightMap[randBlock] + 1, randBlock % 16) +
                                        level.Overworld.Regions[i].MapColumns[j].Location + level.Overworld.Regions[i].MapColumns[j].Region.Location, new SnowCapBlock());
                            }
                        }
                    }
                }
                TicksUntilAccumulation = TicksUntilAccumulation = MultiplayerServer.Random.Next(MinTicksBetweenAccumunation, MaxTicksBetweenAccumulation);
            }
        }
    }
}
