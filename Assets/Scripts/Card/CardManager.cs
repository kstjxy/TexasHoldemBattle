using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    //单例模式
    public static CardManager instance;
    //牌组
    private List<Card> cards;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        InitialCardsList();
    }

    //加载牌堆 在
    private void InitialCardsList()
    {
        cards.Clear();
        for (int i = 2; i < 15; i++)
        {
            cards.Add(new Card(CardSuit.club, i));
            cards.Add(new Card(CardSuit.diamon, i));
            cards.Add(new Card(CardSuit.heart, i));
            cards.Add(new Card(CardSuit.spade, i));
        }
    }

    //发牌
    public void DealCards()
    { 
        //根据GameManager的状态机来判断发的是什么牌
    }
}
