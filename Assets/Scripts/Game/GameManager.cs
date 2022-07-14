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
        roundinit,  //每一局开始的INIT，确定PLAYER ROLES，发牌
        preflop,
        flop,
        turn,
        river,
        result,      //一局游戏结束显示结果，调整筹码和排行
        gameover    //设定的所有局结束
    }
    public GameState currentState = GameState.loading; //0
                                                    //进程转换  ask
    /*
    public static int GameStatus()
    {
        
        int gamestate = GlobalVar.gameStatusCounter;
        if (gamestate < 7)
        {

        }
        
}
    */

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
        currentState = GameState.init;
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

        switch (currentState)
        {
            case GameState.init:
                RoundInit();
                break;

        }
    }
}
