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

        public static List<string> getArrayFromString(String json)
        {
            bool beginRead = false;
            int first = -1;
            List<string> players = new List<string>();
            json = json.Replace('\\', '\0');
            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '[')
                    beginRead = true;
                else if (json[i] == ']')
                    break;

                if (!beginRead)
                    continue;

                if (first != -1 && (json[i] == '"'))
                {
                    players.Add(json.Substring(first, i - first));
                    first = -1;
                    continue;
                }

                if (json[i] == '"')
                    first = i + 1;
            }
            
            return players;
        }
    }
}
