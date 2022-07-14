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

    public string playerName;
    public int seatNum = -1; //������ڴ�����Ϸ�У���Ϊ-1
    public int coin = 0; 
    public bool isFold = false;
    public bool isAllIn = false;
    public PlayerRole role = PlayerRole.outOfGame;
    public List<Card> playerCardList = new List<Card>();
    public int state = -1; //no action
    public List<Card> finalCards = new List<Card>(); 

    public Player(string playerName)
    {
        this.playerName = playerName;
    }

    public void AddPlayerCards(Card a, Card b)
    {
        playerCardList = new List<Card>();
        playerCardList.Add(a);
        playerCardList.Add(b);
    }
}
