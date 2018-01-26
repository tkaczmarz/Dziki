using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.model.send;
using UnityEngine;
using System.Collections;

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


        public int addPlayer(String roomName, String roomPassword, String playerName, out List<string> players)
        {
            AddPlayer player = new AddPlayer(roomName, roomPassword, playerName);

            factory.Request r = udp.SendRequest<AddPlayer>(player, serverPort);
            int port;
            Debug.Log("RESPONSE ON ADD PLAYER: " + r.additionalInformations());
            players = ResponseHandler.getArrayFromString(r.additionalInformations());
            if (int.TryParse(r.responseMessage(), out port))
            {
                Debug.Log("ADDING PLAYER ON PORT: " + port);
                Debug.Log("PLAYER LIST: ");
                foreach (string p in players)
                    Debug.Log("PLAYER: " + p);
                return port;
            }
            else
                return -1;
        }

    }
}
