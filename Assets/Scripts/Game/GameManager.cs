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
        roundinit,  //ÿһ�ֿ�ʼ��INIT��ȷ��PLAYER ROLES������
        preflop,
        flop,
        turn,
        river,
        result,      //һ����Ϸ������ʾ������������������
        gameover    //�趨�����оֽ���
    }
    public GameState currentState = GameState.loading; //0
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

    public void Loading()//��ʼ��
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
