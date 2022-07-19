using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
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
    public int curPlayerSeat = 0;
    public Player curPlayer = null;
    public bool playersInAction = false;
    public List<Player> winners = new List<Player>();
    
    public static GameState GameStatus()
    {
        switch (GolbalVar.gameStatusCounter)
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
        switch(GolbalVar.gameStatusCounter)
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
            GolbalVar.gameStatusCounter++;
        }
    }

    public void Restart()
    {
        GolbalVar.gameStatusCounter = -2;
        GolbalVar.curRoundNum = 0;
    }

    public void Init()
    {
        foreach (Player p in PlayerManager.instance.seatedPlayers)
        {
            p.coin = GolbalVar.initCoin;
        }
        UIManager.instance.UpdateRankingList();
        GolbalVar.gameStatusCounter++;
    }

    public void RoundInit()
    {
        GolbalVar.curRoundNum++;
        UIManager.instance.PrintLog("新一轮游戏开始！当前为第【" + GolbalVar.curRoundNum +"】轮");
        PlayerManager.instance.NewRound();
        //PlayerManager.instance.SetPlayersRole(GolbalVar.curBtnSeat); NewRound() has done this;
        UIManager.instance.PrintLog("位置分配完毕！");
        if (PlayerManager.instance.activePlayers.Count >= 3)
        {
            UIManager.instance.PrintLog("【" + PlayerManager.instance.activePlayers[PlayerManager.instance.activePlayers.Count - 1].playerName + "】为庄家位");
            UIManager.instance.PrintLog("【" + PlayerManager.instance.activePlayers[0].playerName + "】为小盲位");
        } else
        {
            UIManager.instance.PrintLog("【" + PlayerManager.instance.activePlayers[0].playerName + "】为庄家和小盲位");
        }
        UIManager.instance.PrintLog("【" + PlayerManager.instance.activePlayers[1].playerName + "】为大盲位");

        curPlayerSeat = 0;
        curPlayer = PlayerManager.instance.activePlayers[curPlayerSeat];
        CardManager.instance.InitialCardsList();
        GolbalVar.gameStatusCounter++;
    }

    public void Preflop()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("当前为【前翻牌圈】");
                CardManager.instance.AssignCardsToPlayers();
                UIManager.instance.PrintLog("每个在游戏中的玩家获得两张手牌");
                int sign = PlayerManager.instance.PlayerBet();
                if (sign == 0) GolbalVar.gameStatusCounter = 5;
            } else
            {
                ReadyForNextState();
            }
            
        } else
        {
            UpdateCurPlayer();
            UIManager.instance.PrintLog("【" + curPlayer.playerName + "】的手牌为：【" + curPlayer.playerCardList[0].PrintCard() + "】【" + curPlayer.playerCardList[1].PrintCard() + "】");
        }
    }

    public void Flop()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("当前为【翻牌圈】");
                CardManager.instance.AssignCardsToTable(3);
                for (int i = 0; i < 3; i++)
                {
                    UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[i], i);
                }
                UIManager.instance.PrintLog("公共卡池发出前三张牌，分别为：\n【" + GolbalVar.publicCards[0].PrintCard() + "】【" +
                    GolbalVar.publicCards[1].PrintCard() + "】【" + GolbalVar.publicCards[2].PrintCard() + "】");
                int sign = PlayerManager.instance.PlayerBet();
                if (sign == 0) GolbalVar.gameStatusCounter = 5;
            } else
            {
                ReadyForNextState();
            }
        } else
        {
            UpdateCurPlayer();
        }
    }
    public void Turn()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("当前为【转牌圈】");
                CardManager.instance.AssignCardsToTable(1);
                UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[3], 3);
                UIManager.instance.PrintLog("公共卡池发出第四张牌，为【" + GolbalVar.publicCards[3].PrintCard() + "】");
                int sign = PlayerManager.instance.PlayerBet();
                if (sign == 0) GolbalVar.gameStatusCounter = 5;
            }
            else
            {
                ReadyForNextState();
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
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("当前为【河牌圈】");
                CardManager.instance.AssignCardsToTable(1);
                UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[4], 4);
                UIManager.instance.PrintLog("公共卡池发出最后一张牌，为【" + GolbalVar.publicCards[4].PrintCard() + "】"); ;
                int sign = PlayerManager.instance.PlayerBet();
                if (sign == 0) GolbalVar.gameStatusCounter = 5;
            }
            else
            {
                ReadyForNextState();
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
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("本轮游戏结束！现在进入结算阶段");
            }
            else
            {
                ReadyForNextState();
                winners = CardManager.instance.FindWinner(PlayerManager.instance.activePlayers);
                UIManager.instance.PrintLog("所有玩家最终手牌选择完毕！\n在场牌力最大玩家为："+PrintWinner(winners));
            }
        }
        else
        {
            UpdateCurPlayer();
            curPlayer.finalCards = curPlayer.ai.FinalSelection();
            if (IsValidSelection(curPlayer))
            {
                UIManager.instance.PrintLog("玩家【" + curPlayer.playerName + "】最后选定的五张牌为：\n【" + curPlayer.finalCards[0].PrintCard() + "】【" + curPlayer.finalCards[1].PrintCard() +
                "】【" + curPlayer.finalCards[2].PrintCard() + "】【" + curPlayer.finalCards[3].PrintCard() + "】【" + curPlayer.finalCards[4].PrintCard() + "】");
            } else
            {
                UIManager.instance.PrintLog("玩家【" + curPlayer.playerName + "】最后选定的牌不符合规范,无法参与冠军角逐");
                PlayerManager.instance.activePlayers.Remove(curPlayer);
            }
        }
    }

    public void GameOver()
    {

    }

    /// <summary>
    /// 将玩家通过coin的数值进行排序
    /// </summary>
    /// <returns>通过coin大小经过排序的玩家list</returns>
    public List<Player> GetRankedPlayers()
    {
        List<Player> pList = new List<Player>();
        foreach(Player p in PlayerManager.instance.seatedPlayers)
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
    public List<int> GetPlayerRank (List<Player> pList)
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
        curPlayer.playerObject.BackToWaiting_AvatarChange();
        curPlayer = PlayerManager.instance.activePlayers[curPlayerSeat];
        curPlayerSeat++;
        curPlayer.playerObject.HightLightAction_AvatarChange();
        if (curPlayerSeat == PlayerManager.instance.activePlayers.Count)
        {
            playersInAction = false;
        }
    }

    public void ReadyForNextState()
    {
        curPlayer.playerObject.BackToWaiting_AvatarChange();
        if(GolbalVar.gameStatusCounter != 5)
        {
            curPlayerSeat = 0;
            GolbalVar.gameStatusCounter++;
        }
    }

    public bool IsValidSelection(Player p)
    {
        if (p.finalCards.Count != 5)
        {
            return false;
        }
        List<Card> existed = new List<Card>();
        foreach(Card c in p.finalCards)
        {
            if (!existed.Contains(c) && (GolbalVar.publicCards.Contains(c) || p.playerCardList.Contains(c)))
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
        for (int i = 1; i<pList.Count; i++)
        {
            str = str + "】【" + pList[i].playerName;
        }
        return str + "】";
    }



    public void Start()
    {
        Debug.Log("游戏开始......");
 //       PlayerManager.instance.InitPlayers();
        GolbalVar.gameStatusCounter = -2;
    }

    // Update is called once per frame
    public void Update()
    {   
        if (GolbalVar.curRoundNum > GolbalVar.totalRoundNum)
        {
            GameOver();
        }
        timer += Time.deltaTime;
        if (timer > 2 * GolbalVar.speedFactor)
        {
            GameUpdate();
            timer = 0;
        }
       
    }
}
