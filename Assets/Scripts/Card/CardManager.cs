using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    //单例模式
    public static CardManager instance;
    //牌组
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
    /// 在preflop阶段给当前在游戏中的玩家一人发两张牌 仅玩家自已可见
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
    /// 给公共卡池发牌
    /// </summary>
    /// <param name="num">在公共卡池的发牌数量</param>
    public void AssignCardsToTable(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GlobalVar.publicCards.Add(AssignRandomCard());
        }
    }

    /// <summary>
    /// 随机从剩余牌堆中选出一张牌
    /// </summary>
    /// <returns>从剩余牌堆中抽出的牌</returns>
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
    /// 从最终翻牌的全部玩家中寻找冠军，可以是一个或多个平局
    /// </summary>
    /// <param name="pList">当局中的剩余玩家LIST</param>
    /// <returns>牌力最大的/当局冠军玩家LIST</returns>
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
    /// 比较两组牌的牌力
    /// </summary>
    /// <param name="c1">第一组的五张牌</param>
    /// <param name="c2">第二组的五张牌</param>
    /// <returns>如果c1大则return -1, c2大则return 1, 如果相同则return 0</returns>
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
    /// 是否为顺子
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
    /// 最多的相同数字牌有几个
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
    /// 是否为同花
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
    /// 中途数值变了几次
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
    /// 牌面总值
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

    //0：高牌
    //1：一对
    //2：两对
    //3：三条
    //4：顺子
    //5：同花
    //6：葫芦
    //7：四条
    //8：同花顺
    //9：皇家同花顺
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
