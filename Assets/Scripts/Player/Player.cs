using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player 
{
    public enum PlayerRole
    {
        button,         //庄家
        bigBlind,       //大盲
        smallBlind,     //小盲
        normal,         //普通玩家
        outOfGame       //不在此轮游戏中
    }

    public WebAI ai;
    public string playerName;
    public int seatNum = -1; //如果不在此轮游戏中，则为-1
    public int coin = 0;
    public int betCoin = 0;
    public bool isInGame = false;
    public bool isFold = false;
    public bool isAllIn = false;
    public PlayerRole role = PlayerRole.outOfGame;
    public List<Card> playerCardList = new List<Card>();//两张手牌
    public int state = 0; //no action
    public List<Card> finalCards = new List<Card>();//最终组成的五张牌组

    public List<int[]> last_period = new List<int[]>();//上一轮中玩家自己位置之后的玩家的行动记录（记录为【玩家序号(seatNum)，操作序号】，注册进事件来维护这两个列表）       在自己的回合结束后清空该列表以接收接下来的信息
    public List<int[]> this_period = new List<int[]>();//这一轮中从庄家到玩家自己位置的行动记录（同上）      在本轮结束后（这一轮即将开始时）清空该列表以接收接下来的信息

    public PlayerObject playerObject;


    public Player(WebAI ai)
    {
        this.ai = ai;
        this.playerName = ai.name;
        //在进行初始化的时候就要注册进事件中去！接下来只要调用RecordManager中的ActionRecord就可以让所有玩家接收到信息。
        RecordManager.instance.ActionRecords += AddActionRecord;
    }

    public Player(LuaAI ai)
    {
        this.ai = ai;
        this.playerName = ai.name;
        //在进行初始化的时候就要注册进事件中去！接下来只要调用RecordManager中的ActionRecord就可以让所有玩家接收到信息。
        RecordManager.instance.ActionRecords += AddActionRecord;
    }
    /// <summary>
    /// 给玩家的个人卡池添加两张卡牌
    /// </summary>
    /// <param name="a">第一张牌</param>
    /// <param name="b">第二张牌</param>
    public void AddPlayerCards(Card a, Card b)
    {
        playerCardList = new List<Card>();
        playerCardList.Add(a);
        playerCardList.Add(b);
    }

    /// <summary>
    /// 将玩家移出游戏
    /// </summary>
    public void OutOfGame()
    {
        isInGame = false;
        coin = 0;
        role = PlayerRole.outOfGame;
        ResetNewRound();
    }

    /// <summary>
    /// 新一轮游戏重置玩家
    /// </summary>
    public void ResetNewRound()
    {
        isFold = false;
        isAllIn = false;
        state = 0;
        betCoin = 0;
        playerCardList = new List<Card>();
        finalCards = new List<Card>();
    }

    /// <summary>
    /// 有玩家行动了，所有玩家开始尝试记录或者对记录表进行操作(必须放在玩家行动结束之后！！再调用)
    /// </summary>
    /// <param name="playerSeatNum">座位号</param>
    /// <param name="actionNum">行动</param>
    public void AddActionRecord(int playerSeatNum, int actionNum)
    {
        //获取在行动顺序表中的排序以正确记录信息
        int thisPlayerIndex = -1;
        int actionPlayerIndex = -1;
        for (int i = 0; i < PlayerManager.instance.activePlayers.Count; i++)
        {
            if (PlayerManager.instance.activePlayers[i].seatNum == playerSeatNum)
                actionPlayerIndex = i;
            if (PlayerManager.instance.activePlayers[i].seatNum == seatNum)
                thisPlayerIndex = i;
            if (actionPlayerIndex >= 0 && thisPlayerIndex >= 0)
                break;
        }
        if (actionPlayerIndex < 0 || actionPlayerIndex >= 8 || thisPlayerIndex < 0)
        {
            Debug.Log("SeatNum Data Error!!   a:"+actionPlayerIndex+"   t:"+thisPlayerIndex);
            return;
        }

        //根据在行动顺序表中的排序进行合适的信息记录
        if (thisPlayerIndex == actionPlayerIndex)//如果是自己的行动，不需要记录，并且清空last_period为接下来接收信息做准备
        {
            last_period.Clear();
        }
        else if (actionPlayerIndex < thisPlayerIndex)//是自己之前的玩家进行行动，加入 this_period
        {
            //Debug.Log(this.playerName+"  PlayerNum: " + playerSeatNum + "   ActionNum: " + actionNum+"    this");
            this.this_period.Add(new int[2] { playerSeatNum, actionNum });
        }
        else//是自己之后的玩家进行行动，加入 last_period
        {
            //Debug.Log(this.playerName+"  PlayerNum: " + playerSeatNum + "   ActionNum: " + actionNum + "    last");
            this.last_period.Add(new int[2] { playerSeatNum, actionNum });
        }

        //这是最后一位玩家在行动，要清空 this_period 
        if (actionPlayerIndex == PlayerManager.instance.activePlayers.Count - 1)
        {
            this_period.Clear();
        }
    }
}
