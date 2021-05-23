using System;
using System.Collections.Generic;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace PhotonUtil
{
    [BepInPlugin(Guid, "Photon Util", Version)]
    public class PhotonUtilPlugin: BaseUnityPlugin
    {
        /// <summary>
        /// Identifier used to include this plugin as a bepinex dependency.
        /// </summary>
        public const string Guid = "org.hollofox.plugins.PhotonUtil";
        private const string Version = "1.0.2.0";

        private static readonly Guid AuthorId = System.Guid.NewGuid();
        
        private static readonly Dictionary<string,PunHandler> Handlers = new Dictionary<string, PunHandler>();

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
        }
        
        void Update()
        {
        }

        /// <summary>
        /// Gets a unique id for your player this session.
        /// </summary>
        /// <returns>static Guid value for the session</returns>
        public static Guid GetAuthor()
        {
            return AuthorId;
        }

        /// <summary>
        /// Message being added to the ledger.
        /// </summary>
        /// <param name="modGuid">The GUID of the mod</param>
        /// <param name="message">Message being added to the ledger.</param>
        public static void AddMessage(string modGuid, PhotonMessage message)
        {
            message.Author = GetAuthor(); // I won't trust you
            Handlers[modGuid].Add(message);
        }

        /// <summary>
        /// Adds a mod to handle Photon messages.
        /// </summary>
        /// <param name="modGuid">The GUID of the mod</param>
        public static void AddMod(string modGuid)
        {
            UnityEngine.Debug.Log($"Adding Mod: {modGuid}");
            Handlers.Add(modGuid, new PunHandler(modGuid));
            UnityEngine.Debug.Log($"Mod {modGuid} Added");
        }

        /// <summary>
        /// Clears mods that aren't designed to be persistent for this session.
        /// </summary>
        /// <param name="modGuid">The identifier of the mod.</param>
        public static void ClearNonPersistent(string modGuid)
        {
            UnityEngine.Debug.Log($"Clearing Mod: {modGuid}");
            Handlers[modGuid].ClearNonPersistent();
            UnityEngine.Debug.Log($"Mod {modGuid} Cleared");
        }

        /// <summary>
        /// retrieves all PhotonMessages to view, This is in a Ledger to see past to most recent.
        /// </summary>
        /// <param name="modGuid">The GUID of the mod.</param>
        /// <returns>List of messages from each player.</returns>
        public static Dictionary<PhotonPlayer, List<PhotonMessage>> GetMessages(string modGuid)
        {
            return Handlers[modGuid].GetPlayerInfo();
        }

        /// <summary>
        /// Creates a value on the instance
        /// </summary>
        /// <param name="InstanceId">The instance identifier</param>
        /// <param name="message">The message being stored.</param>
        public static void CreateInstance(string InstanceId, PhotonMessage message)
        {
            message.Author = GetAuthor();
            if (!Handlers.ContainsKey(message.PackageId)) return;
            var handler = Handlers[message.PackageId];
            handler.Create(InstanceId,message);
        }

        /// <summary>
        /// Updates a value on the instance.
        /// </summary>
        /// <param name="InstanceId">The instance identifier</param>
        /// <param name="message">The message being stored.</param>
        public static void UpdateInstance(string InstanceId, PhotonMessage message)
        {
            message.Author = GetAuthor();
            if (!Handlers.ContainsKey(message.PackageId)) return;
            var handler = Handlers[message.PackageId];
            handler.Update(InstanceId, message);
        }

        /// <summary>
        /// Retrieves photon message for an instance.
        /// </summary>
        /// <param name="modGuid">The mod that uses this instance.</param>
        /// <param name="InstanceId">The instance identifier.</param>
        /// <param name="player">The player it is based on.</param>
        /// <returns>The message stored on the instance.</returns>
        public static PhotonMessage ReadInstance(string modGuid, string InstanceId, PhotonPlayer player)
        {
            if (!Handlers.ContainsKey(modGuid)) return null;
            var handler = Handlers[modGuid];
            return handler.Read(player,InstanceId);
        }

        /// <summary>
        /// Deletes an instance for your mod.
        /// </summary>
        /// <param name="modGuid">The id of your mod</param>
        /// <param name="InstanceId">the instance you want to clear.</param>
        public static void DeleteInstance(string modGuid, string InstanceId)
        {
            if (!Handlers.ContainsKey(modGuid)) return;
            var handler = Handlers[modGuid];
            handler.Delete(InstanceId);
        }
    }
}
