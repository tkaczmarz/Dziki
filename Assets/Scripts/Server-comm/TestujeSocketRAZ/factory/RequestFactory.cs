using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;
using TestujeSocketRAZ.model;
using TestujeSocketRAZ.model.receive;
using TestujeSocketRAZ.model.send;

namespace TestujeSocketRAZ.service
{
    class RequestFactory
    {
        public static Request ReceiveRequest(String json)
        {
            if (isContainString("error", json)){
                throw new Exception(json);
            }
            if (isContainString("failure", json))
            {
                return ResponseHandler.getObjectFromResponse<ErrorResponse>(json);
            }
            if (isContainString("playerAddedResult", json))
            {
                return ResponseHandler.getObjectFromResponse<PlayerAddSuccess>(json);
            }
            if(isContainString("roomCreation", json)){
                return ResponseHandler.getObjectFromResponse<RoomCreationSuccess>(json);
            }
            if(isContainString("roomStart", json))
            {
                return ResponseHandler.getObjectFromResponse<RoomStart>(json);
            }
            if (isContainString("playerMove", json))
            {
                return ResponseHandler.getObjectFromResponse<PlayerMove>(json);
            }


            throw new NotImplementedException();
        }

        private static Boolean isContainString(String result, String json)
        {
            return json.Contains(result);
        }
    }
}
