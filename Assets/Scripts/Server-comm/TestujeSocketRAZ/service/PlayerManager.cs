using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.model.send;
using UnityEngine;

namespace TestujeSocketRAZ.service
{
    class PlayerManager
    {
        private int serverPort { get; set; }
        private UdpManager udp;

        public PlayerManager(int serverPort = 38621)
        {
            this.serverPort = serverPort;
            udp = new UdpManager();
        }


        public bool addPlayer(String roomName, String roomPassword, String playerName)
        {
            AddPlayer player = new AddPlayer(roomName, roomPassword, playerName);

            factory.Request r = udp.SendRequest<AddPlayer>(player, serverPort);
            Debug.Log("Adding player. Message: " + r.responseMessage());
            return r.isRequestSuccess();
        }

    }
}
