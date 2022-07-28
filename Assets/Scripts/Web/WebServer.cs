using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public class WebServer
{
    TcpListener server;
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
    public int clientNum = 0;
    public bool serverActive = false;
    public void StartServer(string ip,int port)
    {
        //加个合法性判断
        //
        Debug.Log(ip + port);
        server = new TcpListener(IPAddress.Parse(ip), port);
        server.Start();
        serverActive = true;
        Debug.Log("服务器已启动正在监听...");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
