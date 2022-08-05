using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using System.Text;

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
    public List<Socket> sockets = new List<Socket>();
    int oldLenSockets;
    int nowLenSockets;


    //int clientNum = 0;          //������
    bool serverActive = false;  //�������Ƿ���
    //bool connected = false;     //�Ƿ��пͻ�������
    IPEndPoint endPoint;

    public bool StartServer(string ip,int port,int playerNum)
    {
        //�Ӹ��Ϸ����ж�
        //
        
        Debug.Log(ip + port);
        
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress address = IPAddress.Parse(ip);
        endPoint = new IPEndPoint(address, port);

        try
        {
            server.Bind(endPoint);
            Debug.Log("������������!");
            serverActive = true;
            sockets = new List<Socket>();
            oldLenSockets = 0;
            nowLenSockets = 0;
            server.Listen(playerNum);
            listenThread = new Thread(ListenConnect);
            listenThread.IsBackground = true;
            listenThread.Start(server);
            Debug.Log("���ڼ���...\n���������������" + playerNum);
            return true;
        }
        catch(Exception e)
        {
            Debug.Log("����������ʧ�ܣ�ԭ��Ϊ��" + e.Message);
            return false;
        }
    }
    
    void ListenConnect(object o)
    {
        Socket watch = o as Socket;
        Socket socketSend;
        while (true)
        {
            try
            {
                socketSend = watch.Accept();
                //Э��ʽȡ��,ֹͣ�߳�
                if (!serverActive) break;
                sockets.Add(socketSend);
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
            
        }
    }

    public void UpdatePlayers()
    {
        nowLenSockets = sockets.Count;
        if (oldLenSockets == nowLenSockets) return;

        for (int i = oldLenSockets; i < nowLenSockets; i++)
        {
            WebAI ai = new WebAI();
            ai.OnInit(sockets[i]);
            Player p = new(ai);
            GameStat gs = new(p);
            ai.stats = gs;
            ai.player = p;
            PlayerManager.instance.allPlayers.Add(p);
        }
        oldLenSockets = nowLenSockets;

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
            //����״̬
            serverActive = false;
            //�ر�������watch.Accept();
            Socket endSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endSocket.Connect(endPoint);
            endSocket.Send(Encoding.UTF8.GetBytes("End"));
            endSocket.Close();
            endSocket.Dispose();
            foreach(Player p in PlayerManager.instance.allPlayers)
            {
                if (p.type == Player.aiType.WebAI)
                {
                    p.webAI.client.Close();
                    p.webAI.client.Dispose();
                }
            }
            server.Close();
            server.Dispose();
            Debug.Log("�������ѹر�!");
            return true;
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        return false;
    }
}
