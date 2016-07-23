using System;

namespace ArchaicNet
{
    /// <summary>
    /// Creates a Message object to be sent over a packet.
    /// </summary>
    public partial class Message
    {
        /// <summary>
        /// Everything read or written stores as byte data in this array.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// The read/write location. Any values after this point in Data[]
        /// (unless copied using LoadData) is empty/null;
        /// </summary>
        public int Location;

        /// <summary>
        /// Initialize new Message with default size of 4.
        /// This is the size of one empty packet only using
        /// an integer as its name.
        /// </summary>
        public Message()
        {
            Data = new byte[4];
            Location = 0;
        }

        /// <summary>
        /// Initialize new Message with a presize for optimization.
        /// </summary>
        public Message(int initialSize)
        {
            Data = new byte[initialSize];
            Location = 0;
        }

        /// <summary>
        /// Initialize a new Message with existing data ready to be read.
        /// (Same as LoadData)
        /// </summary>
        public Message(byte[] bytes)
        {
            Data = bytes;
            Location = 0;
        }

        /// <summary>
        /// Reset message back to default size of 4.
        /// (See summary of Empty Constructor)
        /// </summary>
        public void Reset()
        {
            Data = new byte[4];
            Location = 0;
        }

        /// <summary>
        /// Reset message with a presize for optimization.
        /// (See summary of Initial Size Constructor)
        /// </summary>
        public void Reset(int initialSize)
        {
            Data = new byte[initialSize];
            Location = 0;
        }

        /// <summary>
        /// Reset message with existing data ready to be read.
        /// (See summary of Byte Array Constructor)
        /// </summary>
        public void Reset(byte[] bytes)
        {
            Data = bytes;
            Location = 0;
        }

        /// <summary>
        /// Null message values to prepair for garbage collection.
        /// </summary>
        public void Dispose()
        {
            Data = null;
            Location = 0;
        }

        /// <summary>
        /// Load existing data ready to be read.
        /// </summary>
        public void LoadData(byte[] bytes)
        {
            Data = bytes;
            Location = 0;
        }

        protected void AddToBuffer(byte[] bytes)
        {
            CheckSize(bytes.Length);
            Buffer.BlockCopy(bytes, 0, Data, Location, bytes.Length);
            Location += bytes.Length;
        }

        protected void AddToBuffer(byte[] bytes, int offset, int size)
        {
            CheckSize(size);
            Buffer.BlockCopy(bytes, offset, Data, Location, size);
            Location += size;
        }

        protected void CheckSize(int length)
        {
            if (length + Location < Data.Length) return;
            var size = Data.Length * 2;
            while (length + Location >= size)
                size *= 2;
            ResizeBuffer(size);
        }

        protected void ResizeBuffer(int length)
        {
            var temp = new byte[length];
            Buffer.BlockCopy(Data, 0, temp, 0, Location);
            Data = temp;
        }
    }
}