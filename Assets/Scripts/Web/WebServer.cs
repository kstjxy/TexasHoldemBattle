using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public class WebServer : MonoBehaviour
{
    TcpListener server;
    public static WebServer instance;
    void Awake()
    {
        instance = this;
    }
    public void StartServer(string ip,int port)
    {
        //加个合法性判断
        //
        server = new TcpListener(IPAddress.Parse(ip), port);
        server.Start();
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
