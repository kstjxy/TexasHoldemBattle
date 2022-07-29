using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System;

public class BaseAI
{
    public string name = "my Name";
    public GameStat stats;
    public Socket server = WebServer.instance.server;
    public string file;
    byte[] recivefrom = new byte[1024];
    string sendto;

    public  void OnInit(Socket socketsend)
    {
        
        
        //name = test.name;
    }

    public void StartGame()
    {
        sendto = "Game Start";
        server.Send(Encoding.UTF8.GetBytes(sendto));
        server.Receive(recivefrom);

        Debug.Log(name + "��ʼ���ɹ���");
    }

    public void RoundStart()
    {
        Debug.Log(name + "��һ���Ѿ�����");
    }

    //1:��ע��2����ע��3�����ƣ�4��ALLIN
    public int BetAction()
    {
        int act=100;
        if (act > 0 && act < 5)
        {
            return act;
        } else
        {
            string bug = "��ҡ�" + name + "�������������Ϸ���Ĭ�����ƣ�";
            Debug.Log(bug);
            UIManager.instance.PrintLog(bug);
            return 3; //����������������
        }
    }

    public  List<Card> FinalSelection()
    {
        List<int> cardNum = new List<int>();        
        List<Card> result = new List<Card>();

        foreach (int i in cardNum)
        {
            if (i < 2)
                result.AddRange(stats.CardsInHands.GetRange(i, 1));
            else
                result.AddRange(stats.CommunityCards.GetRange(i-2, 1));
        };
        return result;
    }

    public byte[] MyLoader(ref string filepath)
    {
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(this.file));
    }

}
