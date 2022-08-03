using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;

public class WebServer 
{
    private static WebServer _instance;
    public static WebServer instance
    {
        get
        {
            if (_instance == null)
                _instance = new WebServer();
            return _instance;
        }
    }

    public Socket server = null;
    public Thread listenThread = null;


    //int clientNum = 0;          //������
    bool serverActive = false;  //�������Ƿ���
    //bool connected = false;     //�Ƿ��пͻ�������

    public bool StartServer(string ip,int port,int playerNum)
    {
        //�Ӹ��Ϸ����ж�
        //
        
        Debug.Log(ip + port);
        
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress address = IPAddress.Parse(ip);
        IPEndPoint endPoint = new IPEndPoint(address, port);

        try
        {
            server.Bind(endPoint);
            Debug.Log("������������!");
        }
        catch(Exception e)
        {
            Debug.Log("����������ʧ�ܣ�ԭ��Ϊ��" + e.Message);
            return false;
        }
        serverActive = true;
        server.Listen(playerNum);
        listenThread = new Thread(ListenConnect);
        listenThread.IsBackground = true;
        listenThread.Start(server);
        Debug.Log("���ڼ���...\n���������������" + playerNum);
        return true;
    }
    
    Socket socketSend;
    void ListenConnect(object o)
    {
        Socket watch = o as Socket;
        while (true)
        {
            try
            {
                socketSend = watch.Accept();
                WebAI ai = new WebAI();
                ai.OnInit(socketSend);
                Player p = new(ai);
                GameStat gs = new(p);
                ai.stats = gs;
                PlayerManager.instance.allPlayers.Add(p);
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
            
        }
    }
    public bool CloseServer()
    {
        if (!serverActive)
        {
            Debug.Log("��������δ������");
            return false;
        }
        try
        {
            Thread.Sleep(100);
            serverActive = false;
            server.Close();
            server.Dispose();
            return true;
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        return false;
    }
}
