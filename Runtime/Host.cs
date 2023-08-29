namespace Extreal.Integration.P2P.WebRTC
{
    /// <summary>
    /// Class that represents the host.
    /// </summary>
    public class Host
    {
        /// <summary>
        /// Id.
        /// </summary>
        public string Id { get;}

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get;}

        /// <summary>
        /// Creates a new host.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="name">Name</param>
        public Host(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
