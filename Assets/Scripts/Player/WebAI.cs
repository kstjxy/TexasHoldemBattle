using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using LitJson;
using System.Threading;


[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> gamestats;
    public List<T> ToList() { return gamestats; }

    public Serialization(List<T> target)
    {
        this.gamestats = target;
    }
}


[Serializable]
public class Data
{
    //该类只用于给WebAI传输数据读取的接口！
    //公共信息------------------------------------------------------------------------------
    public int Round;                       //游戏阶段    
    public int Button;                      //庄家位    
    [SerializeField]
    public List<Card> CommunityCards;       //公共牌   
    [SerializeField]
    public List<int[]> PlayersInformation;  //所有还留在场上的玩家信息【座位号，现有金币，当局已下注金币】

    //个人信息------------------------------------------------------------------------------    
    public int MyPosition;                  //自己的座位号
    [SerializeField]
    public List<int[]> Last_Period;         //上一轮从我位置之后的行动序列【座位号，行动序号】    
    [SerializeField]
    public List<int[]> This_Period;         //这一轮从庄家到我的行动序列 【座位号，行动序号】    
    [SerializeField]
    public List<Card> CardsInHands;         //自己的两张手牌
    public int CoinsLeft;                   //剩余筹码
    public int CoinsBet;                    //该局已下注的筹码

}


public class WebAI 
{
    public string name = "my Name";
    public GameStat stats;
    public Socket client;
    public Player player;
    private Data data = new Data();
    public string file;
    private byte[] recivefrom = new byte[2048];
    private byte[] sendByte = new byte[2048];
    private string reciveString;
    private string sendto;
    public bool waitFlag = false;

    

    //多线程 会造成 玩家还没开始 流程继续 
    //多线程 + 状态阻塞
    //void ThreadSend()
    //{
    //    waitFlag = true;
    //    try
    //    {
    //        client.Send(sendByte);
    //    }
    //    catch (Exception e)
    //    {
    //        string bug = "向客户端【" + name + "】发送信息失败，可能为连接断开或超时（5S） " + e.Message;
    //        Debug.Log(bug);
    //        CloseSocket();
    //        Debug.Log("关闭此客户端的连接");
    //        PlayerManager.instance.RemovePlayer(player);
    //    }
    //    waitFlag = false;
    //}
    //void ThreadRecive()
    //{
    //    waitFlag = true;
    //    try
    //    {
    //        client.Receive(recivefrom);
    //    }
    //    catch (Exception e)
    //    {
    //        string bug = "从客户端【" + name + "】接收信息失败，可能为连接断开或超时（5S） " + e.Message;
    //        Debug.Log(bug);
    //        CloseSocket();
    //        Debug.Log("关闭此客户端的连接");
    //        PlayerManager.instance.RemovePlayer(player);
    //    }
    //    waitFlag = false;
        
    //}
    private void SendFunc(string s)
    {
        sendto = s;
        sendByte = Encoding.UTF8.GetBytes(sendto);
        client.Send(sendByte);
    }
    private void ReciveFunc()
    {
        client.Receive(recivefrom);
        reciveString = Encoding.UTF8.GetString(recivefrom);
        reciveString = reciveString.Substring(0, reciveString.IndexOf('\0'));
        Array.Clear(recivefrom, 0, recivefrom.Length);
    }
    private void SendAndReceive(string s)
    {
        SendFunc(s);
        ReciveFunc();
    }

    private void SendGameStat()
    {
        data.Round = stats.Round;
        data.Button = stats.Button;
        data.CommunityCards = new List<Card>();
        data.CommunityCards = stats.CommunityCards;
        data.PlayersInformation = new List<int[]>();
        data.PlayersInformation = stats.PlayersInformation;
        data.MyPosition = stats.MyPosition;
        data.Last_Period = new List<int[]>();
        data.Last_Period = stats.Last_Period;
        data.CardsInHands = new List<Card>();
        data.CardsInHands = stats.CardsInHands;
        data.CoinsLeft = stats.CoinsLeft;
        data.CoinsBet = stats.CoinsBet;
        List<Data> dataList = new List<Data>();
        dataList.Add(data);
        string jsonStat = JsonMapper.ToJson(data);
        Debug.Log(jsonStat);
        sendByte = Encoding.UTF8.GetBytes(jsonStat);
        client.Send(sendByte);
    }

    private void PrintL(string s)
    {
        UIManager.instance.logList.Add(s);
    }
    public void OnInit(Socket socketsend)
    {
        client = socketsend;
        //接受与发送的超时时间均设为5s
        client.SendTimeout = 5000;
        client.ReceiveTimeout = 5000;
        
        SendAndReceive("OnInit");
        SendFunc("服务器haihai");
        name = reciveString;
        Debug.Log(name + "已连接到服务器");
        PrintL(name + "已连接到服务器");
        //name = test.name;
    }

    public void StartGame()
    {
        SendAndReceive("StartGame");
        Debug.Log(name + "初始化成功！\n" + reciveString);
        PrintL(name + "初始化成功！\n" + reciveString);
    }

    public void RoundStart()
    {
        SendAndReceive("RoundStart");
        SendGameStat();
        Debug.Log(reciveString + "新一轮已就绪！");
    }

    //1:跟注；2：加注；3：弃牌；4：ALLIN
    public int BetAction()
    {
        SendFunc("BetAction");
        SendGameStat();
        ReciveFunc();
        //合法性判断
        if (reciveString[0] < '1' || reciveString[0] > '4')
        {
            string bug = "玩家【" + name + "】所作操作不合法！默认弃牌！";
            Debug.Log(bug);
            PrintL(bug);
            return 3; //如果操作错误就弃牌
        }

        return reciveString[0] - '0';

    }

    public List<Card> FinalSelection()
    {
        List<int> cardNum = new List<int>();
        List<Card> result = new List<Card>();
        SendFunc("FinalSelection");
        SendGameStat();
        ReciveFunc();
        //合法判断，格式确定
        //foreach(char ch in reciveString)
        //{
        //    cardNum.Add(ch - '0');
        //}
        for (int i = 0; i < 5; i++)
        {
            cardNum.Add(reciveString[i] - '0');
        }


        foreach (int i in cardNum)
        {
            int j = i;
            if (i < 0 || i > 6)
                j = 0;
            if (j < 2)
                result.AddRange(stats.CardsInHands.GetRange(i, 1));
            else
                result.AddRange(stats.CommunityCards.GetRange(i - 2, 1));
        };
        return result;
    }
    public void CloseSocket()
    {
        try{
            client.Close();
            client.Dispose();
            Debug.Log(name + "已关闭连接");
            PrintL(name + "已关闭连接");
        }
        catch(Exception e)
        {
            Debug.Log(name + "关闭失败" + e.Message);
            PrintL(name + "关闭失败" + e.Message);
        }
        
        
        //name = test.name;
    }
}
