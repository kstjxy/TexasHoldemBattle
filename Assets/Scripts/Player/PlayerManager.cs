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
    public int totalSeatNum;    //��ǰ�غ�һ��������������������Ѿ�FOLD�Ĳ������Ѿ��Ʋ���

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
                InitialPanelManager.instance.CallStartErrorLog("<2,886");
            } else
            {
                Debug.Log("���������������8�ˣ������ѡ��");
                InitialPanelManager.instance.CallStartErrorLog(">8,886");
            }
            foreach (Player pl in seatedPlayers)
            {
                pl.seatNum = -1;
                pl.isInGame = false;
            }
            seatedPlayers.Clear();
            return false;
        }
        return true;
    }

    public void NewRound()
    {
        foreach (Player p in seatedPlayers)
        {
            if (p.coin <= 0)
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

    public void SetPlayersRole(int btn)
    {
        activePlayers.Sort((a, b) => {
            return a.seatNum - b.seatNum;
        });
        
        foreach (Player p in activePlayers)
        {
            p.role = Player.PlayerRole.normal;
        }

        activePlayers[btn].role = Player.PlayerRole.button;
        activePlayers[(btn + 1) % totalSeatNum].role = Player.PlayerRole.smallBlind;
        if (totalSeatNum >= 3)
        {
            activePlayers[(btn + 2) % totalSeatNum].role = Player.PlayerRole.bigBlind;
        }

        SortPlayers();
    }

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
}
