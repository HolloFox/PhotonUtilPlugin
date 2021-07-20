using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Newtonsoft.Json;

namespace PhotonUtil
{
    /// <summary>
    /// Backend handler created for an individual mod.
    /// It access the Photon network and fetches data specifically filtered for the mod.
    /// </summary>
    public class PunHandler
    {
        // Governing Mod
        private readonly string _mod;
        
        // Transaction
        private readonly Hashtable _myCustomProperties = new Hashtable();
        private readonly List<PhotonMessage> _messages = new List<PhotonMessage>();
        private readonly Dictionary<PhotonPlayer, List<PhotonMessage>> receivedMessages = new Dictionary<PhotonPlayer, List<PhotonMessage>>();

        // Local Instance
        private Dictionary<(PhotonPlayer,string), (PhotonMessage, Hashtable)> Instances 
            = new Dictionary<(PhotonPlayer, string), (PhotonMessage, Hashtable)>();

        // Retrieved
        private Dictionary<(PhotonPlayer, string), PhotonMessage> OtherData
            = new Dictionary<(PhotonPlayer, string), PhotonMessage>();

        /// <summary>
        /// Constructor used to handle all PUN related message for a mod
        /// </summary>
        /// <param name="mod">GUID of </param>
        public PunHandler(string mod)
        {
            _mod = mod;
            try
            {
                _myCustomProperties[mod] = JsonConvert.SerializeObject(_messages);

                PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }

        // Transaction Methods
        /// <summary>
        /// Adds a message to the ledger to allow other players to see.
        /// </summary>
        /// <param name="message">The message intended to be sent.</param>
        public void Add(PhotonMessage message)
        {
            if (_messages.Count > 0) message.Id = _messages.Last().Id + 1; // still don't trust you.
            else message.Id = 0;
            _messages.Add(message);
            _myCustomProperties[_mod] = JsonConvert.SerializeObject(_messages);
            PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
        }
        
        /// <summary>
        /// Remove all non-persistent messages from the ledger
        /// </summary>
        public void ClearNonPersistent()
        {
            _messages.RemoveAll(m => !m.Persist);
            _myCustomProperties[_mod] = JsonConvert.SerializeObject(_messages);
            PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
        }
        
        /// <summary>
        /// Gets a ledger of messages a player has sent/
        /// </summary>
        /// <returns>Gets a dictionary of messages for each player.</returns>
        public Dictionary<PhotonPlayer, List<PhotonMessage>> GetPlayerInfo()
        {
            var players = PhotonNetwork.playerList;
            foreach (var player in players)
            {
                if (!player.CustomProperties.ContainsKey(_mod)) continue;
                var x = (string) player.CustomProperties[_mod];
                var messages = JsonConvert.DeserializeObject<List<PhotonMessage>>(x);

                if (receivedMessages.ContainsKey(player))
                {
                    var list = receivedMessages[player];
                    list.ForEach(t => t.Viewed = true);
                    messages.RemoveAll(m => list.Any(p => p.Id == m.Id));
                    list.AddRange(messages);
                    receivedMessages[player] = list;
                }
                else receivedMessages[player] = messages;
            }
            return receivedMessages;
        }


        /// <summary>
        /// Creates an entry to store a value over the photon network with a key based on the mod.
        /// </summary>
        /// <param name="InstanceId">Identifier of the value you are storing</param>
        /// <param name="message">Wrapped value that you are storing</param>
        public void Create(string InstanceId, PhotonMessage message)
        {
            var player = PhotonNetwork.player;

            if (Instances.ContainsKey((player,InstanceId))) Update(InstanceId, message);
            else
            {
                UnityEngine.Debug.Log($"Creating instance: {_mod}.{InstanceId}");
                message.Id = 0;
                Hashtable property = new Hashtable();
                property[$"{_mod}.{InstanceId}"] = JsonConvert.SerializeObject(_messages);
                PhotonNetwork.SetPlayerCustomProperties(property);
                Instances[(player, InstanceId)] = (message,property);

                
            }
        }

        /// <summary>
        /// Reads a stored message from a specific player on the photon network.
        /// This also returns whether the message has been read before.
        /// </summary>
        /// <param name="player">The player being probed.</param>
        /// <param name="InstanceId">The instance of data being retrieved.</param>
        /// <returns>The data that was stored</returns>
        /// <returns>The data that was stored</returns>
        public PhotonMessage Read(PhotonPlayer player, string InstanceId)
        {
            if (!player.CustomProperties.ContainsKey($"{_mod}.{InstanceId}"))
            {
                OtherData.Remove((player,InstanceId));
                UnityEngine.Debug.Log($"Does not exist: {_mod}.{InstanceId}");
                return null;
            }
            var message = JsonConvert.DeserializeObject<PhotonMessage>((string)player.CustomProperties[$"{_mod}.{InstanceId}"]);
            if (OtherData.ContainsKey((player, InstanceId)))
            {
                var stored = OtherData[(player, InstanceId)];
                if (stored.Id == message.Id)
                {
                    message.Viewed = true;
                }
            }
            OtherData[(player, InstanceId)] = message;
            return message;
        }

        /// <summary>
        /// Allows to update an instance with a new message with an incrementing Id.
        /// </summary>
        /// <param name="InstanceId">The unique identifier of the instance that is being updated.</param>
        /// <param name="message">The new message that is replacing the old message.</param>
        public void Update(string InstanceId, PhotonMessage message)
        {
            var player = PhotonNetwork.player;
            if (!Instances.ContainsKey((player, InstanceId))) Create(InstanceId, message);
            else
            {
                UnityEngine.Debug.Log($"Updating instance: {_mod}.{InstanceId}");
                var tuple = Instances[(player, InstanceId)];
                message.Id = tuple.Item1.Id + 1;
                tuple.Item2[$"{_mod}.{InstanceId}"] = JsonConvert.SerializeObject(message);
                PhotonNetwork.SetPlayerCustomProperties(tuple.Item2);
                Instances[(player, InstanceId)] = (message, tuple.Item2);
            }
        }

        /// <summary>
        /// Clears a message from the mod's instance allowing for things like cleanup.
        /// </summary>
        /// <param name="InstanceId">The instance identifier for the mod.</param>
        public void Delete(string InstanceId)
        {
            UnityEngine.Debug.Log($"Deleting instance: {_mod}.{InstanceId}");
            var properties = new string[1];
            properties[0] = $"{_mod}.{InstanceId}";
            PhotonNetwork.RemovePlayerCustomProperties(properties);
        }
    }
}
