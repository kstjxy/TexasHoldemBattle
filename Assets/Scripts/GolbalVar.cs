using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVar
{
    public static int totalRoundNum;            //�ܾ���
    public static int minBetCoin;               //��С��ע���
    public static int initCoin;                 //��ʼ�����         
    public static int maxBetCount;              //ÿ�����������

    public static int curBetCount;              //��ǰ��ע����
    public static int curRoundNum;              //��ǰ����
    public static int curBtnSeat = -1;          //��ǰׯ����λ
    public static int pot;                      //�ص׽��
    public static int maxBetCoin;               //��ǰ�����ע���
    public static List<Card> publicCards;       //��������
    public static int gameStatusCounter = 0;    //��ǰ��Ϸ����
    public static bool roundComplete = true;    //��ǰ�ִ��Ƿ����

    public static string ipAdress = "127.0.0.1";//IP��ַ
    public static int portNum = 80;               //�˿ں�

    public static float speedFactor = 1.0f; //��Ϸ�����ٶȵ��� ��SLIDER����

    public static int roboName = 0;
}