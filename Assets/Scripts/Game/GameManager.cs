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
        init,       //��ʼ����ȷ��ׯ�� ��äСä
        roundinit,  //
        preflop,    //
        flop,
        turn,
        river,
        calc,       //һ����Ϸ����
        gameover    //�趨�����оֽ���
    }
    public GameState currentState = GameState.init; //0
                                                    //����ת��  ask
    /*
    public static int GameStatus()
    {
        
        int gamestate = GlobalVar.gameStatusCounter;
        if (gamestate < 7)
        {

        }
        
}
    */

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

    public void Init()//��ʼ��
    {
        //ȷ������б�
        //��ע��С
        //��ʼǮ��
        //�ܾ���
        //��ע����
        currentState = GameState.init;
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

        switch (currentState)
        {
            case GameState.init:
                RoundInit();
                break;

        }
    }
}
