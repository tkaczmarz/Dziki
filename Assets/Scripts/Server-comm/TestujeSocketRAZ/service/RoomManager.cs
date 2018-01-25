using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.model.send;
using UnityEngine;

namespace TestujeSocketRAZ.service
{
    class RoomManager
    {

        private UdpManager udp;
        private int serverPort { get; set; }

        public RoomManager(int serverPort = 38621)
        {
            this.serverPort = serverPort;
            udp = new UdpManager();
        }

        public bool addRoom(String playerName, String roomName, int maxRoomSize)
        {
            RoomCreation room = new RoomCreation(playerName, roomName, maxRoomSize);

            factory.Request r = udp.SendRequest<RoomCreation>(room, serverPort);
            Debug.Log("Adding room. Message: " + r.responseMessage());
            return r.isRequestSuccess();
        }

        public bool roomStart(String roomName, String token, String adminNickname)
        {
            RoomStart start = new RoomStart(roomName, token, adminNickname);

            factory.Request r = udp.SendRequest<RoomStart>(start, serverPort);
            Debug.Log("Adding room. Message: " + r.responseMessage());
            return r.isRequestSuccess();
        }
    }
}
