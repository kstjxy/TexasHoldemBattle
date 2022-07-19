using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_undone: MonoBehaviour
{
    // 单例
    public static GameManager_undone instance;
    void Awake()
    {
        instance = this;
    }

    //游戏进程
    public enum GameState
    {
        init,       //整个游戏初始化
        roundinit,  //每局游戏初始化，确定庄家 大盲小盲
        preflop,
        flop,
        turn,
        river,
        calc,       //一局游戏结束
        gameover    //设定的所有局结束
    }
    public GameState currentState = GameState.init; //0
    //进程转换  ask
    public static int GameStatus()
    {
        int gamestate = GolbalVar.gameStatusCounter;
        if (gamestate < 7)
        {

        }
        return 1;
    }

    //全局变量
    public static List<string> namelist = new List<string>();       //玩家姓名
    public static List<Player> allPlayers = new List<Player>();     //全部玩家 PlayerObject pm中
    public static List<Player> seatedPlayers = new List<Player>();  //未淘汰玩家 PlayerObject
    public static List<Player> playing = new List<Player>();        //参与了此局的玩家
    public static List<Player> activePlayers = new List<Player>();  //仍在玩的玩家
    public static List<Player> winPlayers = new List<Player>();     //赢得此局玩家
    public static List<Player> lostPlayers = new List<Player>();    //淘汰玩家
    public static List<Card> cardList = new List<Card>();           //公共牌
    public static int buton;                                        //庄家序号
    public static int setRound;                                     //设定总局数
    public static int initMoney;                                    //
    public static int betMoney;                                     //单注
    public static int betTotal;                                     //当局总下注
    public static int betcount;                                     //每个阶段最多下注数
    public static int curRound;                                     //当前局数
    public int buttonNum;                                           //庄家
    public float time = 0;                                          //用于调节速度，计时
    public float gapset = 5;                                        //时间间隔
    public float ratioTime = 0;                                     //拖动条参数
    public float gaptime = 5;                                       //真正的时间间隔 
    public bool started = false;                                        //游戏开始标志                
    public static string debugLogStr;
    public static string DebugLogStr
    {
        get => debugLogStr;
        set => debugLogStr = value + '\n';
    }



    public void Init()//初始化
    {
        Debug.Log("\t\t初始化ing\t\t");
        //赌注大小
        betMoney = 10;
        //初始钱数
        initMoney = 2000;
        //总局数
        setRound = 20;
        //加注次数?
        //初始化gamestate
        currentState = GameState.init;
        curRound = 0;
        buttonNum = 1;
        betcount = 3;
        //从UI？获取玩家姓名列表（未完成）
        namelist.Add("001");
        namelist.Add("002");
        namelist.Add("003");
        namelist.Add("004");
        namelist.Add("005");
        namelist.Add("006");
        //确定玩家列表
        PlayerManager.instance.InitPlayers(namelist);
        //参加的玩家，编号从1开始
        int tmpNo = 1;
        foreach (Player p in PlayerManager.instance.allPlayers)
        {
            //可以加一个玩不玩的判断（）
            p.isInGame = true;
            p.coin = initMoney;
            p.seatNum = tmpNo++;
        }


    }
    public void SetGap(float ratio)
    {
        gaptime = (float)((1.01 - ratio) * gapset);
    }
    public void RoundInit()//初始化
    {

        Debug.Log("\t\t\t\t");
        currentState = GameState.roundinit;
        cardList.Clear();
        started = true;

        //确定还在场上的玩家
        PlayerManager.instance.SeatPlayers();
        //刷新fold状态,清空下注额
        PlayerManager.instance.NewRound();
        /*foreach (Player p in PlayerManager.instance.seatedPlayers)
        {
            p.isFold = false;
            p.betMoney = 0;
        }
        PlayerManager.instance.ActivePlayer();//undone
        */
        //确定庄家
        // Newround 中已完成这部分工作

    }
    public void Preflop()//初始化
    {
        int betRound = 0;
        //int betp = 0;
        currentState = GameState.preflop;
        //发牌
        CardManager.instance.AssignCardsToPlayers();

        //更改player的betmoney
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            if (p.role == Player.PlayerRole.bigBlind)
                p.betCoin = 2 * betMoney;
            else if (p.role == Player.PlayerRole.smallBlind)
                p.betCoin= betMoney;
        }
        //更改赌金池
        betTotal = 3 * betMoney;
        betRound = 2 * betMoney;
        //AI操作,此处做简单模拟
        bool first = false;
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            if (first)
            {
                int m = Random.Range(0, 3);
                p.state = m;
                PlayerManager.instance.BetAction(p);
            }

            if (p.role == Player.PlayerRole.bigBlind)
                first = true;
        }
        //
        PlayerManager.instance.ActivePlayers();//undone
    }
    public void Flop()//初始化
    {
        currentState = GameState.flop;
        CardManager.instance.AssignCardsToTable(1);
        //AI操作
        //

        PlayerManager.instance.ActivePlayers();
    }
    public void Turn()//初始化
    {
        currentState = GameState.turn;
        CardManager.instance.AssignCardsToTable(1);
        //AI操作
        //

        PlayerManager.instance.ActivePlayers();
    }
    public void River()//初始化
    {
        currentState = GameState.river;
        CardManager.instance.AssignCardsToTable(1);
        //AI操作
        //

        PlayerManager.instance.ActivePlayers();
    }
    public void Calc()//初始化
    {
        currentState = GameState.calc;
        //AI操作
        //
        winPlayers = CardManager.instance.FindWinner(PlayerManager.instance.activePlayers);
        foreach (Player p in winPlayers)
        {
            p.coin += betTotal / winPlayers.Count;
        }
        foreach (Player p in seatedPlayers)
        {
            p.coin -= p.betCoin;
        }
        
    }
    public void GameOver()//
    {
        currentState = GameState.gameover;
        started = false;
        
    }
    // Start is called before the first frame update
    void Begin()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (started){
            if (time < gaptime)
                time += Time.deltaTime;
            else
            {
                switch (currentState)
                {
                    case GameState.init:
                        RoundInit();
                        break;
                    case GameState.roundinit:
                        Preflop();
                        break;
                    case GameState.preflop:
                        Flop();
                        break;
                    case GameState.flop:
                        Turn();
                        break;
                    case GameState.turn:
                        River();
                        break;
                    case GameState.river:
                        Calc();
                        break;
                    case GameState.calc:
                        curRound++;
                        if (curRound == setRound)
                        {
                            GameOver();
                            break;
                        }
                        RoundInit();
                        break;
                }
                // throw new UnityException("GameState Error" + currentState)
                time = 0;
            }
        }
    }
}
