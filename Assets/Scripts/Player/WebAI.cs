using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using Unity;

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

    private void Send(string s)
    {
        sendto = s;
        sendByte = Encoding.UTF8.GetBytes(sendto);
        client.Send(sendByte);
    }
    private void Receive()
    {
        client.Receive(recivefrom);
        reciveString = Encoding.UTF8.GetString(recivefrom);
        reciveString = reciveString.Substring(0, reciveString.IndexOf('\0'));
        Array.Clear(recivefrom, 0, recivefrom.Length);
    }
    private void SendAndReceive(string s)
    {
        Send(s);
        Receive();
    }
    private void SendGameStats()
    {
        string jsonStat = JsonUtility.ToJson(stats);
        sendByte = Encoding.UTF8.GetBytes(jsonStat);
        client.Send(sendByte);
    }

    private void SendGameStat()
    {
        string jsonStat = JsonUtility.ToJson(stats);
        sendByte = Encoding.UTF8.GetBytes(jsonStat);
        client.Send(sendByte);

    }

    public void OnInit(Socket socketsend)
    {
        client = socketsend;
        //接受与发送的超时时间均设为5s
        client.SendTimeout = 5000;
        client.ReceiveTimeout = 5000;
        SendAndReceive("OnInit");
        name = reciveString;
        Send("服务器haihai");
        Debug.Log(name + "已连接到服务器");
        UIManager.instance.PrintThread(name + "已连接到服务器");
        //name = test.name;
    }

    public void StartGame()
    {
        SendAndReceive("StartGame");
        SendGameStats();
        Debug.Log(name + "初始化成功！\n" + reciveString);
        UIManager.instance.PrintThread(name + "初始化成功！\n" + reciveString);       
    }

    public void RoundStart()
    {
        SendAndReceive("RoundStart");
        SendGameStat();
        Receive();
        Debug.Log(reciveString + "新一轮已就绪！");
    }

    //1:跟注；2：加注；3：弃牌；4：ALLIN
    public int BetAction()
    {
        Send("BetAction");
        SendGameStat();
        Receive();
        //合法性判断
        if (reciveString[0] < '1' || reciveString[0] > '4')
        {
            string bug = "玩家【" + name + "】所作操作不合法！默认弃牌！";
            Debug.Log(bug);
            UIManager.instance.PrintThread(bug);
            return 3; //如果操作错误就弃牌
        }
        return reciveString[0] - '0';

    }

    public List<Card> FinalSelection()
    {
        List<int> cardNum = new List<int>();
        List<Card> result = new List<Card>();
        Send("FinalSelection");
        SendGameStat();
        Receive();
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
                result.AddRange(stats.CardsInHands.GetRange(j, 1));
            else
                result.AddRange(stats.CommunityCards.GetRange(j - 2, 1));
        };
        return result;
    }
    public void CloseSocket()
    {
        try{
            client.Close();
            client.Dispose();
            Debug.Log(name + "已关闭连接");
            UIManager.instance.PrintThread(name + "已关闭连接");
        }
        catch(Exception e)
        {
            Debug.Log(name + "关闭失败" + e.Message);
            UIManager.instance.PrintThread(name + "关闭失败" + e.Message);
        }
        
        
        //name = test.name;
    }
}
