using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;

namespace TestujeSocketRAZ.model.send
{
    class PlayerMove : Request
    {
        public string commType = "playerMove";
        public string test = "test"; //tutaj mozesz stworzyc swoje pola

        public string additionalInformations()
        {
            return "";
        }

        //twoje pola
        //kolejne pole
        //kolejne etc

        public bool CancelConnection()
        {
            return false;
        }

        public bool isRequestSuccess()
        {
            return true;
        }

        public string responseMessage()
        {
            return test;//tutaj tez mozesz wlasna wiadomosc wyslac "gracz zmienil pozycje z... do ...
        }
    }
}
