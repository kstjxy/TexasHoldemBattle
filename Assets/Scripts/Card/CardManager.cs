using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    //����ģʽ
    public static CardManager instance;
    //����
    private List<Card> cards;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        InitialCardsList();
    }

    //�����ƶ� ��
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

    //����
    public void DealCards()
    { 
        //����GameManager��״̬�����жϷ�����ʲô��
    }
}
