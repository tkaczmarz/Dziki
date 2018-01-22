using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestujeSocketRAZ.model.send
{
    class RoomStart
    {
        public string commType = "roomStart";
        public string roomName { get; set; }
        public string token { get; set; }
        public string adminNickname { get; set; }

        public RoomStart(String roomName, String token, String adminNickname)
        {
            this.roomName = roomName;
            this.token = token;
            this.adminNickname = adminNickname;
        }
    }
}
