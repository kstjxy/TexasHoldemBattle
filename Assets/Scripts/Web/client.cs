using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client
{
    private Socket clientSocket;//与客户端交互对象
    private Thread t;//线程
    private byte[] data = new byte[1024];//数据容器
    public Client(Socket s)
    {
        //clientSocket就是 与客户端交互对象
        clientSocket = s;
        //启动一个线程 处理客户端的数据接收
        t = new Thread(ReceiveMessage);//创建一个线程用于处理ReceiveMessage
                                       //开始执行
        t.Start();
    }

    private void ReceiveMessage()
    {
        while (true)//设置为死循环，是因为这个线程：服务器一直会与客户端进行通讯
        {

            //在接收前，判断一下socket连接是否断开（与客户端交互对象 的状态是否处于连接）
            //if (clientSocket.Connected==false)
            if (clientSocket.Poll(10, SelectMode.SelectRead))//10毫秒的判断时间是否断开连接了
            {
                clientSocket.Close();//如果断开了连接，那么就要关闭与客户端的通讯（与客户端交互对象），也就是与这台客户端的通讯线程完成了
                break;//跳出循环，终止线程执行
            }

            //返回值表示接收了多少字节的数据, 
            //clientSocket.Receive(data)用于接收客户端发送过来的数据，数据需要接收
            //用一个1024字节大小的容器data接收
            int length = clientSocket.Receive(data);//与客户端交互对象 接收到的数据
                                                    //将接受到的data数据，只把从索引0开始的length个字节转化string
            string message = Encoding.UTF8.GetString(data, 0, length);
            //TODO:接收到数据的时候 要把这个数据分发到客户端
            //广播这个消息
            WebServer.instance.BroadcastMessage(message);
            //在控制台输出服务器接收到的消息
            Debug.Log("收到了消息：" + message);
        }
    }

    public void SendMess(string message)
    {
        //将将需要发送的message数据，转化为byte字节形式存入byte数组中
        byte[] data = Encoding.UTF8.GetBytes(message);
        clientSocket.Send(data);//与客户端交互对象 将这个消息message 发送给这个客户端
    }

    public bool Connected//判断 与客户端交互对象 是否处于连接状态
    {
        get { return clientSocket.Connected; }
    }

}
