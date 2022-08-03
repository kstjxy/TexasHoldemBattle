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


    //int clientNum = 0;          //连接数
    bool serverActive = false;  //服务器是否开启
    //bool connected = false;     //是否有客户端连接

    public bool StartServer(string ip,int port,int playerNum)
    {
        //加个合法性判断
        //
        
        Debug.Log(ip + port);
        
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress address = IPAddress.Parse(ip);
        IPEndPoint endPoint = new IPEndPoint(address, port);

        try
        {
            server.Bind(endPoint);
            Debug.Log("服务器已启动!");
        }
        catch(Exception e)
        {
            Debug.Log("服务器启动失败，原因为：" + e.Message);
            return false;
        }
        serverActive = true;
        server.Listen(playerNum);
        listenThread = new Thread(ListenConnect);
        listenThread.IsBackground = true;
        listenThread.Start(server);
        Debug.Log("正在监听...\n最大可连接玩家数：" + playerNum);
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
            Debug.Log("服务器尚未开启！");
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
