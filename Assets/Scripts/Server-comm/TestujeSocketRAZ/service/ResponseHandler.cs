using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.model.send;

namespace TestujeSocketRAZ.service
{
    class ResponseHandler
    {

        public static T getObjectFromResponse<T>(String response)
        {
            if (response.Contains("error"))
                throw new Exception(response);

            return JsonConvert.DeserializeObject<T>(response);
        }
    }
}
