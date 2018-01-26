using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;

namespace TestujeSocketRAZ.model.receive
{
    class KickUser : Request
    {

        private String message;
        private bool cancelConnection;

        public string additionalInformations()
        {
            throw new NotImplementedException();
        }

        public bool CancelConnection()
        {
            return cancelConnection;
        }

        public bool isRequestSuccess()
        {
            throw new NotImplementedException();
        }

        public string responseMessage()
        {
            throw new NotImplementedException();
        }

        private void jsonToReuqest()
        {
            throw new NotImplementedException();
        }
    }
}
