using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestujeSocketRAZ.service
{
    class UdpManager
    {
        private int clientPort = 11000;
        private UdpClient udpClient;

        public UdpManager()
        {
            this.udpClient = new UdpClient(clientPort);
        }

        //pierwszy element mowi co zamieniamy na jsona
        //drugi mowi co chcemy zwrocic
        public T2 SendRequest<T1, T2>(T1 jsonRequest, int serverPort)
        {
            string message = JsonConvert.SerializeObject(jsonRequest);
            Console.WriteLine(message);

            udpClient.Connect("localhost", serverPort);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            udpClient.Send(sendBytes, sendBytes.Length);

            //received data
            //IPEndPoint object will allow us to read datagrams sent from any source.
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
            string returnData = Encoding.ASCII.GetString(receiveBytes);

            udpClient.Close();
            return ResponseHandler.getObjectFromResponse<T2>(returnData.ToString());
        }

    }
}
