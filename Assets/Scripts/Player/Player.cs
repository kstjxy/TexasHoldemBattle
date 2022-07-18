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

    public string playerName;
    public int seatNum = -1; //如果不在此轮游戏中，则为-1
    public int coin = 0;
    public int betCoin = 0;
    public bool isInGame = false;
    public bool isFold = false;
    public bool isAllIn = false;
    public PlayerRole role = PlayerRole.outOfGame;
    public List<Card> playerCardList = new List<Card>();//两张手牌
    public int state = -1; //no action
    public List<Card> finalCards = new List<Card>();//最终组成的五张牌组

    public List<int[,]> last_period = new List<int[,]>();//上一轮中玩家自己位置之后的玩家的行动记录（记录为【玩家序号(seatNum)，操作序号】，注册进事件来维护这两个列表）       在自己的回合结束后清空该列表以接收接下来的信息
    public List<int[,]> this_period = new List<int[,]>();//这一轮中从庄家到玩家自己位置的行动记录（同上）      在本轮结束后（这一轮即将开始时）清空该列表以接收接下来的信息

    public PlayerObject playerObject;


    public Player(string playerName)
    {
        this.playerName = playerName;

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
        state = -1;
        betCoin = 0;
        playerCardList = new List<Card>();
        finalCards = new List<Card>();
    }

    public void AddActionRecord(int playerNum, int actionNum)
    {
        if (playerNum == seatNum)//如果是自己的行动，不需要记录
            return;
        if (playerNum < seatNum)//是自己之前的玩家进行行动 this_period
        {
            Debug.Log("PlayerNum" + playerNum + "  ActionNum" + actionNum+"    this");
        }
        else//是自己之后的玩家进行行动 last_period
        {
            Debug.Log("PlayerNum" + playerNum + "  ActionNum" + actionNum + "    last");
        }
    }
}
