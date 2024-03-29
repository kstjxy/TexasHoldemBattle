using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GlobalVar
{
    public static int totalRoundNum;            //总局数
    public static int minBetCoin;               //最小下注金额
    public static int initCoin;                 //初始金币数         
    public static int maxBetCount;              //每场最高上线数

    public static int curBetCount;              //当前加注上线
    public static int curRoundNum;              //当前局数
    public static int curBtnSeat = -1;          //当前庄家座位
    public static int pot;                      //池底金额
    public static int maxBetCoin;               //当前最大下注金额
    public static List<Card> publicCards;       //公共卡牌
    public static int gameStatusCounter = 0;    //当前游戏进程
    public static bool roundComplete = true;    //当前轮次是否结束

    public static string hostName = "";         //主机名
    public static string ipAdress = "";         //IP地址
    public static int portNum = 80;             //端口号
    public static int maxPlayerNum = 10;        //服务器最大连接数
    public static IPAddress[] ips;              //本机ip串

    public static float speedFactor = 1.0f; //游戏运行速度调整 由SLIDER控制

    public static int roboName = 0;
}