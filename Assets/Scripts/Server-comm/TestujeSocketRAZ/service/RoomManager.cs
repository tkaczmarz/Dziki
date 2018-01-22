using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.model;
using TestujeSocketRAZ.model.send;
using TestujeSocketRAZ.model.receive;

namespace TestujeSocketRAZ.service
{
    class RoomManager
    {
        public static string addRoom(String playerName, String roomName, int maxRoomSize)
        {
            RoomCreation room = new RoomCreation(playerName, roomName, maxRoomSize);

            //TODO SEND
            UdpManager manager = new UdpManager();
            RoomCreationSuccess response = manager.SendRequest<RoomCreation, RoomCreationSuccess>(room, 38621);
            return response.result;
        }

        public static void roomStart(String roomName, String token, String adminNickname)
        {
            RoomStart start = new RoomStart(roomName, token, adminNickname);

            //TODO SEND
        }
    }
}
