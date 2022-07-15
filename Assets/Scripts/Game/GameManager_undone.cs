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
    public static List<string> namelist = new List<string>();       //�������
    public static List<Player> allPlayers = new List<Player>();     //ȫ����� PlayerObject pm��
    public static List<Player> seatedPlayers = new List<Player>();  //δ��̭��� PlayerObject
    public static List<Player> playing = new List<Player>();        //�����˴˾ֵ����
    public static List<Player> activePlayers = new List<Player>();  //����������
    public static List<Player> winPlayers = new List<Player>();     //Ӯ�ô˾����
    public static List<Player> lostPlayers = new List<Player>();    //��̭���
    public static List<Card> cardList = new List<Card>();           //������
    public static int buton;                                        //ׯ�����
    public static int setRound;                                     //�趨�ܾ���
    public static int initMoney;                                    //
    public static int betMoney;                                     //��ע
    public static int betTotal;                                     //��������ע
    public static int betcount;                                     //ÿ���׶������ע��
    public static int curRound;                                     //��ǰ����
    public int buttonNum;                                           //ׯ��
    public float time = 0;                                          //���ڵ����ٶȣ���ʱ
    public float gapset = 5;                                        //ʱ����
    public float ratioTime = 0;                                     //�϶�������
    public float gaptime = 5;                                       //������ʱ���� 
    public bool started = 0;                                        //��Ϸ��ʼ��־                
    public static string debugLogStr;
    public static string DebugLogStr
    {
        get => debugLogStr;
        set => debugLogStr = value + '\n';
    }



    public void Init()//��ʼ��
    {
        Debug.log("\t\t��ʼ��ing\t\t");
        //��ע��С
        betMoney = 10;
        //��ʼǮ��
        initMoney = 2000;
        //�ܾ���
        setRound = 20;
        //��ע����?
        //��ʼ��gamestate
        currentState = GameState.init;
        curRound = 0;
        buttonNum = 1;
        betcount = 3��
        //��UI����ȡ��������б�δ��ɣ�
        namelist.Add("001");
        namelist.Add("002");
        namelist.Add("003");
        namelist.Add("004");
        namelist.Add("005");
        namelist.Add("006");
        //ȷ������б�
        PlayerManager.instance.IniPlayers(namelist);
        //�μӵ���ң���Ŵ�1��ʼ
        int tmpNo = 1;
        foreach (Player p in PlayerManager.instance.allPlayers)
        {
            //���Լ�һ���治����жϣ���
            p.isInGame = true;
            p.coin = initMoney;
        }


    }
    public void SetGap(float ratio)
    {
        gaptime = (float(1.01) - ratio) * gapset;
    }
    public void RoundInit()//��ʼ��
    {

        Debug.Log("\t\t\t\t");
        currentState = GameState.roundinit;
        cardList.Clear();
        started = true;

        //ȷ�����ڳ��ϵ����
        PlayerManager.instance.SeatPlayers();
        //ˢ��fold״̬,�����ע��
        PlayerManager.instance.NewRound();
        /*foreach (Player p in PlayerManager.instance.seatedPlayers)
        {
            p.isFold = false;
            p.betMoney = 0;
        }
        PlayerManager.instance.ActivePlayer();//undone
        */
        //ȷ��ׯ��
        // Newround ��������ⲿ�ֹ���

    }
    public void Preflop()//��ʼ��
    {
        int betRound = 0;
        int betp = 0;
        currentState = GameState.preflop;
        //����
        CardManager.instance.AssignCardsToPlayers(playManager.instance.activePlayers);

        //����player��betmoney
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            if (p.PlayeRole == Player.PlayeRole.bigBlind)
                p.betMoney = 2 * betMoney;
            else if (p.PlayeRole == Player.PlayeRole.smallBlind)
                p.betMoney = betMoney;
        }
        //���ĶĽ��
        betTotal = 3 * betMoney;
        betRound = 2 * betMoney;
        //AI����,�˴�����ģ��
        bool first = false;
        foreach (Player p in PlayerManager.instance.activePlayers)
        {
            if (first)
            {
                int m = Random.Range(0, 3);
                betp = p.move(m, betRound);
            }

            if (p.PlayeRole == Player.PlayeRole.bigBlind)
                first = true;
        }
        //
        PlayerManager.instance.ActivePlayer();//undone
    }
    public void Flop()//��ʼ��
    {
        currentState = GameState.flop;
        CardManager.instance.AssignCardsToTable(1);
        //AI����
        //

        PlayerManager.instance.ActivePlayer();
    }
    public void Turn()//��ʼ��
    {
        currentState = GameState.turn;
        CardManager.instance.AssignCardsToTable(1);
        //AI����
        //

        PlayerManager.instance.ActivePlayer();
    }
    public void River()//��ʼ��
    {
        currentState = GameState.river;
        CardManager.instance.AssignCardsToTable(1);
        //AI����
        //

        PlayerManager.instance.ActivePlayer();
    }
    public void Calc()//��ʼ��
    {
        currentState = GameState.calc;
        //AI����
        //
        winPlayers = CardManager.instance.FindWinner(PlayerManager.instance.activePlayers);
        foreach (Player p in winPlayers)
        {
            p += betTotal / winPlayers.Count;
        }
        foreach (Player p in seatedPlayers)
        {
            p -= p.betMoney;
        }
        
    }
    public void GameOver()//
    {
        currentState = GameState.gameover;
        started = false;
        
    }
    // Start is called before the first frame update
    void Begin()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (started){
            if (time < gaptime)
                time += Time.deltaTime;
            else
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
                            GameOver();
                            break;
                        }
                        RoundInit();
                        break;
                }
                // throw new UnityException("GameState Error" + currentState)
                time = 0;
            }
        }
    }
}
