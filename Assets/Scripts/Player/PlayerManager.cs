using System.Collections;
using System.Collections.Generic;
using System;
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
            }
            else
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
            if (p.coin <= 2 * GolbalVar.minBetCoin)
            {
                p.OutOfGame();
                totalSeatNum--;
                activePlayers.Remove(p);
            }
            else
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
        }
        else
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
        for (int i = 0; i <= GolbalVar.curBtnSeat; i++)
        {
            Player p = activePlayers[0];
            activePlayers.Remove(p);
            activePlayers.Add(p);
        }
    }
    public Player FindPlayer(int seatNum)
    {
        foreach (Player p in seatedPlayers)
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
        foreach (Player p in pList)
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
                        UIManager.instance.UpdateCoinsPool();
                        UIManager.instance.BetCoinsEffect(p, p.betCoin);
                        p.playerObject.UpdateBetCoinsCount();
                        p.playerObject.UpdateCoinsCount();
                        
                     
                        strbet = p.playerName + "\t选择【小盲注】，剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                        Debug.Log(strbet);
                        UIManager.instance.PrintLog(strbet);
                    }
                    else if (p.role == Player.PlayerRole.bigBlind)
                    {
                        p.betCoin = 2 * GolbalVar.minBetCoin;
                        p.coin -= p.betCoin;
                        GolbalVar.maxBetCoin = p.betCoin;
                        GolbalVar.pot += p.betCoin;
                        UIManager.instance.UpdateCoinsPool();
                        UIManager.instance.BetCoinsEffect(p, p.betCoin);
                        p.playerObject.UpdateBetCoinsCount();
                        p.playerObject.UpdateCoinsCount();
                        strbet = p.playerName + "\t选择【大盲注】，剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
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
                            strbet = p.playerName + "\t选择【过牌】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                        }
                        else //跟注
                        {
                            int change = GolbalVar.maxBetCoin - p.betCoin;
                            p.coin -= change;
                            GolbalVar.pot += change;
                            p.betCoin = GolbalVar.maxBetCoin;
                            UIManager.instance.UpdateCoinsPool();
                            UIManager.instance.BetCoinsEffect(p, change);
                            p.playerObject.UpdateBetCoinsCount();
                            p.playerObject.UpdateCoinsCount();
                            strbet = p.playerName + "\t选择【跟注】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;

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
                        if (p.coin + p.betCoin - GolbalVar.maxBetCoin - 2 * GolbalVar.minBetCoin > 0)
                        {
                            int change = GolbalVar.maxBetCoin - p.betCoin + 2 * GolbalVar.minBetCoin;
                            p.coin -= change;
                            p.betCoin += change;
                            GolbalVar.pot += change;
                            GolbalVar.maxBetCoin = p.betCoin;
                            UIManager.instance.UpdateCoinsPool();
                            UIManager.instance.BetCoinsEffect(p, change);
                            p.playerObject.UpdateBetCoinsCount();
                            p.playerObject.UpdateCoinsCount();
                            strbet = p.playerName + "\t选择【加注】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                            Debug.Log(strbet);
                            UIManager.instance.PrintLog(strbet);
                            GolbalVar.curBetCount++;
                        }
                        //钱不够即为ALL IN
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
                    //如果当前下注的钱已经是最大押注，无需弃牌
                    if (p.betCoin == GolbalVar.maxBetCoin)
                    {
                        p.state = 1;
                        BetAction(p);
                    }
                    else
                    {
                        p.isFold = true;
                        strbet = p.playerName + "\t选择【弃牌】，当前剩余金额为" + p.coin + "，当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                        Debug.Log(strbet);
                        UIManager.instance.PrintLog(strbet);
                    }
                    break;
                }

            case 4://ALL IN
                {
                    int change = p.coin;
                    p.betCoin  += change;
                    p.isAllIn = true;
                    GolbalVar.pot += change;
                    p.coin = 0;
                    strbet = p.playerName + "\t选择【ALL IN】，当前剩余金额为" + p.coin;
                    if (p.betCoin < GolbalVar.maxBetCoin)
                    {
                        strbet += "由于当前玩家的ALL IN下注金额" + p.betCoin + "小于当前最大押注金额，最大押注金额不变";
                    }
                    else
                    {
                        GolbalVar.maxBetCoin = p.betCoin;
                    }
                    UIManager.instance.UpdateCoinsPool();
                    UIManager.instance.BetCoinsEffect(p, change);
                    p.playerObject.UpdateBetCoinsCount();
                    p.playerObject.UpdateCoinsCount();
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
        System.Random randint = new System.Random(Guid.NewGuid().GetHashCode());
        int ranNum = randint.Next(1, 101);
        //Debug.Log("随机值为：" + ranNum);
        //50% 跟注 或 过
        if (ranNum <= 50) return 1;
        //25% 加注
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
        {
            //AI 的接口
            //当前为随机
            p.state = RandomAction();
        }
        BetAction(p);
    }
    //返回值说明
    //-1    
    //0     仅剩一名玩家，游戏结束
    //1
     public IEnumerator PlayerBet()
     {
        List<Player> pList = activePlayers;
        int playerIndex = 0;
        int lastPlayer = 0;
        string strbet;
        bool flag = false;
        do
        {            
            if (playerIndex == pList.Count)
            {
                playerIndex = 0;
                flag = true;
            }


            if (pList[playerIndex].isFold == true || pList[playerIndex].isAllIn == true)
            {
                pList[lastPlayer].playerObject.BackToWaiting_AvatarChange();
                pList[playerIndex].playerObject.HightLightAction_AvatarChange();
                strbet = pList[playerIndex].playerName + "已经弃牌/ALL IN，不做操作";
                Debug.Log(strbet);
                UIManager.instance.PrintLog(strbet);
                lastPlayer = playerIndex;
                playerIndex++;
                continue;
            }

            //preflop时大盲后一位开始
            if (pList[playerIndex].state == 0)
            {
                if (pList[playerIndex].role == Player.PlayerRole.smallBlind || pList[playerIndex].role == Player.PlayerRole.bigBlind)
                {
                    pList[playerIndex].state = 1;
                    playerIndex++;
                    continue;
                }
            }

            if (playerIndex == 0)
            {
                Debug.Log("新一轮下注开始");
                UIManager.instance.PrintLog("新一轮下注开始");
            }

            pList[lastPlayer].playerObject.BackToWaiting_AvatarChange();
            pList[playerIndex].playerObject.HightLightAction_AvatarChange();
            lastPlayer = playerIndex;
            if (CalcFoldNum(pList) == pList.Count - 1)
            {
                strbet = "除了" + pList[playerIndex].playerName + "，其余玩家均弃权。\n当前最大押注为" + GolbalVar.maxBetCoin + "，当前底池的金额为" + GolbalVar.pot;
                Debug.Log(strbet);
                UIManager.instance.PrintLog(strbet);
                //0
                GolbalVar.gameStatusCounter = 5;
                pList[lastPlayer].playerObject.BackToWaiting_AvatarChange();
                break;
            }

            Bet(pList[playerIndex]);


            playerIndex++;
            if (playerIndex >= pList.Count)
            {
                strbet = "底池：" + GolbalVar.pot + "，最大下注金额：" + GolbalVar.maxBetCoin;
                Debug.Log(strbet);
                UIManager.instance.PrintLog(strbet);
            }

            okPlayers.Clear();
            foreach (Player p in pList)
                if (p.isAllIn == true || p.betCoin == GolbalVar.maxBetCoin || p.isFold == true)
                    okPlayers.Add(p);
            //必须强制下注一轮 用flag限制
            if (okPlayers.Count == pList.Count && (flag || playerIndex == pList.Count))
            {
                strbet = "本轮下注结束";
                Debug.Log(strbet);
                UIManager.instance.PrintLog(strbet);
                nowPlayerIndex = 0;
                pList[lastPlayer].playerObject.BackToWaiting_AvatarChange();
                //1
                break;
            }
            yield return new WaitForSeconds(GolbalVar.speedFactor * 5);

        } while (true);

    }
}
