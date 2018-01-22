using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestujeSocketRAZ.model
{
    class RoomCreationSuccess
    {
        public string commType { get; set; }
        public string result { get; set; }
        public int roomPort {
            get { return roomPort; }
            set { roomPort = Convert.ToInt32(value); }
        }
        public string token { get; set; }
    }
}
