using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;

namespace TestujeSocketRAZ.model
{
    class RoomCreationSuccess : Request
    {
        public string commType { get; set; }
        public string result { get; set; }
        public int roomPort {
            get { return roomPort; }
            set { roomPort = Convert.ToInt32(value); }
        }
        public string token { get; set; }
        private bool cancelConnection = false;
        private bool requestSuccess = true;
        public bool CancelConnection()
        {
            return cancelConnection;
        }

        public bool isRequestSuccess()
        {
            return requestSuccess;
        }

        public string responseMessage()
        {

            return result;
        }
    }
}
