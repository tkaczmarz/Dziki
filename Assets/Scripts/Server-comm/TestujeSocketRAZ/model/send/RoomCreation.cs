using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestujeSocketRAZ.model.send
{
    class RoomCreation
    {
        public string commType = "roomCreation";
        public string playerName { get; set; }
        public string roomName { get; set; }
        public int maxRoomSize { get; set; }

        public RoomCreation(String playerName, String roomName, int maxRoomSize)
        {
            this.playerName = playerName;
            this.roomName = roomName;
            this.maxRoomSize = maxRoomSize;
        }

    }
}
