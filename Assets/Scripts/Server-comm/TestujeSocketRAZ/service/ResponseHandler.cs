using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;
using TestujeSocketRAZ.model.send;

namespace TestujeSocketRAZ.service
{
    class ResponseHandler
    {
        public static Request getObjectFromResponse<T>(String response)
        {
            if (response.Contains("error"))
                throw new Exception(response);

            return (Request)JsonConvert.DeserializeObject<T>(response);
        }
    }
}
