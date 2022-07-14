using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolbalVar
{
    public static int totalRoundNum;    //总局数
    public static int minBetCoin;       //最小下注金额
    public static int initCoin;         //初始金币数         
    public static int maxBetCount;      //每场最高上线数

    public static int curBetCount;      //当前加注上线
    public static int curRoundNum;      //当前局数
    public static int curBtnSeat;       //当前庄家座位
    public static int bot;              //池底金额
    public static int maxBetCoin;       //当前最大下注金额
    public static List<Card> publicCards;    //公共卡牌
    public static int gameStatusCounter = 0;    //当前游戏进程
}
