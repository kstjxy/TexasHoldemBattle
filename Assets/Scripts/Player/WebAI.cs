using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;

public class WebAI 
{
    public string name = "my Name";
    public GameStat stats;
    public Socket client;
    public Player player;
    public string file;
    private byte[] recivefrom = new byte[2048];
    private byte[] sendByte = new byte[2048];
    private string reciveString;
    private string sendto;
    public bool waitFlag = false;

    //多线程 会造成 玩家还没开始 流程继续 
    //多线程 + 状态阻塞
    void ThreadSend()
    {
        waitFlag = true;
        try
        {
            client.Send(sendByte);
        }
        catch (Exception e)
        {
            string bug = "向客户端【" + name + "】发送信息失败，可能为连接断开或超时（5S） " + e.Message;
            Debug.Log(bug);
            CloseSocket();
            Debug.Log("关闭此客户端的连接");
            PlayerManager.instance.RemovePlayer(player);
        }
        waitFlag = false;
    }
    void ThreadRecive()
    {
        waitFlag = true;
        try
        {
            client.Receive(recivefrom);
        }
        catch (Exception e)
        {
            string bug = "从客户端【" + name + "】接收信息失败，可能为连接断开或超时（5S） " + e.Message;
            Debug.Log(bug);
            CloseSocket();
            Debug.Log("关闭此客户端的连接");
            PlayerManager.instance.RemovePlayer(player);
        }
        waitFlag = false;
        
    }
    private void SendFunc(string s)
    {
        sendto = s;
        sendByte = Encoding.UTF8.GetBytes(sendto);
        Thread thread = new Thread(ThreadSend);
        thread.IsBackground = true;
        thread.Start();
    }
    private void ReciveFunc()
    {
        Thread thread = new Thread(ThreadRecive);
        thread.IsBackground = true;
        thread.Start();
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
        string jsonStat = JsonUtility.ToJson(stats);
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
        SendFunc("服务器haihai");
        SendAndReceive("OnInit");
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
        SendAndReceive("FinalSelection");
        SendGameStat();
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
