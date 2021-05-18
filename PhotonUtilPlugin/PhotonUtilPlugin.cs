using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;

namespace PhotonUtil
{
    [BepInPlugin("org.hollofox.plugins.PhotonUtil", "Photon Util", "1.0.0.0")]
    public class PhotonUtilPlugin: BaseUnityPlugin
    {
        private static readonly Dictionary<string, ConcurrentQueue<PhotonMessage>> _incomingQueues = new Dictionary<string, ConcurrentQueue<PhotonMessage>>();
        private static readonly Dictionary<string, ConcurrentQueue<PhotonMessage>> _outGoingQueues = new Dictionary<string, ConcurrentQueue<PhotonMessage>>();
        
        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            
        }
        
        void Update()
        {
            var incomingString = "{}"; // Read from chat
            if (incomingString.Contains("PackageId") && incomingString.Contains("Version") && incomingString.Contains("SerializedMessage"))
            {
                try
                {
                    var message = JsonConvert.DeserializeObject<PhotonMessage>(incomingString);
                    ReceiveMessage(message);
                    // Remove from chat
                }
                catch (Exception e)
                {
                    Debug.Log($"message {incomingString} was a false positive" );
                }
            }
        }

        public static void SendMessage(PhotonMessage message)
        {
            Debug.Log($"Sending Message: {message.SerializedMessage}");
            var key = message.PackageId;
            if (!_outGoingQueues.ContainsKey(key)) _outGoingQueues.Add(key, new ConcurrentQueue<PhotonMessage>());
            var queue = _outGoingQueues[key];
            queue.Enqueue(message);
        }

        private static void ReceiveMessage(PhotonMessage message)
        {
            var key = message.PackageId;
            if (!_incomingQueues.ContainsKey(key)) _incomingQueues.Add(key, new ConcurrentQueue<PhotonMessage>());
            var queue = _incomingQueues[key];
            queue.Enqueue(message);
        }

        public static ConcurrentQueue<PhotonMessage> GetIncomingMessageQueue(string key)
        {
            return _incomingQueues.ContainsKey(key) ? _incomingQueues[key] : null;
        }

        public static bool AddQueue(string key)
        {
            Debug.Log($"Loading Key: {key}");
            if (_incomingQueues.ContainsKey(key)) return false;
            _incomingQueues.Add(key, new ConcurrentQueue<PhotonMessage>());
            _outGoingQueues.Add(key, new ConcurrentQueue<PhotonMessage>());
            return true;
        }
    }
}
