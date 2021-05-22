using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Newtonsoft.Json;

namespace PhotonUtil
{
    public class PunHandler
    {
        private readonly Hashtable _myCustomProperties = new Hashtable();

        private readonly List<PhotonMessage> _messages = new List<PhotonMessage>();

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
                output.Add(player,messages);
            }
            return output;
        }
    }
}
