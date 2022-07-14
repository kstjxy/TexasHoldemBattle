using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ����
    public static GameManager instance;
    void Awake()
    {
        instance = this;
    }

    //��Ϸ����
    public enum GameState
    {
        loading,    //��Ϸδ��ʼ
        setting,    //���ý��棬������Ϸ����
        init,       //��ʼ�����л����棬�������
        roundinit,  //ÿһ�ֿ�ʼ��INIT��ȷ��PLAYER ROLES
        preflop,    //������������� ��һ����ע
        flop,       //�������������� �ڶ�����ע
        turn,       //�������ص������� ��������ע
        river,      //�������ص������� ��������ע
        result,     //һ����Ϸ������ʾ������������������
        gameover    //�趨�����оֽ���
    }
    
    public static GameState GameStatus()
    {
        switch (GolbalVar.gameStatusCounter)
        {
            case -3:
                return GameState.loading;
            case -2:
                return GameState.setting;
            case -1:
                return GameState.init;
            case 0:
                return GameState.roundinit;
            case 1:
                return GameState.preflop;
            case 2:
                return GameState.flop;
            case 3:
                return GameState.turn;
            case 4:
                return GameState.river;
            case 5:
                return GameState.result;
            case 6:
                return GameState.gameover;
        }
        throw new UnityException("GameStatus error");
    }

    //ȫ�ֱ���
    public static List<Player> playerList = new List<Player>(); //����б� PlayerObject
    public static List<Card> cardList = new List<Card>();       //������
    public static List<int> allNum = new List<int>();           //������ҵ����
    public static List<int> playingNum = new List<int>();       //���������ҵ����
    public static List<int> lostNum = new List<int>();          //����̭��ҵ����
    public static int buton;                                    //ׯ�����
    public static int totalRound;                               //�ܾ���
    public static int initMoney;                                //
    public static int betMonet;                                 //

    public void Loading()//��ʼ��
    {
        //ȷ������б�
        //��ע��С
        //��ʼǮ��
        //�ܾ���
        //��ע����
    }
    public void RoundInit()//��ʼ��
    {
        //ȷ������б�
        //��ע��С
        //��ʼǮ��
        //�ܾ���
        //��ע����
    }
    // Start is called before the first frame update
    void Begin()
    {	
        
    }

    // Update is called once per frame
    void Update()
    {   

    }
}
