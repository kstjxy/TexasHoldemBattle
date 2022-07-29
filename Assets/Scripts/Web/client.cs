using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client
{
    private Socket clientSocket;//��ͻ��˽�������
    private Thread t;//�߳�
    private byte[] data = new byte[1024];//��������
    public Client(Socket s)
    {
        //clientSocket���� ��ͻ��˽�������
        clientSocket = s;
        //����һ���߳� ����ͻ��˵����ݽ���
        t = new Thread(ReceiveMessage);//����һ���߳����ڴ���ReceiveMessage
                                       //��ʼִ��
        t.Start();
    }

    private void ReceiveMessage()
    {
        while (true)//����Ϊ��ѭ��������Ϊ����̣߳�������һֱ����ͻ��˽���ͨѶ
        {

            //�ڽ���ǰ���ж�һ��socket�����Ƿ�Ͽ�����ͻ��˽������� ��״̬�Ƿ������ӣ�
            //if (clientSocket.Connected==false)
            if (clientSocket.Poll(10, SelectMode.SelectRead))//10������ж�ʱ���Ƿ�Ͽ�������
            {
                clientSocket.Close();//����Ͽ������ӣ���ô��Ҫ�ر���ͻ��˵�ͨѶ����ͻ��˽������󣩣�Ҳ��������̨�ͻ��˵�ͨѶ�߳������
                break;//����ѭ������ֹ�߳�ִ��
            }

            //����ֵ��ʾ�����˶����ֽڵ�����, 
            //clientSocket.Receive(data)���ڽ��տͻ��˷��͹��������ݣ�������Ҫ����
            //��һ��1024�ֽڴ�С������data����
            int length = clientSocket.Receive(data);//��ͻ��˽������� ���յ�������
                                                    //�����ܵ���data���ݣ�ֻ�Ѵ�����0��ʼ��length���ֽ�ת��string
            string message = Encoding.UTF8.GetString(data, 0, length);
            //TODO:���յ����ݵ�ʱ�� Ҫ��������ݷַ����ͻ���
            //�㲥�����Ϣ
            WebServer.instance.BroadcastMessage(message);
            //�ڿ���̨������������յ�����Ϣ
            Debug.Log("�յ�����Ϣ��" + message);
        }
    }

    public void SendMess(string message)
    {
        //������Ҫ���͵�message���ݣ�ת��Ϊbyte�ֽ���ʽ����byte������
        byte[] data = Encoding.UTF8.GetBytes(message);
        clientSocket.Send(data);//��ͻ��˽������� �������Ϣmessage ���͸�����ͻ���
    }

    public bool Connected//�ж� ��ͻ��˽������� �Ƿ�������״̬
    {
        get { return clientSocket.Connected; }
    }

}
