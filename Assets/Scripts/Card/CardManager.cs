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
            List<int[]> c1 = wList[0].finalCards;
            List<int[]> c2 = pList[i].finalCards;
            c1.Sort((a,b)=>{
                return a[1] - b[1];
            });
            c2.Sort((a, b) => {
                return a[1] - b[1];
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
    public int CompareCards(List<int[]> c1, List<int[]> c2)
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
    public bool IsStright(List<int[]> cList)
    {
        for(int i=1; i<cList.Count; i++)
        {
            if (cList[i][1] - cList[i-1][1] != 1)
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
    public int MaxSameNum(List<int[]> cList)
    {
        int max = 1;
        int curMax = 1;
        for (int i=1; i<cList.Count; i++)
        {
            if (cList[i][1] == cList[i-1][1])
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
    public bool IsFlush(List<int[]> cList)
    {
        int curS = cList[0][0];
        for(int i = 1; i<cList.Count; i++)
        {
            if (curS != cList[i][0])
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
    public int ValueChangeNum(List<int[]> cList)
    {
        int valueChange = 0;
        for (int i = 1; i < cList.Count; i++)
        {
            if (cList[i][1] != cList[i - 1][1])
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
    public int totalCardValue(List<int[]> cList)
    {
        int totalValue = 0;
        foreach (int[] c in cList)
        {
            totalValue += c[1];
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
    public int MatchCase(List<int[]> cList)
    {
        if (IsStright(cList) && IsFlush(cList))
        {
            if(cList[0][1] == 10)
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

    public List<int[]> GenListVals(List<Card> cList)
    {
        List<int[]> result = new List<int[]>();
        foreach (Card c in cList)
        {
            result.Add(c.listVal);
        }
        return result;
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

    public void Restart()
    {
        if (GlobalVar.publicCards == null)
            GlobalVar.publicCards = new List<Card>();
        else
            GlobalVar.publicCards.Clear();
        InitialCardsList();
    }
}
