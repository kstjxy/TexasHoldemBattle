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
    public List<Player> okPlayers = new List<Player>();         //本轮满足条件的玩家
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
            p.ai = new BaseAI();
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
            if (p.coin <= 2*GolbalVar.minBetCoin)
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

        if (activePlayers.Count >= 3)
        {
            activePlayers[btn].role = Player.PlayerRole.button;
            activePlayers[(btn + 1) % totalSeatNum].role = Player.PlayerRole.smallBlind;
            activePlayers[(btn + 2) % totalSeatNum].role = Player.PlayerRole.bigBlind;
        } else
        {
            activePlayers[(btn) % totalSeatNum].role = Player.PlayerRole.smallBlind;
            activePlayers[(btn + 1) % totalSeatNum].role = Player.PlayerRole.bigBlind;
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
        for(int i = 0; i<=GolbalVar.curBtnSeat; i++)
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

    //玩家的具体操作
    //-1 初始化大盲小盲
    // 1 跟注 或 过牌
    // 2 加注
    // 3 弃牌
    // 4 ALL IN
    public void BetAction(Player p)
    {
        string strbet;
        switch (p.state)
        {
            case 0://preflolp阶段处理小盲，大盲
                {
                    //小盲注
                    if (p.role == Player.PlayerRole.smallBlind)
                    {
                        p.betCoin = GolbalVar.minBetCoin;
                        p.coin -= p.betCoin;
                        GolbalVar.maxBetCoin = p.betCoin;
                        GolbalVar.pot += p.betCoin;
                        strbet = p.playerName + "选择【小盲注】，剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                        Debug.Log(strbet);
                        UIManager.instance.PrintLog(strbet);
                    }
                    else if (p.role == Player.PlayerRole.bigBlind)
                    {
                        p.betCoin = 2 * GolbalVar.minBetCoin;
                        p.coin -= p.betCoin;
                        GolbalVar.maxBetCoin = p.betCoin;
                        strbet = p.playerName + "选择【大盲注】，剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                        Debug.Log(strbet);
                        UIManager.instance.PrintLog(strbet);
                    }
                    break;
                }
                
            case 1://跟注
                {
                    //剩余金额能否跟注
                    if (p.coin + p.betCoin >= GolbalVar.maxBetCoin) //够钱
                    {
                        if (p.betCoin == GolbalVar.maxBetCoin)  //已经是跟注了 即为过牌
                        {
                            strbet = p.playerName + "选择【过牌】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                        }
                        else //跟注
                        {
                            p.coin -= GolbalVar.maxBetCoin - p.betCoin;
                            GolbalVar.pot += GolbalVar.maxBetCoin - p.betCoin;
                            p.betCoin = GolbalVar.maxBetCoin;
                            strbet = p.playerName + "选择【跟注】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;

                        }

                        Debug.Log(strbet);
                        UIManager.instance.PrintLog(strbet);
                    }
                    else //钱不够 还要跟注 此时为allin
                    {
                        p.state = 4;
                        BetAction(p);
                    }
                    break;
                }
                
            case 2://加注
                {
                    //判断操作是否合法
                    if (GolbalVar.curBetCount > GolbalVar.maxBetCount)
                    {
                        strbet = "本回合加注次数已达" + GolbalVar.maxBetCount + "次，" + p.playerName + "本次操作非法，默认为跟注";
                        p.state = 1;
                        BetAction(p);
                    }
                    else
                    {
                        //判断钱够不够
                        if (p.coin + p.betCoin - GolbalVar.maxBetCoin + 2 * GolbalVar.minBetCoin > 0)
                        {
                            p.coin -= 2 * GolbalVar.minBetCoin;
                            p.betCoin += 2 * GolbalVar.minBetCoin;
                            GolbalVar.pot += 2 * GolbalVar.minBetCoin;
                            strbet = p.playerName + "选择【加注】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                            Debug.Log(strbet);
                            UIManager.instance.PrintLog(strbet);
                        }
                        else
                        {
                            p.state = 4;
                            BetAction(p);
                        }
                    }
                    break;
                }
                
            case 3://弃牌
                {
                    p.isFold = true;
                    strbet = p.playerName + "选择【弃牌】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                    Debug.Log(strbet);
                    UIManager.instance.PrintLog(strbet);
                    break;
                }
               
            case 4://ALL IN
                {
                    p.betCoin = p.betCoin + p.coin;
                    p.isAllIn = true;                    
                    GolbalVar.pot += p.coin;
                    p.coin = 0;
                    strbet = p.playerName + "选择【ALL IN】，当前剩余金额为" + p.coin;
                    if (p.betCoin < GolbalVar.maxBetCoin)
                    {
                        strbet += "由于当前玩家的ALL IN下注金额" + p.betCoin + "小于当前最大押注金额，最大押注金额不变";
                    }
                    else
                    {
                        GolbalVar.maxBetCoin = p.betCoin;  
                    }
                    strbet += "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                    Debug.Log(strbet);
                    UIManager.instance.PrintLog(strbet);

                    break;
                }                
        }
    }
    
    //用于测试的随机行动
    public int RandomAction()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        int ranNum = Random.Range(1, 101);
        //60% 跟注 或 过
        if (ranNum <= 60) return 1;
        //15% 加注
        if (ranNum <= 75) return 2;
        //20% 弃牌
        if (ranNum <= 95) return 3;
        //5% ALL IN
        return 4;

    }
    public void Bet(Player p)
    {
        if (p.state == -1)
            p.state = 0;
        else
            p.state = RandomAction();
        BetAction(p);
    }
    //返回值说明
    //-1    
    //0     仅剩一名玩家，游戏结束
    //1
    public int PlayerBet()
    {
        List<Player> pList = activePlayers;
        int playerIndex = 0;
        string strbet;
        do
        {
            if (pList[playerIndex].isFold == true || pList[playerIndex].isAllIn == true)
            {
                strbet = pList[playerIndex].playerName + "已经弃牌/ALL IN，不做操作";
                Debug.Log(strbet);
                UIManager.instance.PrintLog(strbet);
                continue;
            }

            if (playerIndex == pList.Count)
                playerIndex = 0;
            if (playerIndex == 0)
            {
                Debug.Log("新一轮下注开始");
                UIManager.instance.PrintLog("新一轮下注开始");
            }

            if (CalcFoldNum(pList) == pList.Count - 1)
            {
                strbet = "除了" + pList[playerIndex].playerName + "，其余玩家均弃权。\n当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                Debug.Log(strbet);
                UIManager.instance.PrintLog(strbet);
                return 0;
            }
            Bet(pList[playerIndex]);

            
            playerIndex++;
            if (playerIndex >= pList.Count)
            {
                strbet = "底池：" + GolbalVar.pot + "最大下注金额：" + GolbalVar.maxBetCoin;
                Debug.Log(strbet);
                UIManager.instance.PrintLog(strbet);
            }

            okPlayers.Clear();
            foreach (Player p in pList)
                if (p.isAllIn == true || p.betCoin == GolbalVar.maxBetCoin || p.isFold == true)
                    okPlayers.Add(p);
            if (okPlayers.Count == pList.Count)
            {
                strbet = "第" + GolbalVar.curRoundNum + "回合结束";
                Debug.Log(strbet);
                UIManager.instance.PrintLog(strbet);
                nowPlayerIndex = 0;
                return 1;
            }
        } while (true);     
    }
}
