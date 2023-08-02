using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Extreal.Integration.P2P.WebRTC
{
    /// <summary>
    /// Class representing the host start response.
    /// </summary>
    [SuppressMessage("Usage", "CC0047")]
    public class StartHostResponse
    {
        /// <summary>
        /// Status.
        /// </summary>
        /// <remarks>
        /// 200 for success, 409 for duplicate hostname.
        /// </remarks>
        [JsonPropertyName("status")]
        public ushort Status { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// Class representing the host list response.
    /// </summary>
    [SuppressMessage("Usage", "CC0047")]
    public class ListHostsResponse
    {
        /// <summary>
        /// Status.
        /// </summary>
        /// <remarks>
        /// Always 200.
        /// </remarks>
        [JsonPropertyName("status")]
        public ushort Status { get; set; }

        /// <summary>
        /// Host list.
        /// </summary>
        [JsonPropertyName("hosts")]
        public List<HostResponse> Hosts { get; set; }
    }

    /// <summary>
    /// Class representing the host response.
    /// </summary>
    [SuppressMessage("Usage", "CC0047")]
    public class HostResponse
    {
        /// <summary>
        /// Id.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
