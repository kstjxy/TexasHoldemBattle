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
    public static List<string> namelist = new List<string>();   //�������
    public static List<Player> playerList = new List<Player>(); //����б� PlayerObject
    public static List<Card> cardList = new List<Card>();       //������
    public static List<int> allNum = new List<int>();           //������ҵ����
    public static List<int> playingNum = new List<int>();       //���������ҵ����
    public static List<int> lostNum = new List<int>();          //����̭��ҵ����
    public static int buton;                                    //ׯ�����
    public static int setRound;                                 //�趨�ܾ���
    public static int initMoney;                                //
    public static int betMoney;                                 //
    public static int curRound;                                 //��ǰ����
    public float time = 0;                                      //���ڵ����ٶȣ���ʱ
    public double gapset = 5;                                   //ʱ����
    public double ratioTime = 0;                                //�϶�������
    public float gaptime = 5;                                   //������ʱ���� 
    public bool started = 0;                                    //��Ϸ��ʼ��־                
    public static string debugLogStr;
    public static string DebugLogStr
    {
        get => debugLogStr;
        set => debugLogStr = value + '\n';
    }



    public void Init()//��ʼ��
    {
        Debug.log("\t\t��ʼ��ing\t\t");
        //��UI����ȡ��������б�δ��ɣ�
        namelist.Add("001");
        namelist.Add("002");
        namelist.Add("003");
        //ȷ������б�
        PlayerManager.instance.IniPlayers(namelist) = initMoney;
        //�μӵ���ң���Ŵ�1��ʼ
        int tmpNo = 1;
        foreach (Player p in PlayerManager.instance.allPlayers)
        {
            //���Լ�һ���治����жϣ���
            PlayerManager.instance.SeatedPlayer(p, tmpNo++);
        }
        //��ע��С
        betMoney = 5;
        //��ʼǮ��
        initMoney = 2000;
        //�ܾ���
        setRound = 20;
        //��ע����?
        //��ʼ��gamestate
        currentState = GameState.init;
        curRound = 0;
        

    }
    public void SetGap(double ratio)
    {
        gaptime = (1.01 - ratio) * gapset;
    }
    public void RoundInit()//��ʼ��
    {
        Debug.log("\t\t\t\t")
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
        if ��started){
            if (time >= gaptime)
            {
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
                time = 0;
            }
            else time += Time.deltaTime;
        }
       
    }
}
