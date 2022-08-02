using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player 
{
    public enum PlayerRole
    {
        button,         //ׯ��
        bigBlind,       //��ä
        smallBlind,     //Сä
        normal,         //��ͨ���
        outOfGame       //���ڴ�����Ϸ��
    }

    public WebAI ai;
    public string playerName;
    public int seatNum = -1; //������ڴ�����Ϸ�У���Ϊ-1
    public int coin = 0;
    public int betCoin = 0;
    public bool isInGame = false;
    public bool isFold = false;
    public bool isAllIn = false;
    public PlayerRole role = PlayerRole.outOfGame;
    public List<Card> playerCardList = new List<Card>();//��������
    public int state = 0; //no action
    public List<Card> finalCards = new List<Card>();//������ɵ���������

    public List<int[]> last_period = new List<int[]>();//��һ��������Լ�λ��֮�����ҵ��ж���¼����¼Ϊ��������(seatNum)��������š���ע����¼���ά���������б�       ���Լ��ĻغϽ�������ո��б��Խ��ս���������Ϣ
    public List<int[]> this_period = new List<int[]>();//��һ���д�ׯ�ҵ�����Լ�λ�õ��ж���¼��ͬ�ϣ�      �ڱ��ֽ�������һ�ּ�����ʼʱ����ո��б��Խ��ս���������Ϣ

    public PlayerObject playerObject;


    public Player(WebAI ai)
    {
        this.ai = ai;
        this.playerName = ai.name;
        //�ڽ��г�ʼ����ʱ���Ҫע����¼���ȥ��������ֻҪ����RecordManager�е�ActionRecord�Ϳ�����������ҽ��յ���Ϣ��
        RecordManager.instance.ActionRecords += AddActionRecord;
    }

    public Player(LuaAI ai)
    {
        this.ai = ai;
        this.playerName = ai.name;
        //�ڽ��г�ʼ����ʱ���Ҫע����¼���ȥ��������ֻҪ����RecordManager�е�ActionRecord�Ϳ�����������ҽ��յ���Ϣ��
        RecordManager.instance.ActionRecords += AddActionRecord;
    }
    /// <summary>
    /// ����ҵĸ��˿���������ſ���
    /// </summary>
    /// <param name="a">��һ����</param>
    /// <param name="b">�ڶ�����</param>
    public void AddPlayerCards(Card a, Card b)
    {
        playerCardList = new List<Card>();
        playerCardList.Add(a);
        playerCardList.Add(b);
    }

    /// <summary>
    /// ������Ƴ���Ϸ
    /// </summary>
    public void OutOfGame()
    {
        isInGame = false;
        coin = 0;
        role = PlayerRole.outOfGame;
        ResetNewRound();
    }

    /// <summary>
    /// ��һ����Ϸ�������
    /// </summary>
    public void ResetNewRound()
    {
        isFold = false;
        isAllIn = false;
        state = 0;
        betCoin = 0;
        playerCardList = new List<Card>();
        finalCards = new List<Card>();
    }

    /// <summary>
    /// ������ж��ˣ�������ҿ�ʼ���Լ�¼���߶Լ�¼����в���(�����������ж�����֮�󣡣��ٵ���)
    /// </summary>
    /// <param name="playerSeatNum">��λ��</param>
    /// <param name="actionNum">�ж�</param>
    public void AddActionRecord(int playerSeatNum, int actionNum)
    {
        //��ȡ���ж�˳����е���������ȷ��¼��Ϣ
        int thisPlayerIndex = -1;
        int actionPlayerIndex = -1;
        for (int i = 0; i < PlayerManager.instance.activePlayers.Count; i++)
        {
            if (PlayerManager.instance.activePlayers[i].seatNum == playerSeatNum)
                actionPlayerIndex = i;
            if (PlayerManager.instance.activePlayers[i].seatNum == seatNum)
                thisPlayerIndex = i;
            if (actionPlayerIndex >= 0 && thisPlayerIndex >= 0)
                break;
        }
        if (actionPlayerIndex < 0 || actionPlayerIndex >= 8 || thisPlayerIndex < 0)
        {
            Debug.Log("SeatNum Data Error!!   a:"+actionPlayerIndex+"   t:"+thisPlayerIndex);
            return;
        }

        //�������ж�˳����е�������к��ʵ���Ϣ��¼
        if (thisPlayerIndex == actionPlayerIndex)//������Լ����ж�������Ҫ��¼���������last_periodΪ������������Ϣ��׼��
        {
            last_period.Clear();
        }
        else if (actionPlayerIndex < thisPlayerIndex)//���Լ�֮ǰ����ҽ����ж������� this_period
        {
            //Debug.Log(this.playerName+"  PlayerNum: " + playerSeatNum + "   ActionNum: " + actionNum+"    this");
            this.this_period.Add(new int[2] { playerSeatNum, actionNum });
        }
        else//���Լ�֮�����ҽ����ж������� last_period
        {
            //Debug.Log(this.playerName+"  PlayerNum: " + playerSeatNum + "   ActionNum: " + actionNum + "    last");
            this.last_period.Add(new int[2] { playerSeatNum, actionNum });
        }

        //�������һλ������ж���Ҫ��� this_period 
        if (actionPlayerIndex == PlayerManager.instance.activePlayers.Count - 1)
        {
            this_period.Clear();
        }
    }
}
