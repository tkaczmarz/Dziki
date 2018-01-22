using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestujeSocketRAZ.model;
using TestujeSocketRAZ.model.receive;
using TestujeSocketRAZ.model.send;
using TestujeSocketRAZ.service;

namespace TestujeSocketRAZ
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpManager udpClient = new UdpManager();
            AddPlayer player = new AddPlayer("roomName", null, "playerNaame");
            RoomCreation room = new RoomCreation("playeeerr", "roooooomsd", 3);
            RoomStart start = new RoomStart("roooooomsd", "token", "adminnickname");

            PlayerAddSuccess tmp = udpClient.SendRequest<AddPlayer, PlayerAddSuccess>(player, 27001);
            Console.WriteLine(tmp.result);
            Console.Read();
        }
    }
}
