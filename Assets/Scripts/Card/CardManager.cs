using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    //����ģʽ
    public static CardManager instance;
    //����
    private List<Card> cards = new List<Card>();

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    /// <summary>
    /// Initialize the card List of 4*13 Cards
    /// </summary>
    public void InitialCardsList()
    {
        cards.Clear();
        GlobalVar.publicCards = new List<Card>();
        for (int i = 2; i < 15; i++)
        {
            cards.Add(new Card(CardSuit.club, i));
            cards.Add(new Card(CardSuit.diamon, i));
            cards.Add(new Card(CardSuit.heart, i));
            cards.Add(new Card(CardSuit.spade, i));
        }
    }

    /// <summary>
    /// ��preflop�׶θ���ǰ����Ϸ�е����һ�˷������� ��������ѿɼ�
    /// </summary>
    public void AssignCardsToPlayers()
    {
        foreach(Player p in PlayerManager.instance.activePlayers)
        {
            p.AddPlayerCards(AssignRandomCard(), AssignRandomCard());
            p.playerObject.ShowCards();
        }
    }

    /// <summary>
    /// ���������ط���
    /// </summary>
    /// <param name="num">�ڹ������صķ�������</param>
    public void AssignCardsToTable(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GlobalVar.publicCards.Add(AssignRandomCard());
        }
    }

    /// <summary>
    /// �����ʣ���ƶ���ѡ��һ����
    /// </summary>
    /// <returns>��ʣ���ƶ��г������</returns>
    public Card AssignRandomCard()
    {
        if (cards.Count <= 0)
        {
            return null;
        }
        int rndInt = new System.Random().Next(0, cards.Count);
        Card rndCard = cards[rndInt];
        cards.Remove(rndCard);
        return rndCard;
    }


    /// <summary>
    /// �����շ��Ƶ�ȫ�������Ѱ�ҹھ���������һ������ƽ��
    /// </summary>
    /// <param name="pList">�����е�ʣ�����LIST</param>
    /// <returns>��������/���ֹھ����LIST</returns>
    public List<Player> FindWinner(List<Player> pList)
    {
        if (pList.Count == 0)
        {
            return new List<Player>();
        }
        List<Player> wList = new List<Player>();
        wList.Add(pList[0]);
        for (int i=1; i<pList.Count; i++)
        {
            List<Card> c1 = wList[0].finalCards;
            List<Card> c2 = pList[i].finalCards;
            c1.Sort((a,b)=>{
                if(a.Value != b.Value)
                {
                    return a.Value - b.Value;
                }
                return 0;
            });
            c2.Sort((a, b) => {
                if (a.Value != b.Value)
                {
                    return a.Value - b.Value;
                }
                return 0;
            });
            int result = CompareCards(c1,c2);
            if (result > 0)
            {
                wList.Clear();
                wList.Add(pList[i]);
            } else if (result == 0)
            {
                wList.Add(pList[i]);
            }
        }
        return wList;
    }


    /// <summary>
    /// �Ƚ������Ƶ�����
    /// </summary>
    /// <param name="c1">��һ���������</param>
    /// <param name="c2">�ڶ����������</param>
    /// <returns>���c1����return -1, c2����return 1, �����ͬ��return 0</returns>
    public int CompareCards(List<Card> c1, List<Card> c2)
    {
        int result1 = MatchCase(c1);
        int result2 = MatchCase(c2);
        if (result1 > result2)
        {
            return -1;
        }
        else if (result1 < result2)
        {
            return 1;
        }
        else
        {
            int value1 = totalCardValue(c1);
            int value2 = totalCardValue(c2);
            if (value1 > value2)
            {
                return -1;
            }
            else if (value1 < value2)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// �Ƿ�Ϊ˳��
    /// </summary>
    /// <param name="cList"></param>
    /// <returns></returns>
    public bool IsStright(List<Card> cList)
    {
        for(int i=1; i<cList.Count; i++)
        {
            if (cList[i].Value - cList[i-1].Value != 1)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ������ͬ�������м���
    /// </summary>
    /// <param name="cList"></param>
    /// <returns></returns>
    public int MaxSameNum(List<Card> cList)
    {
        int max = 1;
        int curMax = 1;
        for (int i=1; i<cList.Count; i++)
        {
            if (cList[i].Value == cList[i-1].Value)
            {
                curMax++;
                if (curMax > max)
                {
                    max = curMax;
                }
            } else
            {
                curMax = 1;
            }
        }
        return max;
    }

    /// <summary>
    /// �Ƿ�Ϊͬ��
    /// </summary>
    /// <param name="cList"></param>
    /// <returns></returns>
    public bool IsFlush(List<Card> cList)
    {
        CardSuit curS = cList[0].cardSuit;
        for(int i = 1; i<cList.Count; i++)
        {
            if (curS != cList[i].cardSuit)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ��;��ֵ���˼���
    /// </summary>
    /// <param name="cList"></param>
    /// <returns></returns>
    public int ValueChangeNum(List<Card> cList)
    {
        int valueChange = 0;
        for (int i = 1; i < cList.Count; i++)
        {
            if (cList[i].Value != cList[i - 1].Value)
            {
                valueChange++;
            }
        }
        return valueChange;
    }

    /// <summary>
    /// ������ֵ
    /// </summary>
    /// <param name="cList"></param>
    /// <returns></returns>
    public int totalCardValue(List<Card> cList)
    {
        int totalValue = 0;
        foreach (Card c in cList)
        {
            totalValue += c.Value;
        }
        return totalValue;
    }

    //0������
    //1��һ��
    //2������
    //3������
    //4��˳��
    //5��ͬ��
    //6����«
    //7������
    //8��ͬ��˳
    //9���ʼ�ͬ��˳
    public int MatchCase(List<Card> cList)
    {
        if (IsStright(cList) && IsFlush(cList))
        {
            if(cList[0].Value == 10)
            {
                return 9;
            } else
            {
                return 8;
            }
        } else if (MaxSameNum(cList) == 4)
        {
            return 7;
        } else if (MaxSameNum(cList) == 3 && ValueChangeNum(cList) == 1)
        {
            return 6;
        } else if (IsFlush(cList))
        {
            return 5;
        } else if (IsStright(cList)){
            return 4;
        } else if (MaxSameNum(cList)== 3)
        {
            return 3;
        } else if (MaxSameNum(cList) == 2)
        {
            if(ValueChangeNum(cList) == 2)
            {
                return 2;
            } else
            {
                return 1;
            }
        } else
        {
            return 0;
        }
    }

    public void FillUpTableCards()
    {
        int num = 5 - GlobalVar.publicCards.Count;
        if (num > 0)
        {
            AssignCardsToTable(num);
            for (int i = 0; i < num; i++)
            {
                UIManager.instance.ShowCommunityCard(GlobalVar.publicCards[i], i);
            }
        }
    }

    public bool IsEqual(Card a, Card b)
    {
        return (a.Value == b.Value && a.cardSuit == b.cardSuit); 
    }

    public bool IsValidSelection(Player p)
    {
        if (p.finalCards.Count != 5)
        {
            return false;
        }
        List<Card> existed = new List<Card>();
        foreach (Card c in p.finalCards)
        {
            if (!FindDuplicate(existed, c) && (FindDuplicate(GlobalVar.publicCards, c) || FindDuplicate(p.playerCardList, c)))
            {
                existed.Add(c);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public bool FindDuplicate(List<Card> cList, Card c)
    {
        foreach (Card c1 in cList)
        {
            if (CardManager.instance.IsEqual(c1, c))
            {
                return true;
            }
        }
        return false;
    }


    public void Restart()
    {
        if (GlobalVar.publicCards == null)
            GlobalVar.publicCards = new List<Card>();
        else
            GlobalVar.publicCards.Clear();
        InitialCardsList();
    }
}
