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

    public List<Player> allPlayers = new List<Player>();        //所有玩家的列表
    public List<Player> seatedPlayers = new List<Player>();     //被选中入座玩家列表，包括FOLD和已经破产的
    public List<Player> activePlayers = new List<Player>();     //当前还在本局游戏中的玩家列表
    public int totalSeatNum;    //当前回合一共入座的玩家数，包括已经FOLD的不包括已经破产的
    public int nowPlayerIndex;  //当前下注玩家序号

    /// <summary>
    /// 初始化所有玩家
    /// </summary>
    /// <param name="nameList">玩家的名字列表</param>
    public void InitPlayers(List<string> nameList)
    {
        foreach (string name in nameList)
        {
            Player p = new Player(name);
            allPlayers.Add(p);
        }
    }

    /// <summary>
    /// 判断选择玩家的数量是否符合规范并且赋予座位号
    /// </summary>
    /// <returns>如果玩家数符合要求则RETURN TRUE并进入下一个游戏阶段,否则是FALSE</returns>
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
            }
        }
        if (totalSeatNum < 2 || totalSeatNum > 8)
        {
            if (totalSeatNum < 2)
            {
                Debug.Log("玩家人数未达到2人，请更改选择");
                InitialPanelManager.instance.CallStartErrorLog("玩家人数不能小于2人！");
            } else
            {
                Debug.Log("玩家人数超过上限8人，请更改选择");
                InitialPanelManager.instance.CallStartErrorLog("玩家人数不能超过8人！");
            }
            foreach (Player pl in seatedPlayers)
            {
                pl.seatNum = -1;
                pl.isInGame = false;
            }
            seatedPlayers.Clear();
            InitialPanelManager.instance.ResetAllTheButtons();
            return false;
        }
        return true;
    }

    /// <summary>
    /// 每一轮新游戏开始前判定玩家的财富值修改ACTIVE PLAYERS
    /// </summary>
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

    /// <summary>
    /// 给所有在当局游戏中的玩家设置ROLE
    /// </summary>
    /// <param name="btn">庄家的座位号</param>
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
        if (activePlayers.Count >= 3)
        {
            activePlayers[(btn + 2) % totalSeatNum].role = Player.PlayerRole.bigBlind;
        }

        SortPlayers();
    }
    public bool ActivePlayers()
    {
        activePlayers = new List<Player>();
        totalSeatNum = 0;
        foreach (Player p in seatedPlayers)
        {
            if (!p.isFold)
            {
                activePlayers.Add(p);
            }
        }
        return true;
    }

    /// <summary>
    /// 通过PLAY ROLE来修改活跃玩家的行动顺序，小盲大盲在最前面，庄家在最后
    /// </summary>
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
    public void BetAction(Player p)
    {
        switch (p.state)
        {
            case 0://preflolp阶段处理小盲，大盲
                //小盲注
                if (p.role == Player.PlayerRole.smallBlind)
                {
                    p.betCoin = GolbalVar.minBetCoin;
                    p.coin -= p.betCoin;
                    GolbalVar.maxBetCoin = p.betCoin;
                    GolbalVar.pot += p.betCoin;
                    string strbet = p.playerName + "选择【小盲注】，剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                    Debug.Log(strbet);
                    UIManager.instance.PrintLog(strbet);
                }
                else if (p.role == Player.PlayerRole.bigBlind)
                {
                    p.betCoin = 2 * GolbalVar.minBetCoin;
                    p.coin -= p.betCoin;
                    GolbalVar.maxBetCoin = p.betCoin;
                    string strbet = p.playerName + "选择【大盲注】，剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                    Debug.Log(strbet);
                    UIManager.instance.PrintLog(strbet);
                }
                break;

            case 1://跟注
                //剩余金额能否跟注
                if (p.coin + p.betCoin >= GolbalVar.maxBetCoin) //够钱
                {
                    string strbet;
                    if (p.betCoin == GolbalVar.maxBetCoin)  //已经是
                    {
                        strbet = p.playerName + "选择【过牌】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                    }
                    else
                    {
                        p.coin -= GolbalVar.maxBetCoin - p.betCoin;
                        GolbalVar.pot += GolbalVar.maxBetCoin - p.betCoin;
                        p.betCoin = GolbalVar.maxBetCoin;
                        strbet = p.playerName + "选择【跟注】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;

                    }
                                    
                    Debug.Log(strbet);
                    UIManager.instance.PrintLog(strbet);
                }
                else if (p.role == Player.PlayerRole.bigBlind)
                {
                    p.betCoin = 2 * GolbalVar.minBetCoin;
                    p.coin -= p.betCoin;
                    GolbalVar.maxBetCoin = p.betCoin;
                    string strbet = p.playerName + "选择【大盲注】，剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                    Debug.Log(strbet);
                    UIManager.instance.PrintLog(strbet);
                }
                break;

        }
    }
    //返回值说明
    //0 仅剩一名玩家，游戏结束
    //
    public int PlayerBet(List<Player> pList, int playerIndex)
    {
        //bool thisPlayerIsFold = false;
        if (nowPlayerIndex == pList.Count)
            nowPlayerIndex = 0;
        if (nowPlayerIndex == 0)
        {
            Debug.Log("新一轮下注开始");
        }
        if (CalcFoldNum(pList) == pList.Count-1 && pList[playerIndex].isFold==false)
        {
            Debug.Log("除了" + pList[playerIndex].playerName + "，其余玩家均弃权");
            return 0;
        }

        return 1;
    }
}
