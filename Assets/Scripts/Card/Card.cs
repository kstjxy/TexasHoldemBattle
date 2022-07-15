using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    //卡牌花色
    public CardSuit cardSuit;
    //卡面大小 from 2 to 13(K) 14(A)
    private int value;
    #region Get Set 限制
    public int Value
    {
        get
        {
            return value;
        }
        set
        {
            if (value < 2)
                this.value = 2;
            else if (value > 14)
                this.value = 14;
            else
                this.value = value;
        }
    }
    #endregion

    public Card(CardSuit cs, int vl)
    {
        cardSuit = cs;
        if (vl < 2)
            this.value = 2;
        else if (vl> 14)
            this.value = 14;
        else
            this.value = vl;
    }

    /// <summary>
    /// 返回牌面数值和花色的信息
    /// </summary>
    /// <returns>牌面数值和花色的信息</returns>
    public string PrintCard()
    {
        string cardInfo = "";
        switch (cardSuit)
        {
            case CardSuit.club:
                cardInfo = "♣" + "  " + GetValueString();
                break;
            case CardSuit.spade:
                cardInfo = "♠" + "  " + GetValueString();
                break;
            case CardSuit.diamon:
                cardInfo = "<color=#FF3000>♦" + " " + GetValueString()+ "</color>";
                break;
            case CardSuit.heart:
                cardInfo = "<color=#FF3000>♥" + " " + GetValueString() + "</color>";
                break;
        }
        return cardInfo;
    }

    /// <summary>
    /// 从Resources中读取本卡牌对应的素材图片
    /// </summary>
    /// <returns>相应的卡牌图片</returns>
    public Sprite GetSpriteSurface()
    {
        return Resources.Load<Sprite>("Cards/" + cardSuit.ToString() + "s/" + cardSuit.ToString() + "_" + GetValueString());
    }

    /// <summary>
    /// 根据卡面数字返回正确的字符（AJQK或者数字）
    /// </summary>
    /// <returns>卡面字符串</returns>
    public string GetValueString()
    {
        string valueString;
        switch (value)
        {
            case 14:
                valueString = "A";
                break;
            case 11:
                valueString = "J";
                break;
            case 12:
                valueString = "Q";
                break;
            case 13:
                valueString = "K";
                break;
            default:
                valueString = value.ToString();
                break;
        }
        return valueString;
    }
}
public enum CardSuit
{ 
    club,       //梅花
    spade,      //黑桃
    diamon,     //方块
    heart       //红心
}
