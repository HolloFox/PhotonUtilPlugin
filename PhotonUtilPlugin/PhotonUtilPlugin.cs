using System;
using System.Collections.Generic;
using BepInEx;

namespace PhotonUtil
{
    [BepInPlugin(Guid, "Photon Util", Version)]
    public class PhotonUtilPlugin: BaseUnityPlugin
    {
        public const string Guid = "org.hollofox.plugins.PhotonUtil";
        private const string Version = "1.0.0.0";

        private static readonly Guid AuthorId = System.Guid.NewGuid();
        
        private static readonly Dictionary<string,PunHandler> Handlers = new Dictionary<string, PunHandler>();

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
        }
        
        void Update()
        {
        }

        private bool OnBoard()
        {
            return (CameraController.HasInstance &&
                    BoardSessionManager.HasInstance &&
                    BoardSessionManager.HasBoardAndIsInNominalState &&
                    !BoardSessionManager.IsLoading);
        }

        public Guid getAuthor()
        {
            return AuthorId;
        }

        public void AddMessage(string modGuid, PhotonMessage message)
        {
            message.Author = getAuthor(); // I won't trust you
            Handlers[modGuid].Add(message);
        }

        public void AddMod(string modGuid)
        {
            Handlers.Add(modGuid, new PunHandler(modGuid));
        }

        public Dictionary<PhotonPlayer, List<PhotonMessage>> GetMessages(string modGuid)
        {
            return Handlers[modGuid].GetPlayerInfo();
        }
    }
}
