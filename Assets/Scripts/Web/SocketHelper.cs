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
    //��Զ�����ӹ����Ŀͷ��˵�IP��ַ��Socket���뼯��
    public static SocketHelper Intanter = new SocketHelper();
    public Dictionary<string, Socket> dicScoket = new Dictionary<string, Socket>();
    public List<string> IPItem = new List<string>();

    //��Ϣ����ö��
    public enum MessageType
    {
        news,
        picture
    }

    /// <summary>
    ///  ����˽������ݺ����ݷ��͸����еĿͻ���
    /// </summary>
    /// <param name="buffer">���͵���Ϣ�ֽ�</param>
    /// <param name="ms">��Ϣ����</param>
    /// <returns></returns>
    public void SendMessage(byte[] buffer)
    {

        //����û����������IP��ַ
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
    ///  ����Ϣת������ϢЭ���ʽ
    /// </summary>
    /// <param name="buffer">���͵���Ϣ�ֽ�</param>
    /// <param name="ms">��Ϣ����</param>
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
