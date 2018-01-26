using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;

namespace TestujeSocketRAZ.model.send
{
    class RoomStart : Request
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

        public bool isRequestSuccess()
        {
            return true;
        }

        public bool CancelConnection()
        {
            return false;
        }

        public string responseMessage()
        {
            return token;
        }

        public string additionalInformations()
        {
            return "";
        }
    }
}
