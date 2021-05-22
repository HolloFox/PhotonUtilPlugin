using System;

namespace PhotonUtil
{
    public class PhotonMessage
    {
        public DateTime Created = DateTime.Now;
        public string PackageId;
        public string Version;
        public string SerializedMessage;
        public Guid Author;
        public bool Persist = false;
    }
}
