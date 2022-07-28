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
    public void BroadcastMessage(string message)//message���Ƿ��������յ�����Ϣ
    {
        var notConnectedList = new List<Client>();//������жϿ����ӵ�
        foreach (var client in clientList)//����֮ǰ�������ӳɹ��� ��ͻ��˽�������
        {
            //�ж� ��ͻ��˽������� �Ƿ������Ͽ�����
            if (client.Connected)//��������״̬
            {
                client.SendMessage(message);//��ͻ��˽������� ��ÿͻ��˷��͸���Ϣ
            }
            else//���ڶϿ�״̬
            {
                notConnectedList.Add(client);//�Ͽ��ˣ��Ͳ���Ҫ���䷢����Ϣ��
            }
        }
        foreach (var temp in notConnectedList)//���Ͽ��� ��ͻ��˽�������ɾ��
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
            Debug.Log("��������δ������");
            return false;
        }
        serverActive = false;
        server.Close();
        server.Dispose();
        return true;
    }
    
}
