using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // 单例
    public static GameManager instance;
    void Awake()
    {
        instance = this;
    }

    //游戏进程
    public enum GameState
    {
        setting,    //设置界面，调整游戏规则
        init,       //初始化，切换界面，玩家入座
        roundInit,  //每一局开始的INIT，确定PLAYER ROLES
        preflop,    //发给玩家两张牌 第一轮下注
        flop,       //公开卡池三张牌 第二轮下注
        turn,       //公开卡池第四张牌 第三轮下注
        river,      //公开卡池第四张牌 第四轮下注
        result,     //一局游戏结束显示结果，调整筹码和排行
        gameover    //设定的所有局结束
    }

    public static float timer = 0;
    public int curPlayerSeat = -1;
    public Player curPlayer = null;
    public bool playersInAction = false;
    public List<Player> winners = new List<Player>();
    public List<Player> passablePlayer = new List<Player>();
    public bool flag = false;
    public List<Player> finalPlayers = new List<Player>();

    public static GameState GameStatus()
    {
        switch (GlobalVar.gameStatusCounter)
        {
            case -2:
                return GameState.setting;
            case -1:
                return GameState.init;
            case 0:
                return GameState.roundInit;
            case 1:
                return GameState.preflop;
            case 2:
                return GameState.flop;
            case 3:
                return GameState.turn;
            case 4:
                return GameState.river;
            case 5:
                return GameState.result;
            case 6:
                return GameState.gameover;
        }
        throw new UnityException("GameStatus error");
    }

    public void GameUpdate()
    {
        switch (GlobalVar.gameStatusCounter)
        {
            case -1:
                Init();
                break;
            case 0:
                RoundInit();
                break;
            case 1:
                Preflop();
                break;
            case 2:
                Flop();
                break;
            case 3:
                Turn();
                break;
            case 4:
                River();
                break;
            case 5:
                Result();
                break;
            case 6:
                GameOver();
                break;
        }
    }

    public void Setting()
    {
        if (PlayerManager.instance.SeatPlayers())
        {
            GlobalVar.gameStatusCounter++;
        }
    }

    public void Restart()
    {
        GlobalVar.gameStatusCounter = -2;
        GlobalVar.curRoundNum = 0;
    }

    public void Init()
    {
        foreach (Player p in PlayerManager.instance.seatedPlayers)
        {
            p.coin = GlobalVar.initCoin;
        }
        PlayerManager.instance.lostPlayers = new List<Player>();
        UIManager.instance.UpdateRankingList();
        GlobalVar.curRoundNum++;
        GlobalVar.gameStatusCounter++;
    }

    public void RoundInit()
    {
        UIManager.instance.ClearAllCards();
        UIManager.instance.PrintLog("新一轮游戏开始！当前为第【" + GlobalVar.curRoundNum + "】轮");
        UIManager.instance.UpdateGameRounds();
        PlayerManager.instance.NewRound();
        GlobalVar.curBtnSeat = (GlobalVar.curBtnSeat + 1) % PlayerManager.instance.activePlayers.Count;
        PlayerManager.instance.SetPlayersRole(GlobalVar.curBtnSeat);
        UIManager.instance.PrintLog("位置分配完毕！");
        if (PlayerManager.instance.activePlayers.Count >= 3)
        {
            UIManager.instance.PrintLog("【" + PlayerManager.instance.activePlayers[PlayerManager.instance.activePlayers.Count - 1].playerName + "】为庄家位");
            UIManager.instance.PrintLog("【" + PlayerManager.instance.activePlayers[0].playerName + "】为小盲位");
        }
        else
        {
            UIManager.instance.PrintLog("【" + PlayerManager.instance.activePlayers[0].playerName + "】为庄家和小盲位");
        }
        UIManager.instance.PrintLog("【" + PlayerManager.instance.activePlayers[1].playerName + "】为大盲位");

        curPlayerSeat = -1;
        CardManager.instance.InitialCardsList();
        GlobalVar.gameStatusCounter++;
    }

    public void Preflop()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("当前为【前翻牌圈】");
                CardManager.instance.AssignCardsToPlayers();
                UIManager.instance.PrintLog("每个在游戏中的玩家获得两张手牌");
                foreach(Player p in PlayerManager.instance.activePlayers)
                {
                    UIManager.instance.PrintLog("【" + p.playerName + "】的手牌为：【" + p.playerCardList[0].PrintCard() + "】【" + p.playerCardList[1].PrintCard() + "】");
                    p.state = 0;
                }
                flag = false;
            }

        }
        else
        {
            UpdateCurPlayer();
        }
    }

    public void Flop()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("当前为【翻牌圈】");
                CardManager.instance.AssignCardsToTable(3);
                for (int i = 0; i < 3; i++)
                {
                    UIManager.instance.ShowCommunityCard(GlobalVar.publicCards[i], i);
                }
                UIManager.instance.PrintLog("公共卡池发出前三张牌，分别为：\n【" + GlobalVar.publicCards[0].PrintCard() + "】【" +
                    GlobalVar.publicCards[1].PrintCard() + "】【" + GlobalVar.publicCards[2].PrintCard() + "】");
                flag = false;
            }
        }
        else
        {
            UpdateCurPlayer();
        }
    }
    
    public void Turn()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("当前为【转牌圈】");
                CardManager.instance.AssignCardsToTable(1);
                UIManager.instance.ShowCommunityCard(GlobalVar.publicCards[3], 3);
                UIManager.instance.PrintLog("公共卡池发出第四张牌，为【" + GlobalVar.publicCards[3].PrintCard() + "】");
                flag = false;
            }
        }
        else
        {
            UpdateCurPlayer();
        }
    }
     

    public void River()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("当前为【河牌圈】");
                CardManager.instance.AssignCardsToTable(1);
                UIManager.instance.ShowCommunityCard(GlobalVar.publicCards[4], 4);
                UIManager.instance.PrintLog("公共卡池发出最后一张牌，为【" + GlobalVar.publicCards[4].PrintCard() + "】");
                flag = false;
            }
        }
        else
        {
            UpdateCurPlayer();
        }
    }

    public void Result()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("本轮游戏结束！现在进入结算阶段");
                finalPlayers = PlayerManager.instance.GetFinalPlayers();
                CardManager.instance.FillUpTableCards();
            }
            else
            {
                winners = CardManager.instance.FindWinner(finalPlayers); ;
                if (winners.Count == 0)
                {
                    UIManager.instance.PrintLog("因玩家选牌失误，本轮没有冠军公共奖池被废弃");
                } else
                {
                    UIManager.instance.PrintLog("所有玩家最终手牌选择完毕！\n在场牌力最大玩家为：" + PrintWinner(winners));
                }
                UpdateCoins();
                ReadyForNextState();
            }
        }
        else
        {
            curPlayerSeat++;
            if (curPlayerSeat < finalPlayers.Count)
            {
                curPlayer = finalPlayers[curPlayerSeat];
            } else
            {
                playersInAction = false;
                return;
            }
            curPlayer.finalCards = curPlayer.ai.FinalSelection();
            if (IsValidSelection(curPlayer))
            {
                UIManager.instance.PrintLog("玩家【" + curPlayer.playerName + "】最后选定的五张牌为：\n【" + curPlayer.finalCards[0].PrintCard() + "】【" + curPlayer.finalCards[1].PrintCard() +
                "】【" + curPlayer.finalCards[2].PrintCard() + "】【" + curPlayer.finalCards[3].PrintCard() + "】【" + curPlayer.finalCards[4].PrintCard() + "】");
            }
            else
            {
                UIManager.instance.PrintLog("玩家【" + curPlayer.playerName + "】最后选定的牌不符合规范,无法参与冠军角逐");
                PlayerManager.instance.activePlayers.Remove(curPlayer);
                finalPlayers.Remove(curPlayer);
                curPlayerSeat--;
            }
        }
    }

    public void GameOver()
    {
        UIManager.instance.PrintLog("本轮游戏结束！现在进入结算阶段");
    }

    /// <summary>
    /// 将玩家通过coin的数值进行排序
    /// </summary>
    /// <returns>通过coin大小经过排序的玩家list</returns>
    public List<Player> GetRankedPlayers()
    {
        List<Player> pList = new List<Player>();
        foreach (Player p in PlayerManager.instance.seatedPlayers)
        {
            pList.Add(p);
        }
        pList.Sort((a, b) => {
            return b.coin - a.coin;
        });
        return pList;
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

    public void UpdateCurPlayer()
    {
        if (PlayerManager.instance.CalcFoldNum() == PlayerManager.instance.activePlayers.Count - 1)
        {
            UIManager.instance.PrintLog("场上仅剩一人未弃权，游戏直接结束。\n当前最大押注为" + GlobalVar.maxBetCoin + "，当前底池的金额为" + GlobalVar.pot);
            ReadyForNextState();
            GlobalVar.gameStatusCounter = 5;
            return;
        }

        if (curPlayerSeat == -1 && !flag)
        {
            UIManager.instance.PrintLog("新一轮下注开始");
        } else
        {
            curPlayer.playerObject.BackToWaiting_AvatarChange();
        }

        bool notFound = true;
        while (notFound)
        {

        }

        curPlayerSeat++;

        if (IsRoundCompleted())
        {
            ReadyForNextState();
            return;
        }

        curPlayer = PlayerManager.instance.activePlayers[curPlayerSeat];

        
        if (curPlayer.isFold == true || curPlayer.isAllIn == true)
        {
            UIManager.instance.PrintLog(curPlayer.playerName + "已经弃牌/ALL IN，不做操作");
            curPlayerSeat++;
        }


        if (curPlayerSeat >= PlayerManager.instance.activePlayers.Count && IsRoundCompleted())
        {
            ReadyForNextState();
            return;

        } else if (curPlayerSeat < PlayerManager.instance.activePlayers.Count)
        {
            curPlayer = PlayerManager.instance.activePlayers[curPlayerSeat];
            curPlayer.playerObject.HightLightAction_AvatarChange();
            PlayerManager.instance.Bet(curPlayer);
        }
    }

    public bool IsRoundCompleted()
    {
        passablePlayer.Clear();
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            if (p.isAllIn == true || p.betCoin == GlobalVar.maxBetCoin || p.isFold == true)
            {
                passablePlayer.Add(p);
            }
        }
        if (passablePlayer.Count == PlayerManager.instance.activePlayers.Count && 
            (flag || curPlayerSeat >= PlayerManager.instance.activePlayers.Count))
        {
            return true;
        } else if (curPlayerSeat >= PlayerManager.instance.activePlayers.Count)
        {
            flag = true;
            curPlayerSeat = 0;
        }
        return false;
    }

    public void ReadyForNextState()
    {
        playersInAction = false;
        curPlayer.playerObject.BackToWaiting_AvatarChange();
        curPlayerSeat = -1;
        if (GlobalVar.gameStatusCounter != 5)
        {
            UIManager.instance.PrintLog("本轮下注结束！\n底池：" + GlobalVar.pot + "，最大下注金额：" + GlobalVar.maxBetCoin);
            GlobalVar.gameStatusCounter++;
        } else
        {
            UIManager.instance.PrintLog("本回合游戏结束！马上进入下一轮游戏");
            PlayerManager.instance.NewRound();
            GlobalVar.curRoundNum++;
            if (GlobalVar.curRoundNum > GlobalVar.totalRoundNum || PlayerManager.instance.activePlayers.Count < 2)
            {
                if (GlobalVar.curRoundNum > GlobalVar.totalRoundNum)
                {
                    UIManager.instance.PrintLog("完成设置的全部回合数，本局游戏结束！");
                }
                else
                {
                    UIManager.instance.PrintLog("场上剩余玩家数不足开始新游戏，本局游戏提前结束！");
                }
                GlobalVar.gameStatusCounter = 6;
            } else
            {
                GlobalVar.gameStatusCounter = 0;
            }
        }
    }

    public bool IsValidSelection(Player p)
    {
        if (p.finalCards.Count != 5)
        {
            return false;
        }
        List<Card> existed = new List<Card>();
        foreach (Card c in p.finalCards)
        {
            if (!existed.Contains(c) && (GlobalVar.publicCards.Contains(c) || p.playerCardList.Contains(c)))
            {
                existed.Add(c);
            } else
            {
                return false;
            }
        }
        return true;
    }

    public string PrintWinner(List<Player> pList)
    {
        string str = "【" + pList[0].playerName;
        for (int i = 1; i < pList.Count; i++)
        {
            str = str + "】【" + pList[i].playerName;
        }
        return str + "】";
    }

    public void UpdateCoins()
    {
        int rewards = 0;
        if(winners.Count > 0)
        {
            rewards = GlobalVar.pot / winners.Count;
            UIManager.instance.PrintLog("本回合冠军有【" + winners.Count + "】人，每人得到奖金【" + rewards + "】");
        }
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            p.betCoin = 0;
            if (winners.Contains(p))
            {
                p.coin += rewards;
            }
        }
        GlobalVar.pot = 0;
        UIManager.instance.UpdateRankingList();
        UIManager.instance.PrintLog("排行榜已更新");
    }



    public void Start()
    {
        Debug.Log("游戏开始......");
        //       PlayerManager.instance.InitPlayers();
        GlobalVar.gameStatusCounter = -2;
    }

    // Update is called once per frame

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2*GlobalVar.speedFactor)
        {
            timer = 0;
            GameUpdate();
        }
    }
}