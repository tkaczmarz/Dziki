using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.factory;
using TestujeSocketRAZ.model.send;
using UnityEngine;

namespace TestujeSocketRAZ.service
{
    class UdpManager
    {
        public int clientPort = 11000;
        private UdpClient udpClient;

        public UdpManager()
        {
            bool error = false;

            do
            {
                try
                {
                    this.udpClient = new UdpClient(clientPort);
                    error = false;
                }
                catch (Exception e)
                {
                    clientPort++;
                    error = true;
                }
            }
            while (error);
            
            this.udpClient.Client.SendTimeout = 5000;
            this.udpClient.Client.ReceiveTimeout = 5000;
        }

        //pierwszy element mowi co zamieniamy na jsona
        //drugi mowi co chcemy zwrocic
        public Request SendRequest<T1>(T1 jsonRequest, int serverPort)
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

            Console.WriteLine(returnData);

            Console.WriteLine(RequestFactory.ReceiveRequest(returnData).responseMessage());
            Console.WriteLine(RequestFactory.ReceiveRequest(returnData).isRequestSuccess());
            return RequestFactory.ReceiveRequest(returnData);
        }

        public Request SendRequestPlayerMove(PlayerMove jsonRequest, int serverPort)
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

            udpClient.Close(); //to bylo potrzebne do wyslania pojedynczego requestu
            //w tym momencie trzymasz polaczenie caly czas
            //https://msdn.microsoft.com/pl-pl/library/tst0kwb1(v=vs.110).aspx jak bedzie trzeba to wrzuce to do while
            Console.WriteLine(returnData);
            return RequestFactory.ReceiveRequest(returnData);
        }

        //podajesz ip to samo co przy sendRequestPlayerMove
        // public String waitForMessages(int serverPort) // to jest cos podobnego do powyzszego, ale tutaj tylko sluchasz serwera czy cos wysyla za wiadomosc np. ruch drugiego gracza
        // {
        //     udpClient.Connect("localhost", serverPort);
        //     IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //     Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
        //     string returnData = Encoding.ASCII.GetString(receiveBytes);

        //     udpClient.Close();
        //     return returnData;
        // }

        public void Connect(int serverPort)
        {
            udpClient.Connect("localhost", serverPort);
        }

        public String waitForMessages(int serverPort) // to jest cos podobnego do powyzszego, ale tutaj tylko sluchasz serwera czy cos wysyla za wiadomosc np. ruch drugiego gracza
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            string data = "";

            // Task.Run(async () =>
            // {
            //     while (true)
            //     {
            //         Debug.Log("<color=magenta>START LISTENING</color>");
            //         var result = await udpClient.ReceiveAsync();
            //         Debug.Log("<color=magenta>Data: " + result.ToString() + "</color>");
            //         data += Encoding.ASCII.GetString(result.Buffer);
            //     }
            // });

            // try
            // {
            //     udpClient.BeginReceive(new AsyncCallback(recv), null);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError("Exception while receiving data: " + e.StackTrace);
            // }

            // Task.Run(() => 
            // {
            //     Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
            //     data = Encoding.ASCII.GetString(receiveBytes);
            //     Debug.Log("<color=magenta>Data: " + data + "</color>");
            // });

            try
            {
                udpClient.Client.Blocking = false;
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                data = Encoding.ASCII.GetString(receiveBytes);
            }
            catch (Exception e)
            {
                
            }

            // udpClient.Close();
            return data;
        }

        private void recv(IAsyncResult result)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receiveBytes = udpClient.EndReceive(result, ref RemoteIpEndPoint);

            Debug.Log("<color=orange>Received: " + Encoding.ASCII.GetString(receiveBytes) + "</color>");
            udpClient.BeginReceive(new AsyncCallback(recv), null);
        }
    }
}
