using Minecraft_Realms_Emulator.Responses;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace Minecraft_Realms_Emulator.Helpers
{
    public class MinecraftServerQuery
    {
        public MinecraftServerQueryRepsonse? Query(string address)
        {
            var server = address.Split(':')[0];
            var port = address.Contains(':') ? int.Parse(address.Split(':')[1]) : 25565;

            try
            {
                using TcpClient client = new();
                client.Connect(server, port);

                using var stream = client.GetStream();
                using BinaryWriter writer = new(stream);
                using BinaryReader reader = new(stream);
                // Send Handshake packet (https://wiki.vg/Protocol#Handshake)
                var handshakePacket = CreateHandshakePacket(server, port);
                writer.Write((byte)handshakePacket.Length); // Packet length
                writer.Write(handshakePacket); // Packet data

                // Send Status Request packet (https://wiki.vg/Protocol#Request)
                writer.Write((byte)0x01); // Packet length
                writer.Write((byte)0x00); // Packet ID (Request)

                // Read the response packet ID
                var packetId = ReadVarInt(reader);
                if (packetId != 0x00) throw new Exception("Invalid packet ID");

                // Read the JSON length
                var jsonLength = ReadVarInt(reader);
                // Read the JSON response
                var jsonData = reader.ReadBytes(jsonLength);
                var json = Encoding.UTF8.GetString(jsonData);

                return JsonConvert.DeserializeObject<MinecraftServerQueryRepsonse>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        private static byte[] CreateHandshakePacket(string server, int port)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            WriteVarInt(writer, 0x00); // Packet ID (Handshake)
            WriteVarInt(writer, 754); // Protocol version (754 for Minecraft 1.16.5)
            WriteVarInt(writer, server.Length); // Server address length
            writer.Write(Encoding.UTF8.GetBytes(server)); // Server address
            writer.Write((ushort)port); // Server port
            WriteVarInt(writer, 1); // Next state (1 for status)

            return ms.ToArray();
        }

        private static int ReadVarInt(BinaryReader reader)
        {
            var value = 0;
            var size = 0;
            int b;
            while (((b = reader.ReadByte()) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 5) throw new Exception("VarInt is too big");
            }

            return value | (b << (size * 7));
        }

        private static void WriteVarInt(BinaryWriter writer, int value)
        {
            while ((value & 0xFFFFFF80) != 0)
            {
                writer.Write((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }

            writer.Write((byte)(value & 0x7F));
        }
    }
}
