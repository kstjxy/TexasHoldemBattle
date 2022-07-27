using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStat
{
    //����ֻ���ڸ�AI�ṩ���ݶ�ȡ�Ľӿڣ�
    private Player thisPlayerInformation;
    public GameStat(Player p)
    {
        thisPlayerInformation = p;
    }

    //������Ϣ------------------------------------------------------------------------------
    //��Ϸ�׶�
    public int Round 
    {
        get { return GlobalVar.gameStatusCounter; }
    }
    //ׯ��λ
    public int Button
    {
        get { return GlobalVar.curBtnSeat; }
    }
    //������
    public List<int[]> CommunityCards
    {
        get { return CardManager.instance.GenListVals(GlobalVar.publicCards); }
    }
    //���л����ڳ��ϵ������Ϣ����λ�ţ����н�ң���������ע��ҡ�
    public List<int[]> PlayersInformation
    {
        get
        {
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < PlayerManager.instance.activePlayers.Count; i++)
            {
                int[] info = { PlayerManager.instance.activePlayers[i].seatNum, PlayerManager.instance.activePlayers[i].coin, PlayerManager.instance.activePlayers[i].betCoin };
                list.Add(info);
            }
            return list;
        }
    }

    //������Ϣ------------------------------------------------------------------------------
    //�Լ�����λ��
    public int MyPosition
    {
        get { return thisPlayerInformation.seatNum; }
    }
    //��һ�ִ���λ��֮����ж����С���λ�ţ��ж���š�
    public List<int[]> Last_Period
    {
        get { return thisPlayerInformation.last_period; }
    }
    //��һ�ִ�ׯ�ҵ��ҵ��ж����� ����λ�ţ��ж���š�
    public List<int[]> This_Period
    {
        get { return thisPlayerInformation.this_period; }
    }
    //�Լ�����������
    public List<int[]> CardsInHands
    {
        get { return CardManager.instance.GenListVals(thisPlayerInformation.playerCardList); }
    }
    //ʣ�����
    public int CoinsLeft
    {
        get { return thisPlayerInformation.coin; }
    }
    //�þ�����ע�ĳ���
    public int CoinsBet
    {
        get { return thisPlayerInformation.betCoin; }
    }
}
