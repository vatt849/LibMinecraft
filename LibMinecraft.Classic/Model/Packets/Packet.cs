using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using LibMinecraft.Classic.Server;
using LibMinecraft.Classic.Client;

namespace LibMinecraft.Classic.Model.Packets
{
    /// <summary>
    /// The abstract model for LibMinecraft Classic packets
    /// </summary>
    public abstract class Packet
    {
        #region Packets

        /// <summary>
        /// A full list of packets.
        /// </summary>
        private static Type[] Packets = new Type[] {
            typeof(IdentificationPacket), // 0x00
            typeof(PingPacket), // 0x01
            typeof(WorldInitializePacket), // 0x02
            typeof(WorldDataChunkPacket), // 0x03
            typeof(WorldFinalizePacket), // 0x04
            typeof(ClientSetBlockPacket), // 0x05
            typeof(ServerSetBlockPacket), // 0x06
            typeof(SpawnPlayerPacket), // 0x07
            typeof(PositionAndOrientationPacket), // 0x08
            typeof(InvalidPacket), // 0x09
            typeof(InvalidPacket), // 0x0a
            typeof(InvalidPacket), // 0x0b
            typeof(DespawnPlayerPacket), // 0x0c
            typeof(MessagePacket), // 0x0d
            typeof(DisconnectPlayerPacket), // 0x0e
            typeof(InvalidPacket), // 0x0f
            typeof(InvalidPacket), // 0x10
            typeof(InvalidPacket), // 0x11
            typeof(InvalidPacket), // 0x12
            typeof(InvalidPacket), // 0x13
            typeof(InvalidPacket), // 0x14
            typeof(InvalidPacket), // 0x15
            typeof(InvalidPacket), // 0x16
            typeof(InvalidPacket), // 0x17
            typeof(InvalidPacket), // 0x18
            typeof(InvalidPacket), // 0x19
            typeof(InvalidPacket), // 0x1a
            typeof(InvalidPacket), // 0x1b
            typeof(InvalidPacket), // 0x1c
            typeof(InvalidPacket), // 0x1d
            typeof(InvalidPacket), // 0x1e
            typeof(InvalidPacket), // 0x1f
            typeof(InvalidPacket), // 0x20
            typeof(InvalidPacket), // 0x21
            typeof(InvalidPacket), // 0x22
            typeof(InvalidPacket), // 0x23
            typeof(InvalidPacket), // 0x24
            typeof(InvalidPacket), // 0x25
            typeof(InvalidPacket), // 0x26
            typeof(InvalidPacket), // 0x27
            typeof(InvalidPacket), // 0x28
            typeof(InvalidPacket), // 0x29
            typeof(InvalidPacket), // 0x2a
            typeof(InvalidPacket), // 0x2b
            typeof(InvalidPacket), // 0x2c
            typeof(InvalidPacket), // 0x2d
            typeof(InvalidPacket), // 0x2e
            typeof(InvalidPacket), // 0x2f
            typeof(InvalidPacket), // 0x30
            typeof(InvalidPacket), // 0x31
            typeof(InvalidPacket), // 0x32
            typeof(InvalidPacket), // 0x33
            typeof(InvalidPacket), // 0x34
            typeof(InvalidPacket), // 0x35
            typeof(InvalidPacket), // 0x36
            typeof(InvalidPacket), // 0x37
            typeof(InvalidPacket), // 0x38
            typeof(InvalidPacket), // 0x39
            typeof(InvalidPacket), // 0x3a
            typeof(InvalidPacket), // 0x3b
            typeof(InvalidPacket), // 0x3c
            typeof(InvalidPacket), // 0x3d
            typeof(InvalidPacket), // 0x3e
            typeof(InvalidPacket), // 0x3f
            typeof(InvalidPacket), // 0x40
            typeof(InvalidPacket), // 0x41
            typeof(InvalidPacket), // 0x42
            typeof(InvalidPacket), // 0x43
            typeof(InvalidPacket), // 0x44
            typeof(InvalidPacket), // 0x45
            typeof(InvalidPacket), // 0x46
            typeof(InvalidPacket), // 0x47
            typeof(InvalidPacket), // 0x48
            typeof(InvalidPacket), // 0x49
            typeof(InvalidPacket), // 0x4a
            typeof(InvalidPacket), // 0x4b
            typeof(InvalidPacket), // 0x4c
            typeof(InvalidPacket), // 0x4d
            typeof(InvalidPacket), // 0x4e
            typeof(InvalidPacket), // 0x4f
            typeof(InvalidPacket), // 0x50
            typeof(InvalidPacket), // 0x51
            typeof(InvalidPacket), // 0x52
            typeof(InvalidPacket), // 0x53
            typeof(InvalidPacket), // 0x54
            typeof(InvalidPacket), // 0x55
            typeof(InvalidPacket), // 0x56
            typeof(InvalidPacket), // 0x57
            typeof(InvalidPacket), // 0x58
            typeof(InvalidPacket), // 0x59
            typeof(InvalidPacket), // 0x5a
            typeof(InvalidPacket), // 0x5b
            typeof(InvalidPacket), // 0x5c
            typeof(InvalidPacket), // 0x5d
            typeof(InvalidPacket), // 0x5e
            typeof(InvalidPacket), // 0x5f
            typeof(InvalidPacket), // 0x60
            typeof(InvalidPacket), // 0x61
            typeof(InvalidPacket), // 0x62
            typeof(InvalidPacket), // 0x63
            typeof(InvalidPacket), // 0x64
            typeof(InvalidPacket), // 0x65
            typeof(InvalidPacket), // 0x66
            typeof(InvalidPacket), // 0x67
            typeof(InvalidPacket), // 0x68
            typeof(InvalidPacket), // 0x69
            typeof(InvalidPacket), // 0x6a
            typeof(InvalidPacket), // 0x6b
            typeof(InvalidPacket), // 0x6c
            typeof(InvalidPacket), // 0x6d
            typeof(InvalidPacket), // 0x6e
            typeof(InvalidPacket), // 0x6f
            typeof(InvalidPacket), // 0x70
            typeof(InvalidPacket), // 0x71
            typeof(InvalidPacket), // 0x72
            typeof(InvalidPacket), // 0x73
            typeof(InvalidPacket), // 0x74
            typeof(InvalidPacket), // 0x75
            typeof(InvalidPacket), // 0x76
            typeof(InvalidPacket), // 0x77
            typeof(InvalidPacket), // 0x78
            typeof(InvalidPacket), // 0x79
            typeof(InvalidPacket), // 0x7a
            typeof(InvalidPacket), // 0x7b
            typeof(InvalidPacket), // 0x7c
            typeof(InvalidPacket), // 0x7d
            typeof(InvalidPacket), // 0x7e
            typeof(InvalidPacket), // 0x7f
            typeof(InvalidPacket), // 0x80
            typeof(InvalidPacket), // 0x81
            typeof(InvalidPacket), // 0x82
            typeof(InvalidPacket), // 0x83
            typeof(InvalidPacket), // 0x84
            typeof(InvalidPacket), // 0x85
            typeof(InvalidPacket), // 0x86
            typeof(InvalidPacket), // 0x87
            typeof(InvalidPacket), // 0x88
            typeof(InvalidPacket), // 0x89
            typeof(InvalidPacket), // 0x8a
            typeof(InvalidPacket), // 0x8b
            typeof(InvalidPacket), // 0x8c
            typeof(InvalidPacket), // 0x8d
            typeof(InvalidPacket), // 0x8e
            typeof(InvalidPacket), // 0x8f
            typeof(InvalidPacket), // 0x90
            typeof(InvalidPacket), // 0x91
            typeof(InvalidPacket), // 0x92
            typeof(InvalidPacket), // 0x93
            typeof(InvalidPacket), // 0x94
            typeof(InvalidPacket), // 0x95
            typeof(InvalidPacket), // 0x96
            typeof(InvalidPacket), // 0x97
            typeof(InvalidPacket), // 0x98
            typeof(InvalidPacket), // 0x99
            typeof(InvalidPacket), // 0x9a
            typeof(InvalidPacket), // 0x9b
            typeof(InvalidPacket), // 0x9c
            typeof(InvalidPacket), // 0x9d
            typeof(InvalidPacket), // 0x9e
            typeof(InvalidPacket), // 0x9f
            typeof(InvalidPacket), // 0xa0
            typeof(InvalidPacket), // 0xa1
            typeof(InvalidPacket), // 0xa2
            typeof(InvalidPacket), // 0xa3
            typeof(InvalidPacket), // 0xa4
            typeof(InvalidPacket), // 0xa5
            typeof(InvalidPacket), // 0xa6
            typeof(InvalidPacket), // 0xa7
            typeof(InvalidPacket), // 0xa8
            typeof(InvalidPacket), // 0xa9
            typeof(InvalidPacket), // 0xaa
            typeof(InvalidPacket), // 0xab
            typeof(InvalidPacket), // 0xac
            typeof(InvalidPacket), // 0xad
            typeof(InvalidPacket), // 0xae
            typeof(InvalidPacket), // 0xaf
            typeof(InvalidPacket), // 0xb0
            typeof(InvalidPacket), // 0xb1
            typeof(InvalidPacket), // 0xb2
            typeof(InvalidPacket), // 0xb3
            typeof(InvalidPacket), // 0xb4
            typeof(InvalidPacket), // 0xb5
            typeof(InvalidPacket), // 0xb6
            typeof(InvalidPacket), // 0xb7
            typeof(InvalidPacket), // 0xb8
            typeof(InvalidPacket), // 0xb9
            typeof(InvalidPacket), // 0xba
            typeof(InvalidPacket), // 0xbb
            typeof(InvalidPacket), // 0xbc
            typeof(InvalidPacket), // 0xbd
            typeof(InvalidPacket), // 0xbe
            typeof(InvalidPacket), // 0xbf
            typeof(InvalidPacket), // 0xc0
            typeof(InvalidPacket), // 0xc1
            typeof(InvalidPacket), // 0xc2
            typeof(InvalidPacket), // 0xc3
            typeof(InvalidPacket), // 0xc4
            typeof(InvalidPacket), // 0xc5
            typeof(InvalidPacket), // 0xc6
            typeof(InvalidPacket), // 0xc7
            typeof(InvalidPacket), // 0xc8
            typeof(InvalidPacket), // 0xc9
            typeof(InvalidPacket), // 0xca
            typeof(InvalidPacket), // 0xcb
            typeof(InvalidPacket), // 0xcc
            typeof(InvalidPacket), // 0xcd
            typeof(InvalidPacket), // 0xce
            typeof(InvalidPacket), // 0xcf
            typeof(InvalidPacket), // 0xd0
            typeof(InvalidPacket), // 0xd1
            typeof(InvalidPacket), // 0xd2
            typeof(InvalidPacket), // 0xd3
            typeof(InvalidPacket), // 0xd4
            typeof(InvalidPacket), // 0xd5
            typeof(InvalidPacket), // 0xd6
            typeof(InvalidPacket), // 0xd7
            typeof(InvalidPacket), // 0xd8
            typeof(InvalidPacket), // 0xd9
            typeof(InvalidPacket), // 0xda
            typeof(InvalidPacket), // 0xdb
            typeof(InvalidPacket), // 0xdc
            typeof(InvalidPacket), // 0xdd
            typeof(InvalidPacket), // 0xde
            typeof(InvalidPacket), // 0xdf
            typeof(InvalidPacket), // 0xe0
            typeof(InvalidPacket), // 0xe1
            typeof(InvalidPacket), // 0xe2
            typeof(InvalidPacket), // 0xe3
            typeof(InvalidPacket), // 0xe4
            typeof(InvalidPacket), // 0xe5
            typeof(InvalidPacket), // 0xe6
            typeof(InvalidPacket), // 0xe7
            typeof(InvalidPacket), // 0xe8
            typeof(InvalidPacket), // 0xe9
            typeof(InvalidPacket), // 0xea
            typeof(InvalidPacket), // 0xeb
            typeof(InvalidPacket), // 0xec
            typeof(InvalidPacket), // 0xed
            typeof(InvalidPacket), // 0xee
            typeof(InvalidPacket), // 0xef
            typeof(InvalidPacket), // 0xf0
            typeof(InvalidPacket), // 0xf1
            typeof(InvalidPacket), // 0xf2
            typeof(InvalidPacket), // 0xf3
            typeof(InvalidPacket), // 0xf4
            typeof(InvalidPacket), // 0xf5
            typeof(InvalidPacket), // 0xf6
            typeof(InvalidPacket), // 0xf7
            typeof(InvalidPacket), // 0xf8
            typeof(InvalidPacket), // 0xf9
            typeof(InvalidPacket), // 0xfa
            typeof(InvalidPacket), // 0xfb
            typeof(InvalidPacket), // 0xfc
            typeof(InvalidPacket), // 0xfd
            typeof(InvalidPacket), // 0xfe
            typeof(InvalidPacket), // 0xff
        };

        #endregion

        /// <summary>
        /// Gets a Packet for the specified PacketID.  If IsServer
        /// is set to false, it assumes that the packet is being
        /// handled by a client.
        /// </summary>
        public static Packet GetPacket(PacketID PacketID, bool IsServer = true)
        {
            byte id = (byte)PacketID;
            Type t = Packets[id];
            Packet p = (Packet)Activator.CreateInstance(Packets[(byte)PacketID]);
            p.IsServerContext = IsServer;
            return p;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        public abstract PacketID PacketID { get; }

        protected bool IsServerContext { get; set; }

        /// <summary>
        /// -1 for non-explicit lengths 
        /// Includes packet ID
        /// </summary>
        public abstract int Length { get; }


        public abstract byte[] Payload { get; }

        /// <summary>
        /// Reads a packet from a RemoteClient.
        /// </summary>
        /// <param name="Client">The sender of said packet.</param>
        public abstract void ReadPacket(RemoteClient Client);

        /// <summary>
        /// Reads an incoming packet from a ClassicClient
        /// </summary>
        /// <param name="Client">The sender of said packet.</param>
        public abstract void ReadPacket(ClassicClient Client);

        /// <summary>
        /// Writes a packet to a RemoteClient.
        /// </summary>
        /// <param name="Client">The RemoteClient to be written to.</param>
        public abstract void WritePacket(RemoteClient Client);

        /// <summary>
        /// Writes a packet to a ClassicClient
        /// </summary>
        /// <param name="Client">The client that the packet will be written or sent to.</param>
        public abstract void WritePacket(ClassicClient Client);

        /// <summary>
        /// Deals with an incoming packet from a RemoteClient/ClassicServer
        /// </summary>
        /// <param name="Server">The server involved</param>
        /// <param name="Client">The RemoteClient involved</param>
        public abstract void HandlePacket(ClassicServer Server, RemoteClient Client);

        /// <summary>
        /// Deals with a packet handing a ClassicClient rather than a RemoteClient.
        /// </summary>
        /// <param name="Client">The ClassicClient sender.</param>
        public abstract void HandlePacket(ClassicClient Client);

        #region Static Helpers

        public static byte[] MakeString(String msg)
        {
            if (msg.Length > 64)
                msg = msg.Substring(64);
            byte[] a = new byte[64];
            for (int i = 0; i < 64; i++)
                a[i] = 0x20;
            Array.Copy(Encoding.ASCII.GetBytes(msg), a, msg.Length);
            return a;
        }

        internal static byte[] MakeDouble(double i)
        {
            short s = (short)(i * 32);
            return MakeShort(s);
        }

        public static byte[] MakeLong(long i)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
        }

        public static byte[] MakeShort(short i)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
        }

        static byte[] BooleanArray = new byte[] { 0 };
        public static byte[] MakeBoolean(Boolean b)
        {
            BooleanArray[0] = (byte)(b ? 1 : 0);
            return BooleanArray;
        }

        public static short ReadShort(Stream s)
        {
            byte[] a = new byte[2];
            s.Read(a, 0, 2);
            return IPAddress.HostToNetworkOrder(BitConverter.ToInt16(a, 0));
        }

        public static Boolean ReadBoolean(Stream s)
        {
            return new BinaryReader(s).ReadBoolean();
        }

        public static String ReadString(Stream s)
        {
            byte[] a = new byte[64];
            s.Read(a, 0, 64);
            return Encoding.ASCII.GetString(a, 0, 64).TrimEnd();
        }

        #endregion
    }
}
