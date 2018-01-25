using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour 
{
	private void Awake() 
    {
        
    }

    private IEnumerator WaitForServerCommands(int port)
    {
        //to jest odpowiedzialne za poruszanie graczem
        //wysylasz swoj ruch i konczysz polaczenie
        //udpClient.SendRequestPlayerMove(playerMove, 58278);
        //laczysz sie z serwerem i sluchasz wiadomosci
        while (true) 
        {

            //udpClient.waitForMessages(58278);
            //jak jakas przyjdzie mozesz dac break i znow SendRequestPlayerMove z twoim ruchem
        }
    }
}
