using System;

namespace Extreal.Integration.P2P.WebRTC
{
    public class HostNameAlreadyExistsException : Exception
    {
        public HostNameAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
