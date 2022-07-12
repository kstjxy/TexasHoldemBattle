using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool isFold = false;
    public bool isAllIn = false;
    public PlayerRole role;
    public List<Card> playerCardList;
    public int state = -1; //no action

    public Player(string playerName)
    {
        this.playerName = playerName;
        this.playerCardList = new List<Card>();
    }

    
}
