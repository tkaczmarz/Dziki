using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.model.send;

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

        public int addRoom(String playerName, String roomName, int maxRoomSize)
        {
            RoomCreation room = new RoomCreation(playerName, roomName, maxRoomSize);

            factory.Request r = udp.SendRequest<RoomCreation>(room, serverPort);
            int port;
            if (int.TryParse(r.responseMessage(), out port))
            {
                return port;
            }
            else
                return -1;
        }

        public bool roomStart(String roomName, String token, String adminNickname)
        {
            RoomStart start = new RoomStart(roomName, token, adminNickname);

            return udp.SendRequest<RoomStart>(start, serverPort).isRequestSuccess();
        }
    }
}
