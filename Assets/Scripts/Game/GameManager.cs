using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.UI;

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

    public List<string> aiFile;
    public Dictionary<int, List<Player>> pots = new Dictionary<int, List<Player>>(); 


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
        GlobalVar.curBtnSeat = -1;
        playersInAction = false;
        foreach (Player p in winners)
        {
            p.playerObject.PlayerWinEnded();
        }

    }

    public void Init()
    {
        foreach (Player p in PlayerManager.instance.seatedPlayers)
        {
            try
            {
                p.coin = GlobalVar.initCoin;
                if (p.type == Player.aiType.WebAI)
                    p.webAI.StartGame();
                else
                    p.luaAI.StartGame();
            }
            catch (Exception e)
            {
                Debug.Log(p.playerName + "初始化失败，原因：" + e.Message);
                UIManager.instance.PrintLog(p.playerName + "AI脚本不符合规范，被移出游戏");
                PlayerManager.instance.RemovePlayer(p);
            }
        }
        PlayerManager.instance.lostPlayers = new List<Player>();
        UIManager.instance.UpdateRankingList();
        GlobalVar.curRoundNum++;
        GlobalVar.gameStatusCounter++;
    }

    public void RoundInit()
    {
        UIManager.instance.ClearAllCards();
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

        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            try
            {
                if (p.type == Player.aiType.WebAI)
                    p.webAI.RoundStart();
                else
                    p.luaAI.RoundStart();
            }
            catch (Exception e)
            {
                Debug.Log(p.playerName + "新一轮初始化失败，原因：" + e.Message);
                UIManager.instance.PrintLog(p.playerName + "AI脚本不符合规范，被移出游戏");
                PlayerManager.instance.RemovePlayer(p);
            }
        }
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
                //赢家翻回去
                foreach (Player p in PlayerManager.instance.activePlayers)
                    if (winners.Contains(p))
                        p.playerObject.PlayerWinEnded();
                GlobalVar.curBetCount = 0;
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
                GlobalVar.curBetCount = 0;
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
                GlobalVar.curBetCount = 0;
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
                GlobalVar.curBetCount = 0;
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
                HandlePots();
                int index = 0;
                foreach (var pair in pots)
                {
                    winners = CardManager.instance.FindWinner(pair.Value);
                    if (winners.Count == 0)
                    {
                        if (index == 0)
                        {
                            UIManager.instance.PrintLog("因玩家选牌失误，主奖池没有冠军被废弃");
                        } else
                        {
                            UIManager.instance.PrintLog("因玩家选牌失误，【"+index+"】号边池没有冠军被废弃");
                        }
                    }
                    else
                    {
                        if (index == 0)
                        {
                            UIManager.instance.PrintLog("主奖池所有玩家最终手牌选择完毕！\n在场牌力最大玩家为：" + PrintWinner(winners));
                        }
                        else
                        {
                            UIManager.instance.PrintLog("【" + index + "】号边池所有玩家最终手牌选择完毕！\n在场牌力最大玩家为：" + PrintWinner(winners));
                        }
                    }
                    UpdateCoins(index,pair.Key * pair.Value.Count);
                    index++;
                }
                ResetCoins();
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
            try
            {
                if (curPlayer.type == Player.aiType.WebAI)
                    curPlayer.finalCards = curPlayer.webAI.FinalSelection();
                else
                    curPlayer.finalCards = curPlayer.luaAI.FinalSelection();
                if (CardManager.instance.IsValidSelection(curPlayer))
                {
                    UIManager.instance.PrintLog("玩家【" + curPlayer.playerName + "】最后选定的五张牌为：\n【" + curPlayer.finalCards[0].PrintCard() + "】【" + curPlayer.finalCards[1].PrintCard() +
                    "】【" + curPlayer.finalCards[2].PrintCard() + "】【" + curPlayer.finalCards[3].PrintCard() + "】【" + curPlayer.finalCards[4].PrintCard() + "】");
                    UIManager.instance.ShowFinalCardSet(curPlayer);
                }
                else
                {
                    UIManager.instance.PrintLog("玩家【" + curPlayer.playerName + "】最后选定的牌不符合规范,无法参与冠军角逐");
                    curPlayer.playerObject.PlayerWinEnded();
                    PlayerManager.instance.activePlayers.Remove(curPlayer);
                    finalPlayers.Remove(curPlayer);
                    curPlayerSeat--;
                }
            }
            catch (Exception e)
            {
                Debug.Log(curPlayer.playerName + "新一轮初始化失败，原因：" + e.Message);
                UIManager.instance.PrintLog(curPlayer.playerName + "AI脚本不符合规范，被移出游戏");
                PlayerManager.instance.RemovePlayer(curPlayer);
                finalPlayers.Remove(curPlayer);
                curPlayerSeat--;
            }
        }
    }

    public void GameOver()
    {
        UIManager.instance.ClearAllCards();
        winners = PlayerManager.instance.GetFinalWinners();
        UIManager.instance.PrintLog("全部游戏结束！现在进入最终结算阶段\n最终冠军是"+PrintWinner(winners));
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            p.playerObject.PlayerWinEnded();
            if (winners.Contains(p))
            {
                p.playerObject.PlayerWin();
            }
        }
        UIManager.instance.PrintLog("可以按下【SAVE】保存本局游戏日志\n或按下【RESTART】开始新的游戏\n");
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
            if(!curPlayer.isFold)
                curPlayer.playerObject.BackToWaiting_AvatarChange();
        }

        NextValidPlayer();

        if (IsRoundCompleted())
        {
            ReadyForNextState();
            return;
        }

        curPlayer = PlayerManager.instance.activePlayers[curPlayerSeat];
        curPlayer.playerObject.HightLightAction_AvatarChange();
        PlayerManager.instance.Bet(curPlayer);
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
        if (!curPlayer.isFold)
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
                GameOver();
            } else
            {
                GlobalVar.gameStatusCounter = 0;
            }
        }
    }

    public void NextValidPlayer()
    {
        curPlayerSeat++;
        while (true)
        {
            if (curPlayerSeat >= PlayerManager.instance.activePlayers.Count)
            {
                return;
            } else if (PlayerManager.instance.activePlayers[curPlayerSeat].isFold == true || PlayerManager.instance.activePlayers[curPlayerSeat].isAllIn == true)
            {
                UIManager.instance.PrintLog(PlayerManager.instance.activePlayers[curPlayerSeat].playerName + "已经弃牌/ALL IN，不做操作");
                curPlayerSeat++;
            } else
            {
                return;
            }
        }
    }

    public void HandlePots()
    {
        pots = new Dictionary<int, List<Player>>();
        finalPlayers.Sort((a, b) => {
            return a.betCoin - b.betCoin;
        });
        int startIndex = 0;
        while (startIndex < finalPlayers.Count)
        {
            int potAmt = finalPlayers[startIndex].betCoin;
            List<Player> potPlayers = new List<Player>();
            for (int i = startIndex; i< finalPlayers.Count; i++)
            {
                Player p = finalPlayers[i];
                p.betCoin -= potAmt;
                potPlayers.Add(p);
                if (p.betCoin == 0)
                {
                    startIndex++;
                }
            }
            pots.Add(potAmt, potPlayers);
        }
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

    public void UpdateCoins(int index, int curPot)
    {
        int rewards = 0;
        if (winners.Count > 0)
        {
            rewards = curPot / winners.Count;
            if (index == 0)
            {
                UIManager.instance.PrintLog("主奖池冠军有【" + winners.Count + "】人，每人得到奖金【" + rewards + "】");
            } else
            {
                UIManager.instance.PrintLog("【"+index+"】号边池冠军有【" + winners.Count + "】人，每人得到奖金【" + rewards + "】");
            }
        }
        foreach (Player p in winners)
        {
            p.coin += rewards;
        }
    }
        
    public void ResetCoins()
    {
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            p.betCoin = 0;
            p.playerObject.UpdateCoinsCount();
            p.playerObject.PlayerWinEnded();
            p.playerObject.UpdateBetCoinsCount();
        }
        GlobalVar.pot = 0;
        UIManager.instance.UpdateCoinsPool();
        UIManager.instance.UpdateRankingList();
        UIManager.instance.PrintLog("排行榜已更新");
    }

    public void Start()
    {
        Debug.Log("游戏开始......");
        //       PlayerManager.instance.InitPlayers();
        GlobalVar.gameStatusCounter = -2;
        GlobalVar.hostName = Dns.GetHostName();
        Debug.Log("服务器主机名： " + GlobalVar.hostName);
        GlobalVar.ips = Dns.GetHostAddresses(GlobalVar.hostName);
        foreach(IPAddress ip in GlobalVar.ips)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)//ipv4
            {
                GlobalVar.ipAdress = ip.ToString();
                break;
            }
        }
        Debug.Log("服务器IPV4地址： " + GlobalVar.ipAdress);
        //自动填入本机地址
        InitialPanelManager.instance.ipAdress.text = GlobalVar.ipAdress;
    }

    // Update is called once per frame

    public void Update()
    {
        UIManager.instance.UpdateLog();
        WebServer.instance.UpdatePlayers();
        InitialPanelManager.instance.UpdatePlayerButton();
        timer += Time.deltaTime;
        if (timer > GlobalVar.speedFactor * 2)
        {
            timer = 0;
            GameUpdate();
        }
    }
}