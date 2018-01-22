using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestujeSocketRAZ.model.send
{
    class AddPlayer
    {
        public string commType = "addPlayer";
        public string roomName { get; set; }
        public string roomPassword { get; set; }
        public string playerName { get; set; }

        public AddPlayer(String roomName, String roomPassword, String playerName)
        {
            this.roomName = roomName;
            this.playerName = playerName;
            if (roomPassword == null)
                this.roomPassword = "";
            else
                this.roomPassword = roomPassword;
        }
    }
}
