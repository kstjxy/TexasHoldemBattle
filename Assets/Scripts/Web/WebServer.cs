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

    Socket server = null;
    Thread listenThread = null;
    static List<Client> clientList = new List<Client>();


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
    public void BroadcastMessage(string message)//message就是服务器接收到的消息
    {
        var notConnectedList = new List<Client>();//存放所有断开连接的
        foreach (var client in clientList)//遍历之前所有连接成功的 与客户端交互对象
        {
            //判断 与客户端交互对象 是否与服务断开连接
            if (client.Connected)//处于连接状态
            {
                client.SendMessage(message);//与客户端交互对象 向该客户端发送该消息
            }
            else//处于断开状态
            {
                notConnectedList.Add(client);//断开了，就不需要向其发送消息了
            }
        }
        foreach (var temp in notConnectedList)//将断开的 与客户端交互对象删掉
        {
            clientList.Remove(temp);
        }
    }

    Socket socketSend;
    void ListenConnect(object o)
    {
        Socket watch = o as Socket;
        while (true)
        {
            socketSend = watch.Accept();
            Client client = new Client(socketSend);
            clientList.Add(client);
        }
    }
    public bool CloseServer()
    {
        if (!serverActive)
        {
            Debug.Log("服务器尚未开启！");
            return false;
        }
        serverActive = false;
        server.Close();
        server.Dispose();
        return true;
    }
    
}
