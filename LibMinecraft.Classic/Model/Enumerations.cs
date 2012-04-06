using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibMinecraft.Classic.Model
{
    /// <summary>
    /// Represents the various packets used to communicate
    /// between server and client.
    /// </summary>
    public enum PacketID : byte
    {
        /// <summary>
        /// Sent when the client logs into the server.
        /// </summary>
        Identification = 0x00,
        /// <summary>
        /// Sent to client to make sure they have disconnected.
        /// </summary>
        Ping = 0x01,
        /// <summary>
        /// Sent to client to initate a level transfer.
        /// </summary>
        LevelInitialize = 0x02,
        /// <summary>
        /// Sent to client with the level data and data sent percentage.
        /// </summary>
        LevelDataChunk = 0x03,
        /// <summary>
        /// Sent to signify the level has been transferred.
        /// </summary>
        LevelFinalize = 0x04,
        /// <summary>
        /// Sent to the client to change block client-side
        /// </summary>
        ClientSetBlock = 0x05,
        /// <summary>
        /// Sent to the server to change block server-side
        /// </summary>
        ServerSetBlock = 0x06,
        /// <summary>
        /// Sent to client to spawn specified player.
        /// </summary>
        SpawnPlayer = 0x07,
        /// <summary>
        /// Teleports/Changes Position and Orientation of player.
        /// </summary>
        PositionAndOrientation = 0x08,
        /// <summary>
        /// Teleports/Changes Position and Orientation of player.
        /// </summary>
        PositionAndOrientationUpdate = 0x09,
        /// <summary>
        /// Changes Position of player.
        /// </summary>
        PositionUpdate = 0x0A,
        /// <summary>
        /// Changes Orientation of player.
        /// </summary>
        OrientationUpdate = 0x0B,
        /// <summary>
        /// Sent to client to despawn specified player.
        /// </summary>
        DespawnPlayer = 0x0C,
        /// <summary>
        /// Sent to client/server to show/handle message.
        /// </summary>
        Message = 0x0D,
        /// <summary>
        /// Sent to client to disconnect player.
        /// </summary>
        DisconnectPlayer = 0x0E,
        /// <summary>
        /// Sent to player to enable/disable bedrock placing.
        /// </summary>
        UpdateUserType = 0x0F,
    }

    /// <summary>
    /// Usertypes for packet UpdateUserType
    /// </summary>
    /// <remarks></remarks>
    public enum UserType : byte
    {
        /// <summary>
        /// Normal Players (cannot break bedrock)
        /// </summary>
        Normal = 0x00,
        /// <summary>
        /// OPS (Can break bedrock)
        /// </summary>
        Op = 0x64,
    }
}
