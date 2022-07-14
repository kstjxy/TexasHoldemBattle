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
        int gamestate = GlobalVar.gameStatusCounter;
        if (gamestate < 7)
        {

        }
    }

    //全局变量
    public static List<string> namelist = new List<string>();   //玩家姓名
    public static List<Player> playerList = new List<Player>(); //玩家列表 PlayerObject
    public static List<Card> cardList = new List<Card>();       //公共牌
    public static List<int> allNum = new List<int>();           //所有玩家的序号
    public static List<int> playingNum = new List<int>();       //还在玩的玩家的序号
    public static List<int> lostNum = new List<int>();          //已淘汰玩家的序号
    public static int buton;                                    //庄家序号
    public static int setRound;                                 //设定总局数
    public static int initMoney;                                //
    public static int betMoney;                                 //
    public static int curRound;                                 //当前局数
    public float time = 0;                                      //用于调节速度，计时
    public double gapset = 5;                                   //时间间隔
    public double ratioTime = 0;                                //拖动条参数
    public float gaptime = 5;                                   //真正的时间间隔 
    public bool started = 0;                                    //游戏开始标志                
    public static string debugLogStr;
    public static string DebugLogStr
    {
        get => debugLogStr;
        set => debugLogStr = value + '\n';
    }



    public void Init()//初始化
    {
        Debug.log("\t\t初始化ing\t\t");
        //从UI？获取玩家姓名列表（未完成）
        namelist.Add("001");
        namelist.Add("002");
        namelist.Add("003");
        //确定玩家列表
        PlayerManager.instance.IniPlayers(namelist) = initMoney;
        //参加的玩家，编号从1开始
        int tmpNo = 1;
        foreach (Player p in PlayerManager.instance.allPlayers)
        {
            //可以加一个玩不玩的判断（）
            PlayerManager.instance.SeatedPlayer(p, tmpNo++);
        }
        //赌注大小
        betMoney = 5;
        //初始钱数
        initMoney = 2000;
        //总局数
        setRound = 20;
        //加注次数?
        //初始化gamestate
        currentState = GameState.init;
        curRound = 0;
        

    }
    public void SetGap(double ratio)
    {
        gaptime = (1.01 - ratio) * gapset;
    }
    public void RoundInit()//初始化
    {
        Debug.log("\t\t\t\t")
        //确定玩家列表

        //赌注大小
        //初始钱数
        //总局数
        //加注次数
    }
    public void RoundInit()//初始化
    {
        //确定玩家列表
        //赌注大小
        //初始钱数
        //总局数
        //加注次数
    }
    public void RoundInit()//初始化
    {
        //确定玩家列表
        //赌注大小
        //初始钱数
        //总局数
        //加注次数
    }
    // Start is called before the first frame update
    void Begin()
    {	
        
    }

    // Update is called once per frame
    void Update()
    {   
        if （started){
            if (time >= gaptime)
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
                            Gameover();
                            break;
                        }

                        RoundInit();
                        break;
                }
                // throw new UnityException("GameState Error" + currentState)
                time = 0;
            }
            else time += Time.deltaTime;
        }
       
    }
}
