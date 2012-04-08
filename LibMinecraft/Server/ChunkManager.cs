using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Model;
using LibMinecraft.Model.Packets;
using LibMinecraft.Model.Blocks;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace LibMinecraft.Server
{
    /// <summary>
    /// Manages how the chunks are updated between the client and server
    /// </summary>
    /// <remarks></remarks>
    public static class ChunkManager
    {
        static Deflater zLibDeflater = new Deflater(5);
        /// <summary>
        /// Recalculates the map columns that should be loaded on a given client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="Server">The server.</param>
        /// <param name="forceUpdate">If set to <c>true</c>, all chunks will be unloaded and reloaded.</param>
        /// <remarks></remarks>
        public static void RecalculateClientColumns(RemoteClient client, MultiplayerServer Server, bool forceUpdate = false)
        {
            if ((int)client.PlayerEntity.Location.X >> 4 != (int)client.PlayerEntity.OldLocation.X >> 4 ||
                (int)client.PlayerEntity.Location.Z >> 4 != (int)client.PlayerEntity.OldLocation.Z >> 4 ||
                forceUpdate) // The player has crossed a column boundary, or updating should be forced
            {
                // Build the list of columns that should be loaded
                List<Vector3> newColumns = new List<Vector3>();
                for (int x = -client.PlayerEntity.MapLoadRadius; x < client.PlayerEntity.MapLoadRadius; x++)
                    for (int z = -client.PlayerEntity.MapLoadRadius; z < client.PlayerEntity.MapLoadRadius; z++)
                    {
                        newColumns.Add(new Vector3(
                            ((int)client.PlayerEntity.Location.X >> 4 << 4) + (x * 16),
                            0,
                            ((int)client.PlayerEntity.Location.Z >> 4 << 4) + (z * 16)));
                    }
                // Unload extranneous columns
                List<Vector3> currentColumns = new List<Vector3>(client.PlayerEntity.LoadedColumns);
                foreach (Vector3 column in currentColumns)
                {
                    if (!newColumns.Contains(column))
                        UnloadColumnOnClient(client, column, Server);
                }
                // Load new columns
                foreach (Vector3 column in newColumns)
                {
                    if (!client.PlayerEntity.LoadedColumns.Contains(column))
                    {
                        LoadColumnOnClient(client, column, Server);
                    }
                }
            }
        }

        /// <summary>
        /// Compresses and sends a column of chunks to the specified client.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <param name="ColumnLocation">The column location.</param>
        /// <param name="Server">The server that contains the player.</param>
        /// <remarks></remarks>
        public static void LoadColumnOnClient(RemoteClient Client, Vector3 ColumnLocation, MultiplayerServer Server)
        {
            Client.PacketQueue.Enqueue(new PreChunkPacket(ColumnLocation.X, ColumnLocation.Z));

            MapChunkPacket mcp = new MapChunkPacket();
            mcp.GroundUpContinuous = true;
            mcp.BiomeData = new byte[256];
            mcp.Location = ColumnLocation;

            byte[] blockData = new byte[0];
            byte[] metadata = new byte[0];
            byte[] blockLight = new byte[0];
            byte[] skyLight = new byte[0];

            Vector3 location = ColumnLocation.Clone();
            location.Y = 240;

            ushort mask = 1;
            bool nonAir = true;
            for (int i = 15; i >= 0; i--)
            {
                Chunk c = Server.GetWorld(Client).GetChunk(location - new Vector3(0, i * 16, 0));

                if (c.IsAir)
                    nonAir = false;
                if (nonAir)
                {
                    blockData = blockData.Concat(c.Blocks).ToArray();
                    metadata = metadata.Concat(c.GetMetadata()).ToArray();
                    blockLight = blockLight.Concat(c.GetBlockLight()).ToArray();
                    skyLight = skyLight.Concat(c.GetSkyLight()).ToArray();

                    mcp.PrimaryBitMap |= mask;
                }

                mask <<= 1;
            }

            byte[] columnData = blockData.Concat(metadata).Concat(blockLight).Concat(skyLight).ToArray();

            zLibDeflater.SetInput(columnData);
            zLibDeflater.Finish();
            byte[] compressed = new byte[columnData.Length];
            int l = zLibDeflater.Deflate(compressed);
            zLibDeflater.Reset();

            mcp.Data = new byte[l];
            Array.Copy(compressed, mcp.Data, l);

            Client.PacketQueue.Enqueue(mcp);
            Client.PlayerEntity.LoadedColumns.Add(ColumnLocation);

            for (int i = 15; i >= 0; i--)
            {
                Chunk c = Server.GetWorld(Client).GetChunk(location - new Vector3(0, i * 16, 0));
                foreach (KeyValuePair<Vector3, byte[]> kvp in c.AdditionalData)
                {
                    if (kvp.Value[0] == 63)
                    {
                        Block b = c.GetBlock(kvp.Key);
                        if (b is WallSignBlock)
                            Client.PacketQueue.Enqueue(new UpdateSignPacket(c.Location + kvp.Key,
                                (b as WallSignBlock).Data.Text));
                        if (b is SignPostBlock)
                            Client.PacketQueue.Enqueue(new UpdateSignPacket(c.Location + kvp.Key,
                                (b as SignPostBlock).Data.Text));
                    }
                }
            }
        }

        /// <summary>
        /// Instructs the client to remove the specified column
        /// from memory.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <param name="ColumnLocation">The column location.</param>
        /// <param name="Server">The server.</param>
        /// <remarks></remarks>
        public static void UnloadColumnOnClient(RemoteClient Client, Vector3 ColumnLocation, MultiplayerServer Server)
        {
            Client.PacketQueue.Enqueue(new PreChunkPacket(ColumnLocation.X, ColumnLocation.Z, true));
            // Send an empty chunk at that location (1.2.3 change)
            Client.PacketQueue.Enqueue(new MapChunkPacket(ColumnLocation, true, 0, 0, new byte[0], new byte[0]));
            Client.PlayerEntity.LoadedColumns.Remove(ColumnLocation);
        }
    }
}
