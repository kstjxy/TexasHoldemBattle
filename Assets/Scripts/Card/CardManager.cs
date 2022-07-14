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

    public void AssignCardsToPlayers(List<Player> playerList)
    {
        foreach(Player p in playerList)
        {
            p.AddPlayerCards(AssignRandomCard(), AssignRandomCard());
        }
    }

    public void AssignCardsToTable()
    {
        GolbalVar.publicCards.Add(AssignRandomCard());
    }

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

    //从最终翻牌的全部玩家中寻找冠军，可以是一个或多个平局
    public List<Player> FindWinner(List<Player> pList)
    {
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

    //比较两组牌的牌力，如果c1大则return -1, c2大则return 1, 如果相同则return 0
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


    /// <summary>
    /// 发规定数量的牌到桌面上，并显示
    /// </summary>
    /// <param name="num">牌的数量</param>
    public void DealACardToTable(int num)
    {
        //在这个方法中要做到：更新公共牌列表（GameManager.instance），显示新增的牌（UIManager.instance）
    }


    public void DealCardsToAPlayer()
    {
        //在这个方法中要做到：更新玩家牌列表（用方法PlayerObject.player.AddPlayerCards()），显示玩家的牌（用方法PlayerObject.ShowCards()）
    }

}
