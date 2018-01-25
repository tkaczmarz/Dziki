using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;

namespace TestujeSocketRAZ.model.receive
{
    class ErrorResponse : Request
    {
        public string result { get; set; }
        public string description { get; set; }

        public bool CancelConnection()
        {
            return false ;
        }

        public bool isRequestSuccess()
        {
            return false;
        }

        public string responseMessage()
        {
            return description;
        }
    }
}
