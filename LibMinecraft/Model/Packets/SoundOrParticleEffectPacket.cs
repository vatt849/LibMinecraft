using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibMinecraft.Server;
using LibMinecraft.Client;

namespace LibMinecraft.Model.Packets
{
    /// <summary>
    /// This class contains information for the Sound or Particle Packet
    /// </summary>
    /// <remarks></remarks>
    public class SoundOrParticleEffectPacket : Packet
    {
        /// <summary>
        /// Gets or sets the effect ID.
        /// </summary>
        /// <value>The effect ID.</value>
        /// <remarks></remarks>
        public SoundOrParticleEffect EffectID { get; set; }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        /// <remarks></remarks>
        public Vector3 Location { get; set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        /// <remarks></remarks>
        public int Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundOrParticleEffectPacket"/> class.
        /// </summary>
        /// <remarks></remarks>
        public SoundOrParticleEffectPacket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundOrParticleEffectPacket"/> class.
        /// </summary>
        /// <param name="EffectID">The effect ID.</param>
        /// <param name="Location">The location.</param>
        /// <param name="Data">The data.</param>
        /// <remarks></remarks>
        public SoundOrParticleEffectPacket(SoundOrParticleEffect EffectID, Vector3 Location, int Data)
        {
            this.EffectID = EffectID;
            this.Location = Location;
            this.Data = Data;
        }

        /// <summary>
        /// Gets the packet ID.
        /// </summary>
        /// <remarks></remarks>
        public override PacketID PacketID
        {
            get { return PacketID.SoundOrParticleEffect; }
        }

        /// <summary>
        /// -1 for non-explicit lengths
        /// Includes packet ID
        /// </summary>
        /// <remarks></remarks>
        public override int Length
        {
            get { return 18; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        /// <remarks></remarks>
        public override byte[] Payload
        {
            get 
            {
                return new byte[] { (byte)PacketID }
                    .Concat(MakeInt((int)EffectID))
                    .Concat(MakeInt((int)Location.X))
                    .Concat(new byte[] { (byte)Location.Y })
                    .Concat(MakeInt((int)Location.Z))
                    .Concat(MakeInt(Data)).ToArray();
            }
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Reads the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void ReadPacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(RemoteClient Client)
        {
            Client.TcpClient.GetStream().Write(Payload, 0, Length);
        }

        /// <summary>
        /// Writes the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void WritePacket(MultiplayerClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Server">The server.</param>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerServer Server, RemoteClient Client)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Handles the packet.
        /// </summary>
        /// <param name="Client">The client.</param>
        /// <remarks></remarks>
        public override void HandlePacket(MultiplayerClient Client)
        {
            throw new NotImplementedException();
        }
    }
}
