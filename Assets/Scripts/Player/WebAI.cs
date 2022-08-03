using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System;

public class WebAI
{   
    public string name = "my Name";
    public GameStat stats;
    public Socket client;
    public string file;
    byte[] recivefrom = new byte[1024];
    byte[] sendByte = new byte[1024];
    string reciveString;
    string sendto;

    private void SendAndReceive(string s)
    {
        sendto = s;
        sendByte = Encoding.UTF8.GetBytes(sendto);
        client.Send(sendByte);
        client.Receive(recivefrom);
        reciveString = Encoding.UTF8.GetString(recivefrom);
        reciveString = reciveString.Substring(0, reciveString.IndexOf('\0'));
        Array.Clear(recivefrom, 0, recivefrom.Length);
    }

    private void PrintL(string s)
    {
        UIManager.instance.logList.Add(s);
    }
    public  void OnInit(Socket socketsend)
    {
        client = socketsend;
        SendAndReceive("OnInit");
        name = reciveString;
        client.Send(Encoding.UTF8.GetBytes("服务器haihai"));
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
        Debug.Log(reciveString + "新一轮已就绪！");
    }

    //1:跟注；2：加注；3：弃牌；4：ALLIN
    public int BetAction()
    {
        SendAndReceive("BetAction");
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

    public  List<Card> FinalSelection()
    {
        List<int> cardNum = new List<int>();
        List<Card> result = new List<Card>();
        SendAndReceive("FinalSelection");
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
            if (i < 2)
                result.AddRange(stats.CardsInHands.GetRange(i, 1));
            else
                result.AddRange(stats.CommunityCards.GetRange(i-2, 1));
        };
        return result;
    }

}
