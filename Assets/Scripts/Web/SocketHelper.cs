using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

public class SocketHelper 
{
    //将远程连接过来的客服端的IP地址和Socket存入集合
    public static SocketHelper Intanter = new SocketHelper();
    public Dictionary<string, Socket> dicScoket = new Dictionary<string, Socket>();
    public List<string> IPItem = new List<string>();

    //消息类型枚举
    public enum MessageType
    {
        news,
        picture
    }

    /// <summary>
    ///  服务端接收数据后将数据发送给所有的客户端
    /// </summary>
    /// <param name="buffer">发送的消息字节</param>
    /// <param name="ms">消息类型</param>
    /// <returns></returns>
    public void SendMessage(byte[] buffer)
    {

        //获得用户在下拉框的IP地址
        var task1 = new Task(() =>
        {
            for (int i = 0; i < IPItem.Count; i++)
            {
                string ip = IPItem[i].ToString();
                dicScoket[ip].Send(buffer.ToArray());
            }
        });
        task1.Start();
    }
    /// <summary>
    ///  将消息转换成消息协议格式
    /// </summary>
    /// <param name="buffer">发送的消息字节</param>
    /// <param name="ms">消息类型</param>
    /// <returns></returns>
    public byte[] SendMessageToClient(string message, MessageType ms = MessageType.news)
    {
        List<byte> newbuffer = new List<byte>();
        byte[] buffer = new byte[0];
        switch (ms)
        {
            case MessageType.news:
                newbuffer.Add(0);
                buffer = Encoding.UTF8.GetBytes(message);
                break;
            case MessageType.picture:
                newbuffer.Add(1);
                buffer = Encoding.UTF8.GetBytes(message);
                break;
            default:
                break;
        }
        newbuffer.AddRange(buffer);
        return newbuffer.ToArray();
    }
    public byte[] RemoveFbyte(byte[] buffer)
    {
        List<byte> newbuffer = buffer.ToList();
        newbuffer.RemoveAt(0);
        return newbuffer.ToArray();
    }

}
