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
        setting,    //���ý��棬������Ϸ����
        init,       //��ʼ�����л����棬�������
        roundInit,  //ÿһ�ֿ�ʼ��INIT��ȷ��PLAYER ROLES
        preflop,    //������������� ��һ����ע
        flop,       //�������������� �ڶ�����ע
        turn,       //�������ص������� ��������ע
        river,      //�������ص������� ��������ע
        result,     //һ����Ϸ������ʾ������������������
        gameover    //�趨�����оֽ���
    }

    public static float timer = 0;
    
    public static GameState GameStatus()
    {
        switch (GolbalVar.gameStatusCounter)
        {
            case -2:
                return GameState.setting;
            case -1:
                return GameState.init;
            case 0:
                return GameState.roundInit;
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

    public void GameUpdate()
    {
        switch(GolbalVar.gameStatusCounter)
        {
            case -1:
                Init();
                break;
            case 0:
                RoundInit();
                break;
            case 1:
                Preflop();
                break;
            case 2:
                Flop();
                break;
            case 3:
                Turn();
                break;
            case 4:
                River();
                break;
            case 5:
                Result();
                break;
            case 6:
                GameOver();
                break;
        }
    }

    public void Setting()
    {
        if (PlayerManager.instance.SeatPlayers())
        {
            GolbalVar.gameStatusCounter++;
        }
    }

    public void Restart()
    {
        GolbalVar.gameStatusCounter = -2;
    }

    public void Init()
    {
        foreach (Player p in PlayerManager.instance.seatedPlayers)
        {
            p.coin = GolbalVar.initCoin;
        }
        
    }

    public void RoundInit()
    {

        PlayerManager.instance.NewRound();
        PlayerManager.instance.SetPlayersRole(GolbalVar.curBtnSeat);
        CardManager.instance.InitialCardsList();
    }

    public void Preflop()
    {
        UIManager.instance.PrintLog("��ǰΪ��ǰ����Ȧ��");
        CardManager.instance.AssignCardsToPlayers();
        UIManager.instance.PrintLog("ÿ������Ϸ�е���һ����������");
    }

    public void Flop()
    {
        UIManager.instance.PrintLog("��ǰΪ������Ȧ��");
        CardManager.instance.AssignCardsToTable(3);
        for(int i=0; i<3; i++)
        {
            UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[i], i);
        }
        UIManager.instance.PrintLog("�������ط���ǰ������");
    }
    public void Turn()
    {
        UIManager.instance.PrintLog("��ǰΪ��ת��Ȧ��");
        CardManager.instance.AssignCardsToTable(1);
        UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[3], 3);
        UIManager.instance.PrintLog("�������ط�����������");
    }

    public void River()
    {
        UIManager.instance.PrintLog("��ǰΪ������Ȧ��");
        CardManager.instance.AssignCardsToTable(1);
        UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[4], 4);
        UIManager.instance.PrintLog("�������ط������һ����");
    }

    public void Result()
    {

    }

    public void GameOver()
    {

    }

    
    public void Start()
    {
        Debug.Log("��Ϸ��ʼ......");
 //       PlayerManager.instance.InitPlayers();
        GolbalVar.gameStatusCounter = -2;
    }

    // Update is called once per frame
    public void Update()
    {   
        if (GolbalVar.curRoundNum > GolbalVar.totalRoundNum)
        {
            GameOver();
        }
        timer += Time.deltaTime;
        if (timer > 2 * GolbalVar.speedFactor)
        {
            GameUpdate();
            if (GolbalVar.gameStatusCounter>-2 && GolbalVar.gameStatusCounter <5)
            {
                GolbalVar.gameStatusCounter++;
            }
            timer = 0;
        }
    }
}
