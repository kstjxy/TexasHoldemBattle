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
    public int nowPlayerIndex = 0; //当前的执行的玩家

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
    public int CalcFoldNum(List<Player> pList)
    {
        int num = 0;
        foreach(Player p in pList)
        {
            if (p.isFold == true)
                num++;
        }
        return num;
    }
    private void BetAction(Player p)
    {
        switch (p.state){
            case 0:
                //小盲注
                if (p.role == Player.PlayerRole.smallBlind)
                {

                }
                break;

        }
    }
    //返回值说明
    //0 仅剩一名玩家，游戏结束
    //
    public int PlayerBet(List<Player> pList, int playerIndex)
    {
        bool thisPlayerIsFold = false;
        if (nowPlayerIndex == pList.Count)
            nowPlayerIndex = 0;
        if (nowPlayerIndex == 0)
        {
            Debug.Log("新一轮下注开始")
        }
        if (CalcFoldNum(pList) == pList.Count-1 && pList[playerIndex].isFold==false)
        {
            Debug.Log("除了" + pList[playerIndex].playerName + "，其余玩家均弃权")：
            return 0;
        }

        return 1;
    }
}
