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
    public bool isInGame = false;
    public bool isFold = false;
    public bool isAllIn = false;
    public PlayerRole role = PlayerRole.outOfGame;
    public List<Card> playerCardList = new List<Card>();
    public int state = -1; //no action
    public List<Card> finalCards = new List<Card>();
    public PlayerObject playerObject;

    public Player(string playerName)
    {
        this.playerName = playerName;
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
        playerCardList = new List<Card>();
        finalCards = new List<Card>();
    }
}
