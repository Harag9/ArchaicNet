using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
namespace ArchaicNet
{
    public partial class Message
    {
        public void Write(byte[] bytes, int offset, int size)
        {
            AddToBuffer(bytes, offset, size);
        }
        public void WriteBytes(byte[] bytes, int offset, int size)
        {
            AddToBuffer(bytes, offset, size);
        }
        public void Write(byte[] bytes)
        {
            if (Data.Length - Location <= bytes.Length)
            {
                var temp = new byte[Data.Length + bytes.Length];
                Buffer.BlockCopy(Data, 0, temp, 0, Data.Length);
                Buffer.BlockCopy(bytes, 0, temp, Data.Length, bytes.Length);
                Data = temp;
                Location += bytes.Length;
            }
            else
            {
                Buffer.BlockCopy(bytes, 0, Data, Location, bytes.Length);
                Location += bytes.Length;
            }
        }
        public void WriteBytes(byte[] bytes)
        {
            if (Data.Length - Location <= bytes.Length)
            {
                var temp = new byte[Data.Length + bytes.Length];
                Buffer.BlockCopy(Data, 0, temp, 0, Data.Length);
                Buffer.BlockCopy(bytes, 0, temp, Data.Length, bytes.Length);
                Data = temp;
                Location += bytes.Length;
            }
            else
            {
                Buffer.BlockCopy(bytes, 0, Data, Location, bytes.Length);
                Location += bytes.Length;
            }
        }
        public void Write(bool boolean)
        {
            AddToBuffer(BitConverter.GetBytes(boolean));
        }
        public void WriteBoolean(bool boolean)
        {
            AddToBuffer(BitConverter.GetBytes(boolean));
        }
        public void Write(byte Byte)
        {
            CheckSize(1);
            Data[Location] = Byte;
            Location += 1;
        }
        public void WriteByte(byte Byte)
        {
            CheckSize(1);
            Data[Location] = Byte;
            Location += 1;
        }
        public void Write(sbyte sByte)
        {
            CheckSize(1);
            Data[Location] = (byte)Math.Abs((int)sByte);
            Location += 1;
        }
        public void WriteSByte(sbyte sByte)
        {
            CheckSize(1);
            Data[Location] = (byte)Math.Abs((int)sByte);
            Location += 1;
        }
        public void Write(short Short)
        {
            AddToBuffer(BitConverter.GetBytes(Short));
        }
        public void WriteShort(short Short)
        {
            AddToBuffer(BitConverter.GetBytes(Short));
        }
        public void Write(ushort uShort)
        {
            AddToBuffer(BitConverter.GetBytes(uShort));
        }
        public void WriteUShort(ushort uShort)
        {
            AddToBuffer(BitConverter.GetBytes(uShort));
        }
        public void Write(int integer)
        {
            AddToBuffer(BitConverter.GetBytes(integer));
        }
        public void WriteInteger(int integer)
        {
            AddToBuffer(BitConverter.GetBytes(integer));
        }
        public void Write(uint uInteger)
        {
            AddToBuffer(BitConverter.GetBytes(uInteger));
        }
        public void WriteUInteger(uint uInteger)
        {
            AddToBuffer(BitConverter.GetBytes(uInteger));
        }
        public void Write(float Float)
        {
            AddToBuffer(BitConverter.GetBytes(Float));
        }
        public void WriteFloat(float Float)
        {
            AddToBuffer(BitConverter.GetBytes(Float));
        }
        public void WriteSingle(float Float)
        {
            AddToBuffer(BitConverter.GetBytes(Float));
        }
        public void Write(long Long)
        {
            AddToBuffer(BitConverter.GetBytes(Long));
        }
        public void WriteLong(long Long)
        {
            AddToBuffer(BitConverter.GetBytes(Long));
        }
        public void Write(ulong uLong)
        {
            AddToBuffer(BitConverter.GetBytes(uLong));
        }
        public void WriteULong(ulong uLong)
        {
            AddToBuffer(BitConverter.GetBytes(uLong));
        }
        public void Write(double Double)
        {
            AddToBuffer(BitConverter.GetBytes(Double));
        }
        public void WriteDouble(double Double)
        {
            AddToBuffer(BitConverter.GetBytes(Double));
        }
        public void Write(char Char)
        {
            AddToBuffer(BitConverter.GetBytes(Char));
        }
        public void WriteCharacter(char Char)
        {
            AddToBuffer(BitConverter.GetBytes(Char));
        }
        public void Write(string String)
        {
            var temp = Encoding.UTF8.GetBytes(String);
            AddToBuffer(BitConverter.GetBytes(temp.Length));
            AddToBuffer(temp);
        }
        public void WriteString(string String)
        {
            var temp = Encoding.UTF8.GetBytes(String);
            AddToBuffer(BitConverter.GetBytes(temp.Length));
            AddToBuffer(temp);
        }
        public void Write(object Object)
        {
            var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, Object);
            AddToBuffer(BitConverter.GetBytes((int)memoryStream.Length));
            var temp = new byte[Data.Length + memoryStream.ToArray().Length];
            Buffer.BlockCopy(Data, 0, temp, 0, Data.Length);
            Buffer.BlockCopy(memoryStream.ToArray(), 0, temp, Data.Length, memoryStream.ToArray().Length);
            Data = temp;
            Location += memoryStream.ToArray().Length;
            memoryStream.Dispose();
        }
        public void WriteObject(object Object)
        {
            var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, Object);
            AddToBuffer(BitConverter.GetBytes((int)memoryStream.Length));
            var temp = new byte[Data.Length + memoryStream.ToArray().Length];
            Buffer.BlockCopy(Data, 0, temp, 0, Data.Length);
            Buffer.BlockCopy(memoryStream.ToArray(), 0, temp, Data.Length, memoryStream.ToArray().Length);
            Data = temp;
            Location += memoryStream.ToArray().Length;
            memoryStream.Dispose();
        }
    }
}