using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    private static PlayerManager _instance;
    public static PlayerManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerManager();
            return _instance;
        }
    }

    public List<Player> allPlayers = new List<Player>();
    public List<Player> seatedPlayers = new List<Player>();
    public List<Player> activePlayers = new List<Player>();
    public int totalSeatNum;

    public void InitPlayers(List<string> nameList)
    {
        foreach (string name in nameList)
        {
            Player p = new Player(name);
            allPlayers.Add(p);
        }
    }

    public void SeatedPlayer(Player p, int seatNum)
    {
        p.seatNum = seatNum;
        seatedPlayers.Add(p);
    }

    public void UnseatedPlayer(Player p)
    {
        p.seatNum = -1;
        seatedPlayers.Remove(p);
    }

    public void SetPlayersRole(int btn)
    {
        int bb = (btn + 1) % totalSeatNum;
        int sb = (btn + 2) % totalSeatNum;
        foreach(Player p in seatedPlayers)
        {
            int curSeat = p.seatNum;
            if (curSeat == btn)
            {
                p.role = Player.PlayerRole.button;
            } else if (curSeat == bb)
            {
                p.role = Player.PlayerRole.bigBlind;
            } else if (curSeat == sb)
            {
                p.role = Player.PlayerRole.smallBlind;
            } else
            {
                p.role = Player.PlayerRole.normal;
            }
        }
    }
}
