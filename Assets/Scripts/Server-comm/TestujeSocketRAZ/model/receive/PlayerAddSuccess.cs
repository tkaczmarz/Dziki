using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;

namespace TestujeSocketRAZ.model.receive
{
    class PlayerAddSuccess : Request
    {
        public string commType { get; set; }
        public string result { get; set; }
        public int roomPort { get; set; }
        private bool cancelConnection = false;

        public bool CancelConnection()
        {
            return cancelConnection;
        }

        public string responseMessage()
        {
            return roomPort.ToString();
        }

        public bool isRequestSuccess()
        {
            return true;
        }
    }
}
