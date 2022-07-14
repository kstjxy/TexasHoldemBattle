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

    public bool SeatPlayers()
    {
        seatedPlayers = new List<Player>();
        activePlayers = new List<Player>();
        totalSeatNum = 0;
        foreach (Player p in allPlayers)
        {
            if (p.isInGame)
            {
                seatedPlayers.Add(p);
                p.seatNum = totalSeatNum;
                totalSeatNum++;
                if (totalSeatNum > 8)
                {
                    Debug.Log("玩家人数超过上限8人，请更改选择");
                    ResetSeat();
                    return false;
                }
            }
        }
        if (totalSeatNum < 2)
        {
            Debug.Log("玩家人数未达到2人，请更改选择");
            ResetSeat();
            return false;
        }
        return true;
    }

    public void ResetSeat()
    {
        foreach (Player pl in seatedPlayers)
        {
            pl.seatNum = -1;
            pl.isInGame = false;
        }
        seatedPlayers.Clear();
    }

    public void NewRound()
    {
        foreach (Player p in seatedPlayers)
        {
            if (p.coin <= 0)
            {
                p.OutOfGame();
                totalSeatNum--;
                activePlayers.Remove(p);
            } else
            {
                p.ResetNewRound();
                if (!activePlayers.Contains(p))
                {
                    activePlayers.Add(p);
                }
            }
        }
        GolbalVar.curBtnSeat = (GolbalVar.curBtnSeat + 1) % totalSeatNum;
        SetPlayersRole(GolbalVar.curBtnSeat);
    }

    public void SetPlayersRole(int btn)
    {
        activePlayers.Sort((a, b) => {
            return a.seatNum - b.seatNum;
        });
        
        foreach (Player p in activePlayers)
        {
            p.role = Player.PlayerRole.normal;
        }

        activePlayers[btn].role = Player.PlayerRole.button;
        activePlayers[(btn + 1) % totalSeatNum].role = Player.PlayerRole.smallBlind;
        if (totalSeatNum >= 3)
        {
            activePlayers[(btn + 2) % totalSeatNum].role = Player.PlayerRole.bigBlind;
        }

        SortPlayers();
    }

    public void SortPlayers()
    {
        for(int i = 0; i<GolbalVar.curBtnSeat; i++)
        {
            Player p = activePlayers[i];
            activePlayers.Remove(p);
            activePlayers.Add(p);
        }
    }

    public Player FindPlayer(int seatNum)
    {
        foreach(Player p in seatedPlayers)
        {
            if (p.seatNum == seatNum)
            {
                return p;
            }
        }
        return null;
    }
}
