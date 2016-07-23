using System.Net.Sockets;

namespace ArchaicNet
{
    /// <summary>
    /// Helper class to generate a port for UDP.
    /// (Useful when you may expect one lan to host multiple of the application)
    /// </summary>
    public static class Unique
    {
        /// <summary>
        /// This will return the first unused port starting at the seed.
        /// This may be useful if design of an application may allow for multiple
        /// instances to run UDP methods which may conflict from overuse of the same
        /// port. 
        /// 
        /// NOTE: It is recommended in the cases mentioned previousely to save on the
        /// receiving end the generated port to allow the connections to exist dynamicly.
        /// </summary>
        public static int GeneratePort(int seed = 4075)
        {
            UdpClient testingPort = null;
            try { testingPort = new UdpClient(seed); } // May return error if port is already used.
            catch
            {
                if (testingPort != null)
                {
                    testingPort.Close();
                    testingPort = null;
                }
                return GeneratePort(seed + 1);
            }
            if (testingPort != null)
            {
                testingPort.Close();
                testingPort = null;
            }
            return seed;
        }
    }
}
