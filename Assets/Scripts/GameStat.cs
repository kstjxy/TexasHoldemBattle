using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class GameStat
{
    //该类只用于给AI提供数据读取的接口！
    private Player thisPlayerInformation;
    public GameStat(Player p)
    {
        thisPlayerInformation = p;
    }

    //公共信息------------------------------------------------------------------------------
    //游戏阶段
    public int Round 
    {
        get { return GlobalVar.gameStatusCounter; }
    }
    //庄家位
    public int Button
    {
        get { return GlobalVar.curBtnSeat; }
    }
    //公共牌
    public List<Card> CommunityCards
    {
        get { return GlobalVar.publicCards; }
    }
    //所有还留在场上的玩家信息【座位号，现有金币，当局已下注金币】
    public List<int[]> PlayersInformation
    {
        get
        {
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < PlayerManager.instance.activePlayers.Count; i++)
            {
                int[] info = { PlayerManager.instance.activePlayers[i].seatNum, PlayerManager.instance.activePlayers[i].coin, PlayerManager.instance.activePlayers[i].betCoin };
                list.Add(info);
            }
            return list;
        }
    }

    //个人信息------------------------------------------------------------------------------
    //自己的座位号
    public int MyPosition
    {
        get { return thisPlayerInformation.seatNum; }
    }
    //上一轮从我位置之后的行动序列【座位号，行动序号】
    public List<int[]> Last_Period
    {
        get { return thisPlayerInformation.last_period; }
    }
    //这一轮从庄家到我的行动序列 【座位号，行动序号】
    public List<int[]> This_Period
    {
        get { return thisPlayerInformation.this_period; }
    }
    //自己的两张手牌
    public List<Card> CardsInHands
    {
        get { return thisPlayerInformation.playerCardList; }
    }
    //剩余筹码
    public int CoinsLeft
    {
        get { return thisPlayerInformation.coin; }
    }
    //该局已下注的筹码
    public int CoinsBet
    {
        get { return thisPlayerInformation.betCoin; }
    }
    //返回一个随机数，给所给的参考脚本用
    public int NumRandom
    {
        get 
        {
            System.Random ra = new System.Random(Guid.NewGuid().GetHashCode());
            int a = ra.Next(1,100);
            return a;
        } 
    }
}
