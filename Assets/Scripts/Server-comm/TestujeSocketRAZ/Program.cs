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
            //

            AddPlayer player = new AddPlayer("rooooooms", null, "playerNaameab"); // port do komunikacji zwraca w resultMessage() jako string;
            RoomCreation room = new RoomCreation("playeeerr", "rooooooms", 3);
            RoomStart start = new RoomStart("roomnaaamex", "token", "abs");
            PlayerMove playerMove = new PlayerMove();

            //udpClient.SendRequest<RoomStart>(start, 38621);

            //########## PIERW TO
            //RoomManager rm = new RoomManager(38621);
            //rm.addRoom("abs", "roomnaaame", 3);

            //########## POTEM TO Z NAZWA POKOJU CO ADDROOM
            //PlayerManager pm = new PlayerManager(38621);
            //pm.addPlayer("roomnaaame", null, "playernameA");


            //####### POTEM TO I WSZYSTKO DZIAŁA
            UdpManager udpClient = new UdpManager();
            RoomStart start1 = new RoomStart("roomnaaamex", "token", "abs");
            Console.WriteLine(udpClient.SendRequest<RoomStart>(start1, 38621).isRequestSuccess());

            //to jest odpowiedzialne za poruszanie graczem
            //wysylasz swoj ruch i konczysz polaczenie
            //udpClient.SendRequestPlayerMove(playerMove, 58278);
            //laczysz sie z serwerem i sluchasz wiadomosci
            while (true) {
                //udpClient.waitForMessages(58278);
                //jak jakas przyjdzie mozesz dac break i znow SendRequestPlayerMove z twoim ruchem
            }
            Console.Read();
        }
    }
}
