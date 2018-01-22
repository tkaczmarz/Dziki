using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestujeSocketRAZ.model;
using TestujeSocketRAZ.model.send;
using TestujeSocketRAZ.model.receive;

namespace TestujeSocketRAZ.service
{
    class PlayerManager
    {
        public static string addPlayer(String roomName, String roomPassword, String playerName)
        {
            AddPlayer player = new AddPlayer(roomName, roomPassword, playerName);

            //TODO send request
            UdpManager manager = new UdpManager();
            PlayerAddSuccess response = manager.SendRequest<AddPlayer, PlayerAddSuccess>(player, 38621);
            return response.result;
        }

    }
}
