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

    //���߳� ����� ��һ�û��ʼ ���̼��� 
    //���߳� + ״̬����
    void ThreadSend()
    {
        waitFlag = true;
        try
        {
            client.Send(sendByte);
        }
        catch (Exception e)
        {
            string bug = "��ͻ��ˡ�" + name + "��������Ϣʧ�ܣ�����Ϊ���ӶϿ���ʱ��5S�� " + e.Message;
            Debug.Log(bug);
            CloseSocket();
            Debug.Log("�رմ˿ͻ��˵�����");
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
            string bug = "�ӿͻ��ˡ�" + name + "��������Ϣʧ�ܣ�����Ϊ���ӶϿ���ʱ��5S�� " + e.Message;
            Debug.Log(bug);
            CloseSocket();
            Debug.Log("�رմ˿ͻ��˵�����");
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
        //�����뷢�͵ĳ�ʱʱ�����Ϊ5s
        client.SendTimeout = 5000;
        client.ReceiveTimeout = 5000;
        SendFunc("������haihai");
        SendAndReceive("OnInit");
        name = reciveString;
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
        SendGameStat();
        Debug.Log(reciveString + "��һ���Ѿ�����");
    }

    //1:��ע��2����ע��3�����ƣ�4��ALLIN
    public int BetAction()
    {
        SendFunc("BetAction");
        SendGameStat();
        ReciveFunc();
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

    public List<Card> FinalSelection()
    {
        List<int> cardNum = new List<int>();
        List<Card> result = new List<Card>();
        SendAndReceive("FinalSelection");
        SendGameStat();
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
            Debug.Log(name + "�ѹر�����");
            PrintL(name + "�ѹر�����");
        }
        catch(Exception e)
        {
            Debug.Log(name + "�ر�ʧ��" + e.Message);
            PrintL(name + "�ر�ʧ��" + e.Message);
        }
        
        
        //name = test.name;
    }
}
