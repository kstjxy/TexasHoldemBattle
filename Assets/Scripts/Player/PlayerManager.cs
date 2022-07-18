using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    private static PlayerManager _instance;
    public static PlayerManager instance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerManager();
            return _instance;
        }
    }

    public List<Player> allPlayers = new List<Player>();        //������ҵ��б�
    public List<Player> seatedPlayers = new List<Player>();     //��ѡ����������б�����FOLD���Ѿ��Ʋ���
    public List<Player> activePlayers = new List<Player>();     //��ǰ���ڱ�����Ϸ�е�����б�
    public List<Player> okPlayers = new List<Player>();         //�����������������
    public int totalSeatNum;    //��ǰ�غ�һ��������������������Ѿ�FOLD�Ĳ������Ѿ��Ʋ���
    public int nowPlayerIndex;  //��ǰ��ע������

    /// <summary>
    /// ��ʼ���������
    /// </summary>
    /// <param name="nameList">��ҵ������б�</param>
    public void InitPlayers(List<string> nameList)
    {
        foreach (string name in nameList)
        {
            Player p = new Player(name);
            allPlayers.Add(p);
        }
    }

    /// <summary>
    /// �ж�ѡ����ҵ������Ƿ���Ϲ淶���Ҹ�����λ��
    /// </summary>
    /// <returns>������������Ҫ����RETURN TRUE��������һ����Ϸ�׶�,������FALSE</returns>
    public bool SeatPlayers()
    {
        seatedPlayers = new List<Player>();
        activePlayers = new List<Player>();
        totalSeatNum = 0;
        foreach (Player p in allPlayers)
        {
            if (p.isInGame)
            {
                seatedPlayers.Add(p);
                p.seatNum = totalSeatNum;
                totalSeatNum++;
            }
        }
        if (totalSeatNum < 2 || totalSeatNum > 8)
        {
            if (totalSeatNum < 2)
            {
                Debug.Log("�������δ�ﵽ2�ˣ������ѡ��");
                InitialPanelManager.instance.CallStartErrorLog("�����������С��2�ˣ�");
            } else
            {
                Debug.Log("���������������8�ˣ������ѡ��");
                InitialPanelManager.instance.CallStartErrorLog("����������ܳ���8�ˣ�");
            }
            foreach (Player pl in seatedPlayers)
            {
                pl.seatNum = -1;
                pl.isInGame = false;
            }
            seatedPlayers.Clear();
            InitialPanelManager.instance.ResetAllTheButtons();
            return false;
        }
        return true;
    }

    /// <summary>
    /// ÿһ������Ϸ��ʼǰ�ж���ҵĲƸ�ֵ�޸�ACTIVE PLAYERS
    /// </summary>
    public void NewRound()
    {
        foreach (Player p in seatedPlayers)
        {
            if (p.coin <= 2*GolbalVar.minBetCoin)
            {
                p.OutOfGame();
                totalSeatNum--;
                activePlayers.Remove(p);
            } else
            {
                p.ResetNewRound();
                if (!activePlayers.Contains(p))
                {
                    activePlayers.Add(p);
                }
            }
        }
        GolbalVar.curBtnSeat = (GolbalVar.curBtnSeat + 1) % totalSeatNum;
        SetPlayersRole(GolbalVar.curBtnSeat);
    }

    /// <summary>
    /// �������ڵ�����Ϸ�е��������ROLE
    /// </summary>
    /// <param name="btn">ׯ�ҵ���λ��</param>
    public void SetPlayersRole(int btn)
    {
        activePlayers.Sort((a, b) => {
            return a.seatNum - b.seatNum;
        });
        
        foreach (Player p in activePlayers)
        {
            p.role = Player.PlayerRole.normal;
        }

        if (activePlayers.Count >= 3)
        {
            activePlayers[btn].role = Player.PlayerRole.button;
            activePlayers[(btn + 1) % totalSeatNum].role = Player.PlayerRole.smallBlind;
            activePlayers[(btn + 2) % totalSeatNum].role = Player.PlayerRole.bigBlind;
        } else
        {
            activePlayers[(btn) % totalSeatNum].role = Player.PlayerRole.smallBlind;
            activePlayers[(btn + 1) % totalSeatNum].role = Player.PlayerRole.bigBlind;
        }
        

        SortPlayers();
    }
    public bool ActivePlayers()
    {
        activePlayers = new List<Player>();
        totalSeatNum = 0;
        foreach (Player p in seatedPlayers)
        {
            if (!p.isFold)
            {
                activePlayers.Add(p);
            }
        }
        return true;
    }

    /// <summary>
    /// ͨ��PLAY ROLE���޸Ļ�Ծ��ҵ��ж�˳��Сä��ä����ǰ�棬ׯ�������
    /// </summary>
    public void SortPlayers()
    {
        for(int i = 0; i<GolbalVar.curBtnSeat; i++)
        {
            Player p = activePlayers[i];
            activePlayers.Remove(p);
            activePlayers.Add(p);
        }
    }
    public Player FindPlayer(int seatNum)
    {
        foreach(Player p in seatedPlayers)
        {
            if (p.seatNum == seatNum)
            {
                return p;
            }
        }
        return null;
    }
    public int CalcFoldNum(List<Player> pList)
    {
        int num = 0;
        foreach(Player p in pList)
        {
            if (p.isFold == true)
                num++;
        }
        return num;
    }

    //��ҵľ������
    //-1 ��ʼ����äСä
    // 1 ��ע �� ����
    // 2 ��ע
    // 3 ����
    // 4 ALL IN
    public void BetAction(Player p)
    {
        string strbet;
        switch (p.state)
        {
            case 0://preflolp�׶δ���Сä����ä
                {
                    //Сäע
                    if (p.role == Player.PlayerRole.smallBlind)
                    {
                        p.betCoin = GolbalVar.minBetCoin;
                        p.coin -= p.betCoin;
                        GolbalVar.maxBetCoin = p.betCoin;
                        GolbalVar.pot += p.betCoin;
                        strbet = p.playerName + "ѡ��Сäע����ʣ����Ϊ" + p.coin + "����ǰ���ѺעΪ" + GolbalVar.maxBetCoin + "����ǰ�׳صĽ��Ϊ" + GolbalVar.pot;
                        Debug.Log(strbet);
                        UIManager.instance.PrintLog(strbet);
                    }
                    else if (p.role == Player.PlayerRole.bigBlind)
                    {
                        p.betCoin = 2 * GolbalVar.minBetCoin;
                        p.coin -= p.betCoin;
                        GolbalVar.maxBetCoin = p.betCoin;
                        strbet = p.playerName + "ѡ�񡾴�äע����ʣ����Ϊ" + p.coin + "����ǰ���ѺעΪ" + GolbalVar.maxBetCoin + "����ǰ�׳صĽ��Ϊ" + GolbalVar.pot;
                        Debug.Log(strbet);
                        UIManager.instance.PrintLog(strbet);
                    }
                    break;
                }
                
            case 1://��ע
                {
                    //ʣ�����ܷ��ע
                    if (p.coin + p.betCoin >= GolbalVar.maxBetCoin) //��Ǯ
                    {
                        if (p.betCoin == GolbalVar.maxBetCoin)  //�Ѿ��Ǹ�ע�� ��Ϊ����
                        {
                            strbet = p.playerName + "ѡ�񡾹��ơ�����ǰʣ����Ϊ" + p.coin + "����ǰ���ѺעΪ" + GolbalVar.maxBetCoin + "����ǰ�׳صĽ��Ϊ" + GolbalVar.pot;
                        }
                        else //��ע
                        {
                            p.coin -= GolbalVar.maxBetCoin - p.betCoin;
                            GolbalVar.pot += GolbalVar.maxBetCoin - p.betCoin;
                            p.betCoin = GolbalVar.maxBetCoin;
                            strbet = p.playerName + "ѡ�񡾸�ע������ǰʣ����Ϊ" + p.coin + "����ǰ���ѺעΪ" + GolbalVar.maxBetCoin + "����ǰ�׳صĽ��Ϊ" + GolbalVar.pot;

                        }

                        Debug.Log(strbet);
                        UIManager.instance.PrintLog(strbet);
                    }
                    else //Ǯ���� ��Ҫ��ע ��ʱΪallin
                    {
                        p.state = 4;
                        BetAction(p);
                    }
                    break;
                }
                
            case 2://��ע
                {
                    //�жϲ����Ƿ�Ϸ�
                    if (GolbalVar.curBetCount > GolbalVar.maxBetCount)
                    {
                        strbet = "���غϼ�ע�����Ѵ�" + GolbalVar.maxBetCount + "�Σ�" + p.playerName + "���β����Ƿ���Ĭ��Ϊ��ע";
                        p.state = 1;
                        BetAction(p);
                    }
                    else
                    {
                        //�ж�Ǯ������
                        if (p.coin + p.betCoin - GolbalVar.maxBetCoin + 2 * GolbalVar.minBetCoin > 0)
                        {
                            p.coin -= 2 * GolbalVar.minBetCoin;
                            p.betCoin += 2 * GolbalVar.minBetCoin;
                            GolbalVar.pot += 2 * GolbalVar.minBetCoin;
                            strbet = p.playerName + "ѡ�񡾼�ע������ǰʣ����Ϊ" + p.coin + "����ǰ���ѺעΪ" + GolbalVar.maxBetCoin + "����ǰ�׳صĽ��Ϊ" + GolbalVar.pot;
                            Debug.Log(strbet);
                            UIManager.instance.PrintLog(strbet);
                        }
                        else
                        {
                            p.state = 4;
                            BetAction(p);
                        }
                    }
                    break;
                }
                
            case 3://����
                {
                    p.isFold = true;
                    strbet = p.playerName + "ѡ�����ơ�����ǰʣ����Ϊ" + p.coin + "����ǰ���ѺעΪ" + GolbalVar.maxBetCoin + "����ǰ�׳صĽ��Ϊ" + GolbalVar.pot;
                    Debug.Log(strbet);
                    UIManager.instance.PrintLog(strbet);
                    break;
                }
               
            case 4://ALL IN
                {
                    p.betCoin = p.betCoin + p.coin;
                    p.isAllIn = true;                    
                    GolbalVar.pot += p.coin;
                    p.coin = 0;
                    strbet = p.playerName + "ѡ��ALL IN������ǰʣ����Ϊ" + p.coin;
                    if (p.betCoin < GolbalVar.maxBetCoin)
                    {
                        strbet += "���ڵ�ǰ��ҵ�ALL IN��ע���" + p.betCoin + "С�ڵ�ǰ���Ѻע�����Ѻע����";
                    }
                    else
                    {
                        GolbalVar.maxBetCoin = p.betCoin;  
                    }
                    strbet += "����ǰ���ѺעΪ" + GolbalVar.maxBetCoin + "����ǰ�׳صĽ��Ϊ" + GolbalVar.pot;
                    Debug.Log(strbet);
                    UIManager.instance.PrintLog(strbet);

                    break;
                }                
        }
    }
    
    //���ڲ��Ե�����ж�
    public int RandomAction()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        int ranNum = Random.Range(1, 101);
        //60% ��ע �� ��
        if (ranNum <= 60) return 1;
        //15% ��ע
        if (ranNum <= 75) return 2;
        //20% ����
        if (ranNum <= 95) return 3;
        //5% ALL IN
        return 4;

    }
    public void Bet(Player p)
    {
        if (p.state == -1)
            p.state = 0;
        else
            p.state = RandomAction();
        BetAction(p);
    }
    //����ֵ˵��
    //-1    
    //0     ��ʣһ����ң���Ϸ����
    //1
    public int PlayerBet(List<Player> pList, int playerIndex)
    {
        string strbet;
        if (pList[playerIndex].isFold == true || pList[playerIndex].isAllIn == true)
        {
            strbet = pList[playerIndex].playerName + "�Ѿ�����/ALL IN����������";
            return -1;
        }


        if (nowPlayerIndex == pList.Count)
            nowPlayerIndex = 0;
        if (nowPlayerIndex == 0)
        {
            Debug.Log("��һ����ע��ʼ");
            UIManager.instance.PrintLog("��һ����ע��ʼ");
        }

        if (nowPlayerIndex == playerIndex)
        {
            nowPlayerIndex++;
            if (CalcFoldNum(pList) == pList.Count - 1 && pList[playerIndex].isFold == false)
            {
                nowPlayerIndex = 0;
                strbet = "����" + pList[playerIndex].playerName + "��������Ҿ���Ȩ��\n��ǰ���ѺעΪ" + GolbalVar.maxBetCoin + "����ǰ�׳صĽ��Ϊ" + GolbalVar.pot;
                Debug.Log("����" + pList[playerIndex].playerName + "��������Ҿ���Ȩ");
                UIManager.instance.PrintLog("����" + pList[playerIndex].playerName + "��������Ҿ���Ȩ");
                return 0;
            }
            Bet(pList[playerIndex]);
        }
        //
        if (nowPlayerIndex >= pList.Count)
        {
            strbet = "�׳أ�" + GolbalVar.pot + "�����ע��" + GolbalVar.maxBetCoin;
            Debug.Log(strbet);
            UIManager.instance.PrintLog(strbet);
        }

        okPlayers.Clear();
        foreach(Player p in pList)
            if (p.isAllIn == true || p.betCoin == GolbalVar.maxBetCoin || p.isFold == true)
                okPlayers.Add(p);
        if(okPlayers.Count == pList.Count)
        {
            strbet = "��" + GolbalVar.curRoundNum + "�غϽ���";
            Debug.Log(strbet);
            UIManager.instance.PrintLog(strbet);
            nowPlayerIndex = 0;
        }

        return 1;
    }

    
}
