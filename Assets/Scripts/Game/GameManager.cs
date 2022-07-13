using System.Collections;
using System.Collections.Generic;
using System.Timers;
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
        init,       //������Ϸ��ʼ��
        roundinit,  //ÿ����Ϸ��ʼ����ȷ��ׯ�� ��äСä
        preflop,    
        flop,
        turn,
        river,
        calc,       //һ����Ϸ����
        gameover    //�趨�����оֽ���
    }
    public GameState currentState = GameState.init; //0
    //����ת��  ask
    public static int GameStatus()
    {
        int gamestate = GlobalVar.gameStatusCounter;
        if (gamestate < 7)
        {

        }
    }

    //ȫ�ֱ���
    public static List<Player> playerList = new List<Player>(); //����б� PlayerObject
    public static List<Card> cardList = new List<Card>();       //������
    public static List<int> allNum = new List<int>();           //������ҵ����
    public static List<int> playingNum = new List<int>();       //���������ҵ����
    public static List<int> lostNum = new List<int>();          //����̭��ҵ����
    public static int buton;                                    //ׯ�����
    public static int setRound;                                 //�趨�ܾ���
    public static int initMoney;                                //
    public static int betMonet;                                 //
    public static int curRound;                                 //��ǰ����
    public Timer time;                                          //���ڵ����ٶ�

    public void Init()//��ʼ��
    {
        //ȷ������б�
        //��ע��С
        //��ʼǮ��
        //�ܾ���
        //��ע����
        currentState = GameState.init;
        curRound = 0;

    }
    public void RoundInit()//��ʼ��
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
        if 
        switch (currentState)
        {
            case GameState.init:
                RoundInit();
                break;
            case GameState.roundinit:
                Preflop();
                break;
            case GameState.preflop:
                Flop();
                break;
            case GameState.flop:
                Turn();
                break;
            case GameState.turn:
                River();
                break;
            case GameState.river:
                Calc();
                break;
            case GameState.calc:
                curRound++;
                if (curRound == setRound)
                {
                    Gameover();
                    break;
                }
                 
                RoundInit();
                break;
        }
        // throw new UnityException("GameState Error" + currentState)
    }
}
