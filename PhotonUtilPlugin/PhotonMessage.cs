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
        public UInt64 Id = 0;
        public bool Persist = false;
        public bool Viewed = false;
    }
}
