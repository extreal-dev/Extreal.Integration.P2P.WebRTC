using System;

namespace Extreal.Integration.P2P.WebRTC
{
    /// <summary>
    /// Exception thrown when hostname already exists at creation of host.
    /// </summary>
    public class HostNameAlreadyExistsException : Exception
    {
        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// <param name="message">Message</param>
        public HostNameAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
