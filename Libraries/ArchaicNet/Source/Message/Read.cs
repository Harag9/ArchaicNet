using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
namespace ArchaicNet
{
    public partial class Message
    {
        public void Read(ref byte[] bytes, int length)
        {
            if (Location + length > Data.Length)
                return;
            bytes = new byte[length];
            Buffer.BlockCopy(Data, Location, bytes, 0, bytes.Length);
            Location += length;
        }
        public byte[] ReadBytes(int length)
        {
            if (Location + length > Data.Length)
                return new byte[0];
            var bytes = new byte[length];
            Buffer.BlockCopy(Data, Location, bytes, 0, bytes.Length);
            Location += length;
            return bytes;
        }
        public void Read(ref bool boolean)
        {
            if (Location + 1 > Data.Length)
                return;
            boolean = BitConverter.ToBoolean(Data, Location);
            Location++;
        }
        public bool ReadBool
        {
            get
            {
                if (Location + 1 > Data.Length)
                    return false;
                var boolean = BitConverter.ToBoolean(Data, Location);
                Location++;
                return boolean;
            }
        }
        public void Read(ref byte Byte)
        {
            if (Location + 1 > Data.Length)
                return;
            Byte = Data[Location];
            Location++;
        }
        public byte ReadByte
        {
            get
            {
                if (Location + 1 > Data.Length)
                    return 0;
                var Byte = Data[Location];
                Location++;
                return Byte;
            }
        }
        public void Read(ref sbyte sByte)
        {
            if (Location + 1 > Data.Length)
                return;
            sByte = (sbyte)(Data[Location] * -1);
            Location++;
        }
        public sbyte ReadSByte
        {
            get
            {
                if (Location + 1 > Data.Length)
                    return 0;
                var sByte = (sbyte)(Data[Location] * -1);
                Location++;
                return sByte;
            }
        }
        public void Read(ref short Short)
        {
            if (Location + 2 > Data.Length)
                return;
            Short = BitConverter.ToInt16(Data, Location);
            Location += 2;
        }
        public short ReadShort
        {
            get
            {
                if (Location + 2 > Data.Length)
                    return 0;
                var Short = BitConverter.ToInt16(Data, Location);
                Location += 2;
                return Short;
            }
        }
        public void Read(ref ushort uShort)
        {
            if (Location + 2 > Data.Length)
                return;
            uShort = BitConverter.ToUInt16(Data, Location);
            Location += 2;
        }
        public ushort ReadUShort
        {
            get
            {
                if (Location + 2 > Data.Length)
                    return 0;
                var uShort = BitConverter.ToUInt16(Data, Location);
                Location += 2;
                return uShort;
            }
        }
        public void Read(ref int integer)
        {
            if (Location + 4 > Data.Length)
                return;
            integer = BitConverter.ToInt32(Data, Location);
            Location += 4;
        }
        public int ReadInteger
        {
            get
            {
                if (Location + 4 > Data.Length)
                    return 0;
                var integer = BitConverter.ToInt32(Data, Location);
                Location += 4;
                return integer;
            }
        }
        public void Read(ref uint uInteger)
        {
            if (Location + 4 > Data.Length)
                return;
            uInteger = BitConverter.ToUInt32(Data, Location);
            Location += 4;
        }
        public uint ReadUInteger
        {
            get
            {
                if (Location + 4 > Data.Length)
                    return 0;
                var uInteger = BitConverter.ToUInt32(Data, Location);
                Location += 4;
                return uInteger;
            }
        }
        public void Read(ref float Float)
        {
            if (Location + 4 > Data.Length)
                return;
            Float = BitConverter.ToSingle(Data, Location);
            Location += 4;
        }
        public float ReadFloat
        {
            get
            {
                if (Location + 4 > Data.Length)
                    return 0.0f;
                var Float = BitConverter.ToSingle(Data, Location);
                Location += 4;
                return Float;
            }
        }
        public float ReadSingle
        {
            get
            {
                if (Location + 4 > Data.Length)
                    return 0.0f;
                var single = BitConverter.ToSingle(Data, Location);
                Location += 4;
                return single;
            }
        }
        public void Read(ref long Long)
        {
            if (Location + 8 > Data.Length)
                return;
            Long = BitConverter.ToInt64(Data, Location);
            Location += 8;
        }
        public long ReadLong
        {
            get
            {
                if (Location + 8 > Data.Length)
                    return 0;
                var Long = BitConverter.ToInt64(Data, Location);
                Location += 8;
                return Long;
            }
        }
        public void Read(ref ulong uLong)
        {
            if (Location + 8 > Data.Length)
                return;
            uLong = BitConverter.ToUInt64(Data, Location);
            Location += 8;
        }
        public ulong ReadULong
        {
            get
            {
                if (Location + 8 > Data.Length)
                    return 0;
                var uLong = BitConverter.ToUInt64(Data, Location);
                Location += 8;
                return uLong;
            }
        }
        public void Read(ref double Double)
        {
            if (Location + 8 > Data.Length)
                return;
            Double = BitConverter.ToDouble(Data, Location);
            Location += 8;
        }
        public double ReadDouble
        {
            get
            {
                if (Location + 8 > Data.Length)
                    return 0.0d;
                var Double = BitConverter.ToDouble(Data, Location);
                Location += 8;
                return Double;
            }
        }
        public void Read(ref char character)
        {
            if (Location + 2 > Data.Length)
                return;
            character = BitConverter.ToChar(Data, Location);
            Location += 2;
        }
        public char ReadChar
        {
            get
            {
                if (Location + 2 > Data.Length)
                    return '\0';
                var character = BitConverter.ToChar(Data, Location);
                Location += 2;
                return character;
            }
        }
        public void Read(ref string String)
        {
            if (Location + 4 > Data.Length)
                return;
            var len = BitConverter.ToInt32(Data, Location);
            Location += 4;
            if (Location + len > Data.Length)
                return;
            String = Encoding.ASCII.GetString(Data, Location, len);
            Location += len;
        }
        public string ReadString
        {
            get
            {
                if (Location + 4 > Data.Length)
                    return string.Empty;
                var len = BitConverter.ToInt32(Data, Location);
                Location += 4;
                if (Location + len > Data.Length)
                    return string.Empty;
                var String = Encoding.ASCII.GetString(Data, Location, len);
                Location += len;
                return String;
            }
        }
        public void Read(ref object Object)
        {
            if (Location + 4 > Data.Length)
                return;
            var len = BitConverter.ToInt32(Data, Location);
            Location += 4;
            if (Location + len > Data.Length)
                return;
            var memoryStream = new MemoryStream();
            memoryStream.SetLength(len);
            memoryStream.Read(Data, Location, len);
            Location += len;
            Object = new BinaryFormatter().Deserialize(memoryStream);
            memoryStream.Dispose();
        }
        public object ReadObject
        {
            get
            {
                if (Location + 4 > Data.Length)
                    return null;
                var len = BitConverter.ToInt32(Data, Location);
                Location += 4;
                if (Location + len > Data.Length)
                    return null;
                var memoryStream = new MemoryStream();
                memoryStream.SetLength(len);
                memoryStream.Read(Data, Location, len);
                Location += len;
                var Object = new BinaryFormatter().Deserialize(memoryStream);
                memoryStream.Dispose();
                return Object;
            }
        }
    }
}