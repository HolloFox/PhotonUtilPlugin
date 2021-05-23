using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Newtonsoft.Json;

namespace PhotonUtil
{
    public class PunHandler
    {
        private readonly Hashtable _myCustomProperties = new Hashtable();

        private readonly List<PhotonMessage> _messages = new List<PhotonMessage>();

        private Dictionary<PhotonPlayer, List<PhotonMessage>> receivedMessages = new Dictionary<PhotonPlayer, List<PhotonMessage>>();

        private readonly string _mod;
        
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
    }
}
