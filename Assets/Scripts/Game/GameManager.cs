using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
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
    public int curPlayerSeat = 0;
    public Player curPlayer = null;
    public bool playersInAction = false;
    public List<Player> winners = new List<Player>();
    
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
        GolbalVar.curRoundNum = 0;
    }

    public void Init()
    {
        foreach (Player p in PlayerManager.instance.seatedPlayers)
        {
            p.coin = GolbalVar.initCoin;
        }
        UIManager.instance.UpdateRankingList();
        GolbalVar.gameStatusCounter++;
    }

    public void RoundInit()
    {
        GolbalVar.curRoundNum++;
        UIManager.instance.PrintLog("��һ����Ϸ��ʼ����ǰΪ�ڡ�" + GolbalVar.curRoundNum +"����");
        PlayerManager.instance.NewRound();
        //PlayerManager.instance.SetPlayersRole(GolbalVar.curBtnSeat); NewRound() has done this;
        UIManager.instance.PrintLog("λ�÷�����ϣ�");
        if (PlayerManager.instance.activePlayers.Count >= 3)
        {
            UIManager.instance.PrintLog("��" + PlayerManager.instance.activePlayers[PlayerManager.instance.activePlayers.Count - 1].playerName + "��Ϊׯ��λ");
            UIManager.instance.PrintLog("��" + PlayerManager.instance.activePlayers[0].playerName + "��ΪСäλ");
        } else
        {
            UIManager.instance.PrintLog("��" + PlayerManager.instance.activePlayers[0].playerName + "��Ϊׯ�Һ�Сäλ");
        }
        UIManager.instance.PrintLog("��" + PlayerManager.instance.activePlayers[1].playerName + "��Ϊ��äλ");

        curPlayerSeat = 0;
        curPlayer = PlayerManager.instance.activePlayers[curPlayerSeat];
        CardManager.instance.InitialCardsList();
        GolbalVar.gameStatusCounter++;
    }

    public void Preflop()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("��ǰΪ��ǰ����Ȧ��");
                CardManager.instance.AssignCardsToPlayers();
                UIManager.instance.PrintLog("ÿ������Ϸ�е���һ����������");
                int sign = PlayerManager.instance.PlayerBet();
                if (sign == 0) GolbalVar.gameStatusCounter = 5;
            } else
            {
                ReadyForNextState();
            }
            
        } else
        {
            UpdateCurPlayer();
            UIManager.instance.PrintLog("��" + curPlayer.playerName + "��������Ϊ����" + curPlayer.playerCardList[0].PrintCard() + "����" + curPlayer.playerCardList[1].PrintCard() + "��");
        }
    }

    public void Flop()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("��ǰΪ������Ȧ��");
                CardManager.instance.AssignCardsToTable(3);
                for (int i = 0; i < 3; i++)
                {
                    UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[i], i);
                }
                UIManager.instance.PrintLog("�������ط���ǰ�����ƣ��ֱ�Ϊ��\n��" + GolbalVar.publicCards[0].PrintCard() + "����" +
                    GolbalVar.publicCards[1].PrintCard() + "����" + GolbalVar.publicCards[2].PrintCard() + "��");
                int sign = PlayerManager.instance.PlayerBet();
                if (sign == 0) GolbalVar.gameStatusCounter = 5;
            } else
            {
                ReadyForNextState();
            }
        } else
        {
            UpdateCurPlayer();
        }
    }
    public void Turn()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("��ǰΪ��ת��Ȧ��");
                CardManager.instance.AssignCardsToTable(1);
                UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[3], 3);
                UIManager.instance.PrintLog("�������ط����������ƣ�Ϊ��" + GolbalVar.publicCards[3].PrintCard() + "��");
                int sign = PlayerManager.instance.PlayerBet();
                if (sign == 0) GolbalVar.gameStatusCounter = 5;
            }
            else
            {
                ReadyForNextState();
            }
        }
        else
        {
            UpdateCurPlayer();
        }
    }

    public void River()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("��ǰΪ������Ȧ��");
                CardManager.instance.AssignCardsToTable(1);
                UIManager.instance.ShowCommunityCard(GolbalVar.publicCards[4], 4);
                UIManager.instance.PrintLog("�������ط������һ���ƣ�Ϊ��" + GolbalVar.publicCards[4].PrintCard() + "��"); ;
                int sign = PlayerManager.instance.PlayerBet();
                if (sign == 0) GolbalVar.gameStatusCounter = 5;
            }
            else
            {
                ReadyForNextState();
            }
        }
        else
        {
            UpdateCurPlayer();
        }
    }

    public void Result()
    {
        if (!playersInAction)
        {
            if (curPlayerSeat == 0)
            {
                playersInAction = true;
                UIManager.instance.PrintLog("������Ϸ���������ڽ������׶�");
            }
            else
            {
                ReadyForNextState();
                winners = CardManager.instance.FindWinner(PlayerManager.instance.activePlayers);
                UIManager.instance.PrintLog("���������������ѡ����ϣ�\n�ڳ�����������Ϊ��"+PrintWinner(winners));
            }
        }
        else
        {
            UpdateCurPlayer();
            curPlayer.finalCards = curPlayer.ai.FinalSelection();
            if (IsValidSelection(curPlayer))
            {
                UIManager.instance.PrintLog("��ҡ�" + curPlayer.playerName + "�����ѡ����������Ϊ��\n��" + curPlayer.finalCards[0].PrintCard() + "����" + curPlayer.finalCards[1].PrintCard() +
                "����" + curPlayer.finalCards[2].PrintCard() + "����" + curPlayer.finalCards[3].PrintCard() + "����" + curPlayer.finalCards[4].PrintCard() + "��");
            } else
            {
                UIManager.instance.PrintLog("��ҡ�" + curPlayer.playerName + "�����ѡ�����Ʋ����Ϲ淶,�޷�����ھ�����");
                PlayerManager.instance.activePlayers.Remove(curPlayer);
            }
        }
    }

    public void GameOver()
    {

    }

    /// <summary>
    /// �����ͨ��coin����ֵ��������
    /// </summary>
    /// <returns>ͨ��coin��С������������list</returns>
    public List<Player> GetRankedPlayers()
    {
        List<Player> pList = new List<Player>();
        foreach(Player p in PlayerManager.instance.seatedPlayers)
        {
            pList.Add(p);
        }
        pList.Sort((a, b) => {
            return b.coin - a.coin;
        });
        return pList;
    }
 
    /// <summary>
    /// ����ҽ�����������ͬ����coinӵ�����������
    /// </summary>
    /// <param name="pList">�Ѿ�������ϵ����list</param>
    /// <returns>��ҵ������б�</returns>
    public List<int> GetPlayerRank (List<Player> pList)
    {
        List<int> rankNum = new List<int>();
        int curRank = 1;
        int cumm = 0;
        int prevCumm = 0;
        rankNum.Add(curRank);
        for (int i = 1; i < pList.Count; i++)
        {
            if (pList[i - 1].coin != pList[i].coin)
            {
                curRank++;
                prevCumm = cumm;
                cumm = 0;
            }
            else
            {
                cumm++;
            }
            if (prevCumm != 0)
            {
                curRank += prevCumm;
                prevCumm = 0;
            }
            rankNum.Add(curRank);
        }
        return rankNum;
    }

    public void UpdateCurPlayer()
    {
        curPlayer.playerObject.BackToWaiting_AvatarChange();
        curPlayer = PlayerManager.instance.activePlayers[curPlayerSeat];
        curPlayerSeat++;
        curPlayer.playerObject.HightLightAction_AvatarChange();
        if (curPlayerSeat == PlayerManager.instance.activePlayers.Count)
        {
            playersInAction = false;
        }
    }

    public void ReadyForNextState()
    {
        curPlayer.playerObject.BackToWaiting_AvatarChange();
        if(GolbalVar.gameStatusCounter != 5)
        {
            curPlayerSeat = 0;
            GolbalVar.gameStatusCounter++;
        }
    }

    public bool IsValidSelection(Player p)
    {
        if (p.finalCards.Count != 5)
        {
            return false;
        }
        List<Card> existed = new List<Card>();
        foreach(Card c in p.finalCards)
        {
            if (!existed.Contains(c) && (GolbalVar.publicCards.Contains(c) || p.playerCardList.Contains(c)))
            {
                existed.Add(c);
            } else
            {
                return false;
            }
        }
        return true;
    }

    public string PrintWinner(List<Player> pList)
    {
        string str = "��" + pList[0].playerName;
        for (int i = 1; i<pList.Count; i++)
        {
            str = str + "����" + pList[i].playerName;
        }
        return str + "��";
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
            timer = 0;
        }
    }
}
