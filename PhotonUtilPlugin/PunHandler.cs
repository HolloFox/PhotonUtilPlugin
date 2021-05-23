using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Newtonsoft.Json;

namespace PhotonUtil
{
    public class PunHandler
    {
        // Governing Mod
        private readonly string _mod;
        
        // Transaction
        private readonly Hashtable _myCustomProperties = new Hashtable();
        private readonly List<PhotonMessage> _messages = new List<PhotonMessage>();
        private Dictionary<PhotonPlayer, List<PhotonMessage>> receivedMessages = new Dictionary<PhotonPlayer, List<PhotonMessage>>();

        // Local Instance
        private Dictionary<(PhotonPlayer,string), (PhotonMessage, Hashtable)> Instances 
            = new Dictionary<(PhotonPlayer, string), (PhotonMessage, Hashtable)>();

        // Retrieved
        private Dictionary<(PhotonPlayer, string), PhotonMessage> OtherData
            = new Dictionary<(PhotonPlayer, string), PhotonMessage>();

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
        public void Add(PhotonMessage message)
        {
            message.Id = _messages.Last().Id + 1; // still don't trust you.
            _messages.Add(message);
            _myCustomProperties[_mod] = JsonConvert.SerializeObject(_messages);
            PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
        }
        public void ClearNonPersistent()
        {
            _messages.RemoveAll(m => !m.Persist);
            _myCustomProperties[_mod] = JsonConvert.SerializeObject(_messages);
            PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
        }
        public Dictionary<PhotonPlayer, List<PhotonMessage>> GetPlayerInfo()
        {
            var output = new Dictionary<PhotonPlayer, List<PhotonMessage>>();

            var players = PhotonNetwork.playerList;
            foreach (var player in players)
            {
                if (!player.CustomProperties.ContainsKey(_mod)) continue;
                var x = (string) player.CustomProperties[_mod];
                var messages = JsonConvert.DeserializeObject<List<PhotonMessage>>(x);

                if (receivedMessages.ContainsKey(player))
                {
                    var list = receivedMessages[player];
                    foreach (var photonMessage in messages.Where(m => list.Any( lm => lm.Id == m.Id)))
                    {
                        photonMessage.Viewed = true;
                    }
                }
                output.Add(player,messages);
            }
            receivedMessages = output;
            return output;
        }

        // Instance Related Methods
        public void Create(string InstanceId, PhotonMessage message)
        {
            var player = PhotonNetwork.player;

            if (Instances.ContainsKey((player,InstanceId))) Update(InstanceId, message);
            else
            {
                UnityEngine.Debug.Log($"Creating instance: {_mod}.{InstanceId}");
                Hashtable property = new Hashtable();
                property[$"{_mod}.{InstanceId}"] = JsonConvert.SerializeObject(_messages);
                PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
                Instances[(player, InstanceId)] = (message,property);
            }
        }

        // Read
        public PhotonMessage Read(PhotonPlayer player, string InstanceId)
        {
            if (!player.CustomProperties.ContainsKey($"{_mod}.{InstanceId}"))
            {
                OtherData.Remove((player,InstanceId));
                return null;
            }
            var message = JsonConvert.DeserializeObject<PhotonMessage>((string)player.CustomProperties[$"{_mod}.{InstanceId}"]);
            if (OtherData.ContainsKey((player, InstanceId)))
            {
                var stored = OtherData[(player, InstanceId)];
                if (stored.Id == message.Id)
                {
                    message.Viewed = true;
                    OtherData[(player, InstanceId)] = stored;
                }
            }
            return message;
        }

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
                PhotonNetwork.SetPlayerCustomProperties(_myCustomProperties);
                Instances[(player, InstanceId)] = (message, tuple.Item2);
            }
        }

        // Read
        public void Delete(string InstanceId)
        {
            UnityEngine.Debug.Log($"Deleting instance: {_mod}.{InstanceId}");
            var properties = new string[1];
            properties[0] = $"{_mod}.{InstanceId}";
            PhotonNetwork.RemovePlayerCustomProperties(properties);
        }
    }
}
