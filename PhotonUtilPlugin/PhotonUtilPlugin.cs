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
            if (OnBoard() && !Handlers.ContainsKey(Guid))
            {
                AddMod(Guid);
            }
        }

        private bool OnBoard()
        {
            return (CameraController.HasInstance &&
                    BoardSessionManager.HasInstance &&
                    BoardSessionManager.HasBoardAndIsInNominalState &&
                    !BoardSessionManager.IsLoading);
        }

        public static Guid GetAuthor()
        {
            return AuthorId;
        }

        public static void AddMessage(string modGuid, PhotonMessage message)
        {
            message.Author = GetAuthor(); // I won't trust you
            Handlers[modGuid].Add(message);
        }

        public static void AddMod(string modGuid)
        {
            UnityEngine.Debug.Log($"Adding Mod: {modGuid}");
            Handlers.Add(modGuid, new PunHandler(modGuid));
            UnityEngine.Debug.Log($"Mod {modGuid} Added");
        }

        public static void ClearNonPersistent(string modGuid)
        {
            UnityEngine.Debug.Log($"Clearing Mod: {modGuid}");
            Handlers[modGuid].ClearNonPersistent();
            UnityEngine.Debug.Log($"Mod {modGuid} Cleared");
        }

        public static Dictionary<PhotonPlayer, List<PhotonMessage>> GetMessages(string modGuid)
        {
            return Handlers[modGuid].GetPlayerInfo();
        }
    }
}
