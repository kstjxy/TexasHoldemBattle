using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;

public class WebAI : MonoBehaviour
{
    public new string name = "my Name";
    public GameStat stats;
    public Socket client;
    public string file;
    private byte[] recivefrom = new byte[2048];
    private byte[] sendByte = new byte[2048];
    private string reciveString;
    private string sendto;

    private IEnumerator SendAndReceive(string s)
    {
        sendto = s;
        sendByte = Encoding.UTF8.GetBytes(sendto);
        client.Send(sendByte);
        client.Receive(recivefrom);

        reciveString = Encoding.UTF8.GetString(recivefrom);
        reciveString = reciveString.Substring(0, reciveString.IndexOf('\0'));
        Array.Clear(recivefrom, 0, recivefrom.Length);
        yield return null;
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
        StartCoroutine(SendAndReceive("OnInit"));
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
        SendGameStat();
        Debug.Log(reciveString + "��һ���Ѿ�����");
    }

    //1:��ע��2����ע��3�����ƣ�4��ALLIN
    public int BetAction()
    {
        SendAndReceive("BetAction");
        SendGameStat();
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
            if (i < 2)
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
            Debug.Log(name + "�ر�ʧ��");
            PrintL(name + "�ر�ʧ��");
        }
        
        
        //name = test.name;
    }
}
