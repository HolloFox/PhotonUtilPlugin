using System;

namespace PhotonUtil
{
    /// <summary>
    /// Base Wrapper class to send messages via Photon Network.
    /// </summary>
    public class PhotonMessage
    {
        /// <summary>
        /// Time message created, Auto-generated.
        /// </summary>
        public readonly DateTime Created = DateTime.Now;

        /// <summary>
        /// Identifier of mod/package used to send/receive the message.
        /// </summary>
        public string PackageId;

        /// <summary>
        /// Version of the mod/package being used.
        /// </summary>
        public string Version;

        /// <summary>
        /// The content of the message being sent
        /// </summary>
        public string SerializedMessage;

        /// <summary>
        /// The author of the message, 
        /// </summary>
        public Guid Author;

        /// <summary>
        /// Id of the message being sent, this auto-increments.
        /// </summary>
        public UInt64 Id = 0;

        /// <summary>
        /// Determines if the message persist upon using the ledger component.
        /// </summary>
        public bool Persist = false;

        /// <summary>
        /// Determines if the message has been viewed before.
        /// </summary>
        public bool Viewed = false;
    }
}
