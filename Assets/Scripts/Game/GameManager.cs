using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.UI;
using System.Threading;

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

    public static float timer = 0;  //记录游戏离上次更新的时间，用于调节游戏速度
    public int curPlayerSeat = -1;  //当前执行操作的玩家座位号
    public Player curPlayer = null; //当前执行操作的玩家
    public bool playersInAction = false;    //判断玩家是否正在执行操作
    public List<Player> winners = new List<Player>();   //当前奖池中牌力最大的玩家列表
    public List<Player> passablePlayer = new List<Player>();    //因为fold/allin被跳过操作的玩家列表
    public bool flag = false;   //用于检测当前回合玩家操作是否可以结束
    public List<Player> finalPlayers = new List<Player>();  //当前回合存活到最后算牌阶段的玩家

    public List<string> aiFile;
    public Dictionary<int, List<Player>> pots = new Dictionary<int, List<Player>>();
    private Thread logicThread;
    private bool rankUpdateFlag = false;
    private bool clearCardsFlag = false;
    private bool gameRoundFlag = false;

    //作为参考，没有引用
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

    /// <summary>
    /// 每次运行update根据game state调用对应阶段的方法
    /// </summary>
    public void GameUpdate()
    {
        while (true)
        {
            if (timer < GlobalVar.speedFactor * 2)
                continue;
            timer = 0;
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
    }

    /// <summary>
    /// 专门用于更新UI 每次运行update根据game state调用对应阶段的方法
    /// </summary>
    public void UIUpdate()
    {
        UIManager.instance.UpdateLog();
        WebServer.instance.UpdatePlayers();
        InitialPanelManager.instance.UpdatePlayerButton();
        if (rankUpdateFlag)
        {
            UIManager.instance.UpdateRankingList();
            rankUpdateFlag = false;
        }
        if (clearCardsFlag)
        {
            UIManager.instance.ClearAllCards();
            rankUpdateFlag = false;
        }
        if (gameRoundFlag)
        {
            UIManager.instance.UpdateGameRounds();
            gameRoundFlag = false;
        }
        
        if (PlayerManager.instance.UIUpdateString != "")
        {
            InitialPanelManager.instance.CallStartErrorLog(PlayerManager.instance.UIUpdateString);
            PlayerManager.instance.UIUpdateString = "";
        }
        

    }

    /// <summary>
    /// 结束setting阶段（点击start）时调用
    /// </summary>
    public void Setting()
    {
        if (PlayerManager.instance.SeatPlayers())
        {
            GlobalVar.gameStatusCounter++;
        }
    }

    /// <summary>
    /// 重新开始新一局/游戏中途重启比赛时重新初始化参数
    /// </summary>
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

    /// <summary>
    /// 新一局游戏开始时初始化参数
    /// </summary>
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
                UIManager.instance.PrintThread(p.playerName + "AI脚本不符合规范，被移出游戏");
                PlayerManager.instance.RemovePlayer(p);
            }
        }
        PlayerManager.instance.lostPlayers = new List<Player>();
        rankUpdateFlag = true;
        GlobalVar.curRoundNum++;
        GlobalVar.gameStatusCounter++;
    }

    /// <summary>
    /// 每一轮回合结束后更新参数
    /// </summary>
    public void RoundInit()
    {
        clearCardsFlag = true;
        UIManager.instance.PrintThread("新一轮游戏开始！当前为第【" + GlobalVar.curRoundNum + "】轮");
        gameRoundFlag = true;       
        PlayerManager.instance.NewRound();
        GlobalVar.curBtnSeat = (GlobalVar.curBtnSeat + 1) % PlayerManager.instance.activePlayers.Count;
        PlayerManager.instance.SetPlayersRole(GlobalVar.curBtnSeat);
        UIManager.instance.PrintThread("位置分配完毕！");
        if (PlayerManager.instance.activePlayers.Count >= 3)
        {
            UIManager.instance.PrintThread("【" + PlayerManager.instance.activePlayers[PlayerManager.instance.activePlayers.Count - 1].playerName + "】为庄家位");
            UIManager.instance.PrintThread("【" + PlayerManager.instance.activePlayers[0].playerName + "】为小盲位");
        }
        else
        {
            UIManager.instance.PrintThread("【" + PlayerManager.instance.activePlayers[0].playerName + "】为庄家和小盲位");
        }
        UIManager.instance.PrintThread("【" + PlayerManager.instance.activePlayers[1].playerName + "】为大盲位");

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
                UIManager.instance.PrintThread(p.playerName + "AI脚本不符合规范，被移出游戏");
                PlayerManager.instance.RemovePlayer(p);
            }
        }
        curPlayerSeat = -1;
        CardManager.instance.InitialCardsList();
        GlobalVar.gameStatusCounter++;
    }

    /// <summary>
    /// 前翻牌轮调用的方法
    /// </summary>
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
                UIManager.instance.PrintThread("当前为【前翻牌圈】");
                CardManager.instance.AssignCardsToPlayers();
                UIManager.instance.PrintThread("每个在游戏中的玩家获得两张手牌");
                foreach(Player p in PlayerManager.instance.activePlayers)
                {
                    UIManager.instance.PrintThread("【" + p.playerName + "】的手牌为：【" + p.playerCardList[0].PrintCard() + "】【" + p.playerCardList[1].PrintCard() + "】");
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

    /// <summary>
    /// 翻牌轮调用的方法
    /// </summary>
    public void Flop()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                GlobalVar.curBetCount = 0;
                playersInAction = true;
                UIManager.instance.PrintThread("当前为【翻牌圈】");
                CardManager.instance.AssignCardsToTable(3);
                for (int i = 0; i < 3; i++)
                {
                    UIManager.instance.ShowCommunityCard(GlobalVar.publicCards[i], i);
                }
                UIManager.instance.PrintThread("公共卡池发出前三张牌，分别为：\n【" + GlobalVar.publicCards[0].PrintCard() + "】【" +
                    GlobalVar.publicCards[1].PrintCard() + "】【" + GlobalVar.publicCards[2].PrintCard() + "】");
                flag = false;
            }
        }
        else
        {
            UpdateCurPlayer();
        }
    }
    
    /// <summary>
    /// 转牌轮调用的方法
    /// </summary>
    public void Turn()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                GlobalVar.curBetCount = 0;
                playersInAction = true;
                UIManager.instance.PrintThread("当前为【转牌圈】");
                CardManager.instance.AssignCardsToTable(1);
                UIManager.instance.ShowCommunityCard(GlobalVar.publicCards[3], 3);
                UIManager.instance.PrintThread("公共卡池发出第四张牌，为【" + GlobalVar.publicCards[3].PrintCard() + "】");
                flag = false;
            }
        }
        else
        {
            UpdateCurPlayer();
        }
    }
     
    /// <summary>
    /// 河牌轮调用的方法
    /// </summary>
    public void River()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                GlobalVar.curBetCount = 0;
                playersInAction = true;
                UIManager.instance.PrintThread("当前为【河牌圈】");
                CardManager.instance.AssignCardsToTable(1);
                UIManager.instance.ShowCommunityCard(GlobalVar.publicCards[4], 4);
                UIManager.instance.PrintThread("公共卡池发出最后一张牌，为【" + GlobalVar.publicCards[4].PrintCard() + "】");
                flag = false;
            }
        }
        else
        {
            UpdateCurPlayer();
        }
    }

    /// <summary>
    /// 当前回合结束后调用的方法
    /// </summary>
    public void Result()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == -1)
            {
                playersInAction = true;
                UIManager.instance.PrintThread("本轮游戏结束！现在进入结算阶段");
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
                            UIManager.instance.PrintThread("因玩家选牌失误，主奖池没有冠军被废弃");
                        } else
                        {
                            UIManager.instance.PrintThread("因玩家选牌失误，【"+index+"】号边池没有冠军被废弃");
                        }
                    }
                    else
                    {
                        if (index == 0)
                        {
                            UIManager.instance.PrintThread("主奖池所有玩家最终手牌选择完毕！\n在场牌力最大玩家为：" + PrintWinner(winners));
                        }
                        else
                        {
                            UIManager.instance.PrintThread("【" + index + "】号边池所有玩家最终手牌选择完毕！\n在场牌力最大玩家为：" + PrintWinner(winners));
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
                    UIManager.instance.PrintThread("玩家【" + curPlayer.playerName + "】最后选定的五张牌为：\n【" + curPlayer.finalCards[0].PrintCard() + "】【" + curPlayer.finalCards[1].PrintCard() +
                    "】【" + curPlayer.finalCards[2].PrintCard() + "】【" + curPlayer.finalCards[3].PrintCard() + "】【" + curPlayer.finalCards[4].PrintCard() + "】");
                    UIManager.instance.ShowFinalCardSet(curPlayer);
                }
                else
                {
                    UIManager.instance.PrintThread("玩家【" + curPlayer.playerName + "】最后选定的牌不符合规范,无法参与冠军角逐");
                    curPlayer.playerObject.PlayerWinEnded();
                    PlayerManager.instance.activePlayers.Remove(curPlayer);
                    finalPlayers.Remove(curPlayer);
                    curPlayerSeat--;
                }
            }
            catch (Exception e)
            {
                Debug.Log(curPlayer.playerName + "新一轮初始化失败，原因：" + e.Message);
                UIManager.instance.PrintThread(curPlayer.playerName + "AI脚本不符合规范，被移出游戏");
                PlayerManager.instance.RemovePlayer(curPlayer);
                finalPlayers.Remove(curPlayer);
                curPlayerSeat--;
            }
        }
    }

    /// <summary>
    /// 当局游戏回合全部结束后调用的方法
    /// </summary>
    public void GameOver()
    {
        UIManager.instance.ClearAllCards();
        winners = PlayerManager.instance.GetFinalWinners();
        UIManager.instance.PrintThread("全部游戏结束！现在进入最终结算阶段\n最终冠军是"+PrintWinner(winners));
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            p.playerObject.PlayerWinEnded();
            if (winners.Contains(p))
            {
                p.playerObject.PlayerWin();
            }
        }
        UIManager.instance.PrintThread("可以按下【SAVE】保存本局游戏日志\n或按下【RESTART】开始新的游戏\n");
    }


    /// <summary>
    /// 更新当前执行操作的玩家
    /// </summary>
    public void UpdateCurPlayer()
    {
        if (PlayerManager.instance.CalcFoldNum() == PlayerManager.instance.activePlayers.Count - 1)
        {
            UIManager.instance.PrintThread("场上仅剩一人未弃权，游戏直接结束。\n当前最大押注为" + GlobalVar.maxBetCoin + "，当前底池的金额为" + GlobalVar.pot);
            ReadyForNextState();
            GlobalVar.gameStatusCounter = 5;
            return;
        }

        if (curPlayerSeat == -1 && !flag)
        {
            UIManager.instance.PrintThread("新一轮下注开始");
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

    /// <summary>
    /// 判断当前阶段所有玩家是否都完成了全部操作
    /// </summary>
    /// <returns>如果全部玩家都操作完毕则return true</returns>
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

    /// <summary>
    /// 判断游戏是否可以进入下一阶段，如果是则更新参数
    /// </summary>
    /// <returns>如果可以进入下一阶段则return true</returns>
    public void ReadyForNextState()
    {
        playersInAction = false;
        if (!curPlayer.isFold)
            curPlayer.playerObject.BackToWaiting_AvatarChange();
        curPlayerSeat = -1;
        if (GlobalVar.gameStatusCounter != 5)
        {
            UIManager.instance.PrintThread("本轮下注结束！\n底池：" + GlobalVar.pot + "，最大下注金额：" + GlobalVar.maxBetCoin);
            GlobalVar.gameStatusCounter++;
        } else
        {
            UIManager.instance.PrintThread("本回合游戏结束！马上进入下一轮游戏");
            PlayerManager.instance.NewRound();
            GlobalVar.curRoundNum++;
            if (GlobalVar.curRoundNum > GlobalVar.totalRoundNum || PlayerManager.instance.activePlayers.Count < 2)
            {
                if (GlobalVar.curRoundNum > GlobalVar.totalRoundNum)
                {
                    UIManager.instance.PrintThread("完成设置的全部回合数，本局游戏结束！");
                }
                else
                {
                    UIManager.instance.PrintThread("场上剩余玩家数不足开始新游戏，本局游戏提前结束！");
                }
                GlobalVar.gameStatusCounter = 6;
                GameOver();
            } else
            {
                GlobalVar.gameStatusCounter = 0;
            }
        }
    }

    /// <summary>
    /// 在当前玩家操作阶段跳到下一个可执行操作的玩家
    /// </summary>
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
                UIManager.instance.PrintThread(PlayerManager.instance.activePlayers[curPlayerSeat].playerName + "已经弃牌/ALL IN，不做操作");
                curPlayerSeat++;
            } else
            {
                return;
            }
        }
    }

    /// <summary>
    /// 在回合结算阶段处理奖池，如有玩家ALLIN则开开边池
    /// </summary>
    public void HandlePots()
    {
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

    /// <summary>
    /// 在LOG里打印当前全部WINNER
    /// </summary>
    /// <param name="pList">需要打印出来的玩家名列表</param>
    /// <returns>列表中所有玩家的姓名（带括号））</returns>
    public string PrintWinner(List<Player> pList)
    {
        string str = "【" + pList[0].playerName;
        for (int i = 1; i < pList.Count; i++)
        {
            str = str + "】【" + pList[i].playerName;
        }
        return str + "】";
    }

    /// <summary>
    /// 更新当前奖池中玩家金币数
    /// </summary>
    /// <param name="index">当前奖池编号</param>
    /// <param name="curPot">当前奖池中总奖金数</param>
    public void UpdateCoins(int index, int curPot)
    {
        int rewards = 0;
        if (winners.Count > 0)
        {
            rewards = curPot / winners.Count;
            if (index == 0)
            {
                UIManager.instance.PrintThread("主奖池冠军有【" + winners.Count + "】人，每人得到奖金【" + rewards + "】");
            } else
            {
                UIManager.instance.PrintThread("【"+index+"】号边池冠军有【" + winners.Count + "】人，每人得到奖金【" + rewards + "】");
            }
        }
        foreach (Player p in winners)
        {
            p.coin += rewards;
        }
    }
    
    /// <summary>
    /// 更新完当前回合全部奖池后重置玩家、公共奖池参数
    /// </summary>
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
        UIManager.instance.PrintThread("排行榜已更新");
    }

    /// <summary>
    /// 开始游戏自动调用
    /// </summary>
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
        //开启多线程负责游戏逻辑 与 UI更新分离
        logicThread = new Thread(GameUpdate);
        logicThread.IsBackground = true;
        logicThread.Start();
    }

    // Update is called once per frame
    /// <summary>
    /// 游戏更新方法，自动调用
    /// </summary>

    public void Update()
    {
        timer += Time.deltaTime;
        
        UIUpdate();
    }
}