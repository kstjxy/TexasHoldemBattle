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
        client.Send(Encoding.UTF8.GetBytes("������haihai"));
        Debug.Log(name + "�����ӵ�������");
        PrintL(name + "�����ӵ�������");
        //name = test.name;
    }

    public void StartGame()
    {
        SendAndReceive("StartGame");
        Debug.Log(name + "��ʼ���ɹ���\n" + reciveString);
        PrintL(name + "��ʼ���ɹ���\n" + reciveString);
    }

    public void RoundStart()
    {
        SendAndReceive("RoundStart");
        Debug.Log(reciveString + "��һ���Ѿ�����");
    }

    //1:��ע��2����ע��3�����ƣ�4��ALLIN
    public int BetAction()
    {
        SendAndReceive("BetAction");
        //�Ϸ����ж�
        if (reciveString[0] < '1' || reciveString[0] > '4')
        {
            string bug = "��ҡ�" + name + "�������������Ϸ���Ĭ�����ƣ�";
            Debug.Log(bug);
            PrintL(bug);
            return 3; //����������������
        }
        
        return reciveString[0] - '0';
               
    }

    public  List<Card> FinalSelection()
    {
        List<int> cardNum = new List<int>();
        List<Card> result = new List<Card>();
        SendAndReceive("FinalSelection");
        //�Ϸ��жϣ���ʽȷ��
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
