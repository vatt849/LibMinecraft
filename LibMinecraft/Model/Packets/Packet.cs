using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using System.Net;
using System.IO;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// Represents an abstract packet and handles some packet logic.
    /// </summary>
    /// <remarks></remarks>
    public abstract class Packet
    {
        #region Packets

        private static Type[] Packets = new Type[] {
            typeof(KeepAlivePacket), // 0x00
            typeof(LoginRequestPacket), // 0x01
            typeof(HandshakePacket), // 0x02
            typeof(ChatMessagePacket), // 0x03
            typeof(TimeUpdatePacket), // 0x04
            typeof(EntityEquipmentPacket), // 0x05 
            typeof(SpawnPositionPacket), // 0x06
            typeof(UseEntityPacket), // 0x07
            typeof(HealthUpdatePacket), // 0x08
            typeof(RespawnPacket), // 0x09
            typeof(PlayerPacket), // 0x0a
            typeof(PlayerPositionPacket), // 0x0b
            typeof(PlayerLookPacket), // 0x0c
            typeof(PlayerPositionAndLookPacket), // 0x0d
            typeof(PlayerDiggingPacket), // 0x0e
            typeof(PlayerBlockPlacementPacket), // 0x0f
            typeof(HoldingChangePacket), // 0x10
            typeof(InvalidPacket), // 0x11
            typeof(AnimationPacket), // 0x12
            typeof(EntityActionPacket), // 0x13
            typeof(NamedEntitySpawnPacket), // 0x14
            typeof(InvalidPacket), // 0x15
            typeof(InvalidPacket), // 0x16
            typeof(AddObjectOrVehicleEntityPacket), // 0x17
            typeof(InvalidPacket), // 0x18
            typeof(InvalidPacket), // 0x19
            typeof(InvalidPacket), // 0x1a
            typeof(InvalidPacket), // 0x1b
            typeof(EntityVelocityPacket), // 0x1c
            typeof(DestroyEntityPacket), // 0x1d
            typeof(EntityPacket), // 0x1e
            typeof(EntityRelativeMovePacket), // 0x1f
            typeof(EntityLookPacket), // 0x20
            typeof(EntityLookAndRelativeMovePacket), // 0x21
            typeof(EntityTeleportPacket), // 0x22
            typeof(EntityHeadLookPacket), // 0x23
            typeof(InvalidPacket), // 0x24
            typeof(InvalidPacket), // 0x25
            typeof(EntityStatusPacket), // 0x26
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
            typeof(PreChunkPacket), // 0x32
            typeof(MapChunkPacket), // 0x33
            typeof(InvalidPacket), // 0x34
            typeof(BlockChangePacket), // 0x35
            typeof(InvalidPacket), // 0x36
            typeof(InvalidPacket), // 0x37
            typeof(InvalidPacket), // 0x38
            typeof(InvalidPacket), // 0x39
            typeof(InvalidPacket), // 0x3a
            typeof(InvalidPacket), // 0x3b
            typeof(InvalidPacket), // 0x3c
            typeof(SoundOrParticleEffectPacket), // 0x3d
            typeof(InvalidPacket), // 0x3e
            typeof(InvalidPacket), // 0x3f
            typeof(InvalidPacket), // 0x40
            typeof(InvalidPacket), // 0x41
            typeof(InvalidPacket), // 0x42
            typeof(InvalidPacket), // 0x43
            typeof(InvalidPacket), // 0x44
            typeof(InvalidPacket), // 0x45
            typeof(InvalidPacket), // 0x46
            typeof(ThunderboltPacket), // 0x47
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
            typeof(CloseWindowPacket), // 0x65
            typeof(InvalidPacket), // 0x66
            typeof(SetSlotPacket), // 0x67
            typeof(InvalidPacket), // 0x68
            typeof(InvalidPacket), // 0x69
            typeof(InvalidPacket), // 0x6a
            typeof(CreativeInventoryActionPacket), // 0x6b
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
            typeof(UpdateSignPacket), // 0x82
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
            typeof(PlayerListItemPacket), // 0xc9
            typeof(PlayerAbilitiesPacket), // 0xca
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
            typeof(ServerListPingPacket), // 0xfe
            typeof(DisconnectPacket), // 0xff
        };

        #endregion

        /// <summary>
        /// Gets a Packet for the specified PacketID.  If IsServer
        /// is set to false, it assumes that the packet is being
        /// handled by a client.
        /// </summary>
        /// <param name="PacketID">The packet ID.</param>
        /// <param name="IsServer">if set to <c>true</c> [is server].</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Packet GetPacket(PacketID PacketID, bool IsServer = true)
        {
            Packet p = (Packet)Activator.CreateInstance(Packets[(byte)PacketID]);
            p.IsServerContext = IsServer;
            return p;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public abstract PacketID PacketID { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is server context.
        /// </summary>
        /// <value><c>true</c> if this instance is server context; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        protected bool IsServerContext { get; set; }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public abstract int Length { get; }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public abstract byte[] Payload { get; }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public abstract void ReadPacket(RemoteClient Client);

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public abstract void ReadPacket(MultiplayerClient Client);

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public abstract void WritePacket(RemoteClient Client);

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public abstract void WritePacket(MultiplayerClient Client);

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public abstract void HandlePacket(MultiplayerServer Server, RemoteClient Client);

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public abstract void HandlePacket(MultiplayerClient Client);

        #region Static Helpers

        /// <summary>
        /// Strings the length.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected static int StringLength(string str)
        {
            return 2 + str.Length * 2;
        }

        /// <summary>
        /// Makes the vector3 int.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected static byte[] MakeVector3Int(Vector3 vector)
        {
            return MakeInt((int)vector.X).Concat(MakeInt((int)vector.Y).Concat(MakeInt((int)vector.Z))).ToArray();
        }

        /// <summary>
        /// Makes the string.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] MakeString(String msg)
        {
            short len = IPAddress.HostToNetworkOrder((short)msg.Length);
            byte[] a = BitConverter.GetBytes(len);
            byte[] b = Encoding.BigEndianUnicode.GetBytes(msg);
            return a.Concat(b).ToArray();
        }

        /// <summary>
        /// Makes the int.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] MakeInt(int i)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
        }

        /// <summary>
        /// Makes the absolute int.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] MakeAbsoluteInt(double i)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)(i * 32.0)));
        }

        /// <summary>
        /// Makes the long.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] MakeLong(long i)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
        }

        /// <summary>
        /// Makes the short.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] MakeShort(short i)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
        }

        public static byte[] MakeUShort(ushort i)
        {
            return MakeShort((short)i);
        }

        /// <summary>
        /// Makes the double.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] MakeDouble(double d)
        {
            byte[] b = BitConverter.GetBytes(d);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return b;
        }

        /// <summary>
        /// Makes the float.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] MakeFloat(float f)
        {
            byte[] b = BitConverter.GetBytes(f);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return b;
        }

        /// <summary>
        /// Makes the packed byte.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte MakePackedByte(float f)
        {
            return (byte)(((Math.Floor(f) % 360) / 360) * 256);
        }

        /// <summary>
        /// 
        /// </summary>
        static byte[] BooleanArray = new byte[] { 0 };
        /// <summary>
        /// Makes the boolean.
        /// </summary>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] MakeBoolean(Boolean b)
        {
            BooleanArray[0] = (byte)(b ? 1 : 0);
            return BooleanArray;
        }

        /// <summary>
        /// Reads the int.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int ReadInt(Stream s)
        {
            return IPAddress.HostToNetworkOrder((int)Read(s, 4));
        }

        /// <summary>
        /// Reads the short.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static short ReadShort(Stream s)
        {
            return IPAddress.HostToNetworkOrder((short)Read(s, 2));
        }

        /// <summary>
        /// Reads the long.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static long ReadLong(Stream s)
        {
            return IPAddress.HostToNetworkOrder((long)Read(s, 8));
        }

        /// <summary>
        /// Reads the double.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static double ReadDouble(Stream s)
        {
            byte[] doubleArray = new byte[sizeof(double)];
            s.Read(doubleArray, 0, sizeof(double));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(doubleArray);
            return BitConverter.ToDouble(doubleArray, 0);
        }

        /// <summary>
        /// Reads the float.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static unsafe float ReadFloat(Stream s)
        {
            byte[] floatArray = new byte[sizeof(int)];
            s.Read(floatArray, 0, sizeof(int));
            if (BitConverter.IsLittleEndian)
                Array.Reverse(floatArray);
            int i = BitConverter.ToInt32(floatArray, 0);
            return *(float*)&i;
        }

        /// <summary>
        /// Reads the boolean.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Boolean ReadBoolean(Stream s)
        {
            return new BinaryReader(s).ReadBoolean();
        }

        /// <summary>
        /// Reads the bytes.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] ReadBytes(Stream s, int count)
        {
            return new BinaryReader(s).ReadBytes(count);
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static String ReadString(Stream s)
        {
            short len;
            byte[] a = new byte[2];
            a[0] = (byte)s.ReadByte();
            a[1] = (byte)s.ReadByte();
            len = IPAddress.HostToNetworkOrder(BitConverter.ToInt16(a, 0));
            byte[] b = new byte[len * 2];
            for (int i = 0; i < len * 2; i++)
            {
                b[i] = (byte)s.ReadByte();
            }
            return Encoding.BigEndianUnicode.GetString(b);
        }

        /// <summary>
        /// Reads the vector3.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected Vector3 ReadVector3(Stream s)
        {
            return new Vector3(ReadInt(s), ReadInt(s), ReadInt(s));
        }

        /// <summary>
        /// Writes the vector3.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="vector">The vector.</param>
        /// <remarks></remarks>
        protected void WriteVector3(Stream s, Vector3 vector)
        {
            WriteInt(s, (int)vector.X);
            WriteInt(s, (int)vector.Y);
            WriteInt(s, (int)vector.Z);
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="msg">The MSG.</param>
        /// <remarks></remarks>
        public static void WriteString(Stream s, String msg)
        {

            short len = IPAddress.HostToNetworkOrder((short)msg.Length);
            byte[] a = BitConverter.GetBytes(len);
            byte[] b = Encoding.BigEndianUnicode.GetBytes(msg);
            byte[] c = a.Concat(b).ToArray();
            s.Write(c, 0, c.Length);
        }

        /// <summary>
        /// Writes the int.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="i">The i.</param>
        /// <remarks></remarks>
        public static void WriteInt(Stream s, int i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        /// <summary>
        /// Writes the long.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="i">The i.</param>
        /// <remarks></remarks>
        public static void WriteLong(Stream s, long i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        /// <summary>
        /// Writes the short.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="i">The i.</param>
        /// <remarks></remarks>
        public static void WriteShort(Stream s, short i)
        {
            byte[] a = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(i));
            s.Write(a, 0, a.Length);
        }

        /// <summary>
        /// Writes the double.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="d">The d.</param>
        /// <remarks></remarks>
        public static void WriteDouble(Stream s, double d)
        {
            byte[] doubleArray = BitConverter.GetBytes(d);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(doubleArray);
            s.Write(doubleArray, 0, sizeof(double));
        }

        /// <summary>
        /// Writes the float.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="f">The f.</param>
        /// <remarks></remarks>
        public static void WriteFloat(Stream s, float f)
        {
            byte[] floatArray = BitConverter.GetBytes(f);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(floatArray);
            s.Write(floatArray, 0, sizeof(float));
        }

        /// <summary>
        /// Writes the boolean.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <remarks></remarks>
        public static void WriteBoolean(Stream s, Boolean b)
        {
            new BinaryWriter(s).Write(b);
        }

        /// <summary>
        /// Writes the bytes.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="b">The b.</param>
        /// <remarks></remarks>
        public static void WriteBytes(Stream s, byte[] b)
        {
            new BinaryWriter(s).Write(b);
        }

        /// <summary>
        /// Reads the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="num">The num.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Object Read(Stream s, int num)
        {
            byte[] b = new byte[num];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)s.ReadByte();
            }
            switch (num)
            {
                case 4:
                    return BitConverter.ToInt32(b, 0);
                case 8:
                    return BitConverter.ToInt64(b, 0);
                case 2:
                    return BitConverter.ToInt16(b, 0);
                default:
                    return 0;
            }
        }

        #endregion
    }
}
