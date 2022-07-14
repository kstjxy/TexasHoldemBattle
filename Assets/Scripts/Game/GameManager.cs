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
        loading,    //游戏未开始
        setting,    //设置界面，调整游戏规则
        init,       //初始化，切换界面，玩家入座
        roundinit,  //每一局开始的INIT，确定PLAYER ROLES
        preflop,    //发给玩家两张牌 第一轮下注
        flop,       //公开卡池三张牌 第二轮下注
        turn,       //公开卡池第四张牌 第三轮下注
        river,      //公开卡池第四张牌 第四轮下注
        result,     //一局游戏结束显示结果，调整筹码和排行
        gameover    //设定的所有局结束
    }
    
    public static GameState GameStatus()
    {
        switch (GolbalVar.gameStatusCounter)
        {
            case -3:
                return GameState.loading;
            case -2:
                return GameState.setting;
            case -1:
                return GameState.init;
            case 0:
                return GameState.roundinit;
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

    //全局变量
    public static List<Player> playerList = new List<Player>(); //玩家列表 PlayerObject
    public static List<Card> cardList = new List<Card>();       //公共牌
    public static List<int> allNum = new List<int>();           //所有玩家的序号
    public static List<int> playingNum = new List<int>();       //还在玩的玩家的序号
    public static List<int> lostNum = new List<int>();          //已淘汰玩家的序号
    public static int buton;                                    //庄家序号
    public static int totalRound;                               //总局数
    public static int initMoney;                                //
    public static int betMonet;                                 //

    public void Loading()//初始化
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

    }
}
