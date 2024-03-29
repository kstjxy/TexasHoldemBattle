using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Net.Sockets;

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
    public List<Player> lostPlayers = new List<Player>();
    public int totalSeatNum;    //当前回合一共入座的玩家数，包括已经FOLD的不包括已经破产的
    public int nowPlayerIndex;  //当前下注玩家序号
    public string UIUpdateString = ""; //

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
            if (p.coin < 2 * GlobalVar.minBetCoin)
            {
                if (!lostPlayers.Contains(p))
                {
                    UIManager.instance.PrintThread("【" + p.playerName + "】金币数量不足，退出接下来的游戏回合");
                }
                RemovePlayer(p);
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
        PlayerManager.instance.StatJudge();
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
            activePlayers[(btn + 1) % activePlayers.Count].role = Player.PlayerRole.smallBlind;
            activePlayers[(btn + 2) % activePlayers.Count].role = Player.PlayerRole.bigBlind;
        }
        else
        {
            activePlayers[(btn) % activePlayers.Count].role = Player.PlayerRole.smallBlind;
            activePlayers[(btn + 1) % activePlayers.Count].role = Player.PlayerRole.bigBlind;
        }

        foreach (Player p in activePlayers)
        {
            p.playerObject.BackToWaiting_AvatarChange();
            if (p.role == Player.PlayerRole.bigBlind || p.role == Player.PlayerRole.smallBlind)
            {
                p.state = 1;
            }
        }
        SortPlayers();
    }

    /// <summary>
    /// 通过PLAY ROLE来修改活跃玩家的行动顺序，小盲大盲在最前面，庄家在最后
    /// </summary>
    public void SortPlayers()
    {
        for (int i = 0; i <= GlobalVar.curBtnSeat; i++)
        {
            Player p = activePlayers[0];
            activePlayers.Remove(p);
            activePlayers.Add(p);
        }
    }

    /// <summary>
    /// 返回当前选择弃牌的玩家数量
    /// </summary>
    /// <returns>弃牌的玩家数</returns>
    public int CalcFoldNum()
    {
        int num = 0;
        foreach (Player p in activePlayers)
        {
            if (p.isFold == true)
                num++;
        }
        return num;
    }

    //玩家的具体操作
    // 0 初始化大盲小盲
    // 1 跟注 或 过牌
    // 2 加注
    // 3 弃牌
    // 4 ALL IN
    /// <summary>
    /// 处理玩家出牌操作对应游戏逻辑的方法
    /// </summary>
    /// <param name="p">正在执行出牌操作的玩家</param>
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
                        p.betCoin = GlobalVar.minBetCoin;
                        p.coin -= p.betCoin;
                        GlobalVar.maxBetCoin = p.betCoin;
                        GlobalVar.pot += p.betCoin;
                        UIManager.instance.UpdateCoinsPool();
                        UIManager.instance.BetCoinsEffect(p, p.betCoin);
                        p.playerObject.UpdateBetCoinsCount();
                        p.playerObject.UpdateCoinsCount();
                        strbet = p.playerName + "【小盲注】剩余金额" + p.coin + "，最大押注" + GlobalVar.maxBetCoin + "，底池金额" + GlobalVar.pot;
                        Debug.Log(strbet);
                        UIManager.instance.PrintThread(strbet);
                    }
                    else if (p.role == Player.PlayerRole.bigBlind)
                    {
                        p.betCoin = 2 * GlobalVar.minBetCoin;
                        p.coin -= p.betCoin;
                        GlobalVar.maxBetCoin = p.betCoin;
                        GlobalVar.pot += p.betCoin;
                        UIManager.instance.UpdateCoinsPool();
                        UIManager.instance.BetCoinsEffect(p, p.betCoin);
                        p.playerObject.UpdateBetCoinsCount();
                        p.playerObject.UpdateCoinsCount();
                        strbet = p.playerName + "【大盲注】剩余金额" + p.coin + "，最大押注" + GlobalVar.maxBetCoin + "，底池金额" + GlobalVar.pot;
                        Debug.Log(strbet);
                        UIManager.instance.PrintThread(strbet);
                    }
                    p.state = 1;
                    break;
                }

            case 1://跟注
                {
                    //剩余金额能否跟注
                    if (p.coin + p.betCoin > GlobalVar.maxBetCoin) //够钱
                    {
                        if (p.betCoin == GlobalVar.maxBetCoin)  //已经是跟注了 即为过牌
                        {
                            strbet = p.playerName + "【过牌】";
                        }
                        else //跟注
                        {
                            int change = GlobalVar.maxBetCoin - p.betCoin;
                            p.coin -= change;
                            GlobalVar.pot += change;
                            p.betCoin = GlobalVar.maxBetCoin;
                            UIManager.instance.UpdateCoinsPool();
                            UIManager.instance.BetCoinsEffect(p, change);
                            p.playerObject.UpdateBetCoinsCount();
                            p.playerObject.UpdateCoinsCount();
                            strbet = p.playerName + "【跟注】，剩余金额" + p.coin + "，底池金额" + GlobalVar.pot;

                        }

                        Debug.Log(strbet);
                        UIManager.instance.PrintThread(strbet);
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
                    if (GlobalVar.curBetCount > GlobalVar.maxBetCount)
                    {
                        strbet = "本回合加注次数已达" + GlobalVar.maxBetCount + "次，" + p.playerName + "本次操作非法，默认为跟注";
                        p.state = 1;
                        BetAction(p);
                    }
                    else
                    {
                        //判断钱够不够
                        if (p.coin + p.betCoin - GlobalVar.maxBetCoin - 2 * GlobalVar.minBetCoin > 0)
                        {
                            int change = GlobalVar.maxBetCoin - p.betCoin + 2 * GlobalVar.minBetCoin;
                            p.coin -= change;
                            p.betCoin += change;
                            GlobalVar.pot += change;
                            GlobalVar.maxBetCoin = p.betCoin;
                            UIManager.instance.UpdateCoinsPool();
                            UIManager.instance.BetCoinsEffect(p, change);
                            p.playerObject.UpdateBetCoinsCount();
                            p.playerObject.UpdateCoinsCount();
                            strbet = p.playerName + "【加注】，剩余金额" + p.coin + "，最大押注" + GlobalVar.maxBetCoin + "，底池金额" + GlobalVar.pot;
                            Debug.Log(strbet);
                            UIManager.instance.PrintThread(strbet);
                            GlobalVar.curBetCount++;
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
                    if (p.betCoin == GlobalVar.maxBetCoin)
                    {
                        p.state = 1;
                        BetAction(p);
                    }
                    else
                    {
                        p.isFold = true;
                        strbet = p.playerName + "【弃牌】，剩余金额" + p.coin;
                        p.playerObject.NoMoreActions_AvatarChange();
                        Debug.Log(strbet);
                        UIManager.instance.PrintThread(strbet);
                    }
                    break;
                }

            case 4://ALL IN
                {
                    int change = p.coin;
                    p.betCoin += change;
                    p.isAllIn = true;
                    GlobalVar.pot += change;
                    p.coin = 0;
                    strbet = p.playerName + "【ALL_IN】";
                    if (p.betCoin < GlobalVar.maxBetCoin)
                    {
                        strbet += "由于玩家的ALL_IN下注金额" + p.betCoin + "小于最大押注金额，最大押注金额不变";
                    }
                    else
                    {
                        GlobalVar.maxBetCoin = p.betCoin;
                    }
                    UIManager.instance.UpdateCoinsPool();
                    UIManager.instance.BetCoinsEffect(p, change);
                    p.playerObject.UpdateBetCoinsCount();
                    p.playerObject.UpdateCoinsCount();
                    strbet += "，最大押注为" + GlobalVar.maxBetCoin + "，底池金额" + GlobalVar.pot;
                    Debug.Log(strbet);
                    UIManager.instance.PrintThread(strbet);

                    break;
                }
        }
    }

    /// <summary>
    /// AI脚本接口，处理脚本返回的出牌操作
    /// </summary>
    /// <param name="p">执行出牌操作的玩家</param>
    public void Bet(Player p)
    {
        if (!(p.state == 0 && (p.role == Player.PlayerRole.smallBlind || p.role == Player.PlayerRole.bigBlind))) 
        {
            try
            {
                //AI 的接口
                if (p.type == Player.aiType.WebAI)
                    p.state = p.webAI.BetAction();
                else
                    p.state = p.luaAI.BetAction();
                RecordManager.instance.CallActionRecord(p.seatNum,p.state);
            }
            catch (SocketException e)
            {
                string bug = "与客户端【" + p.playerName + "】沟通失败，可能为连接断开或超时（5S） " + e.Message;
                Debug.Log(bug.Substring(0, bug.IndexOf('\0')));
                UIManager.instance.PrintLog(bug);
                p.webAI.CloseSocket();
                Debug.Log("关闭此客户端的连接");
                UIManager.instance.PrintLog("关闭此客户端的连接");
                RemovePlayer(p);
            }
            catch (Exception e)
            {
                Debug.Log(p.playerName + "出牌失败，原因：" + e.Message);
                UIManager.instance.PrintThread(p.playerName + "AI脚本不符合规范，被移出游戏");
                RemovePlayer(p);
            }
        }
        BetAction(p);
    }

    /// <summary>
    /// 返回最后进入算牌阶段的玩家列表
    /// </summary>
    /// <returns>没有弃牌准备算牌的玩家列表</returns>
    public List<Player> GetFinalPlayers()
    {
        List<Player> final = new List<Player>();
        foreach (Player p in activePlayers)
        {
            if (!p.isFold)
            {
                final.Add(p);
            }
        }
        return final;
    }

    /// <summary>
    ///每一局游戏结束后返回金币数最大的玩家列表
    /// </summary>
    /// <returns>金币最多的玩家列表/最终赢家</returns>
    public List<Player> GetFinalWinners()
    {
        List<Player> result = new List<Player>();
        List<Player> pList = GetRankedPlayers();
        result.Add(pList[0]);
        for (int i = 1; i < pList.Count; i++)
        {
            if (pList[i].coin == result[0].coin)
            {
                result.Add(pList[i]);
            } else
            {
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 将玩家通过coin的数值进行排序
    /// </summary>
    /// <returns>通过coin大小经过排序的玩家list</returns>
    public List<Player> GetRankedPlayers()
    {
        List<Player> pList = new List<Player>();
        foreach (Player p in seatedPlayers)
        {
            pList.Add(p);
        }
        pList.Sort((a, b) =>
        {
            return b.coin - a.coin;
        });
        return pList;
    }

    /// <summary>
    /// 将玩家从当前游戏（之后的全部回合）中移除
    /// </summary>
    /// <param name="p">要移出游戏的玩家</param>
    public void RemovePlayer(Player p)
    {
        p.OutOfGame();
        p.playerObject.QuitTheGame_AvatarChange();
        totalSeatNum--;
        //activePlayers.Remove(p);
        p.isFold = true;
        //seatedPlayers.Remove(p);
        lostPlayers.Add(p);
        if (allPlayers.Count < 2 || activePlayers.Count < 2)
        {
            UIManager.instance.PrintThread("场上剩余玩家数不足开始新游戏，本局游戏提前结束！");
            GlobalVar.gameStatusCounter = 6;
            GameManager.instance.GameOver();
        }
    }
    public void StatJudge()
    {
        for (int i = 0; i < seatedPlayers.Count; i++)
        {
            Player p = seatedPlayers[i];
            if(p.role == Player.PlayerRole.outOfGame)
            {
                i--;
                seatedPlayers.Remove(p);
            }
        }
        if (seatedPlayers.Count < 2)
        {
            GameManager.instance.GameOver();
        }
    }

    /// <summary>
    /// 将玩家进行排名，相同数量coin拥有者名次相等
    /// </summary>
    /// <param name="pList">已经排序完毕的玩家list</param>
    /// <returns>玩家的排名列表</returns>
    public List<int> GetPlayerRank(List<Player> pList)
    {
        List<int> rankNum = new List<int>();
        int curRank = 1;
        int cumm = 0;
        int prevCumm = 0;
        rankNum.Add(curRank);
        for (int i = 1; i < pList.Count; i++)
        {
            if (pList[i - 1].coin != pList[i].coin)
            {
                curRank++;
                prevCumm = cumm;
                cumm = 0;
            }
            else
            {
                cumm++;
            }
            if (prevCumm != 0)
            {
                curRank += prevCumm;
                prevCumm = 0;
            }
            rankNum.Add(curRank);
        }
        return rankNum;
    }
}
