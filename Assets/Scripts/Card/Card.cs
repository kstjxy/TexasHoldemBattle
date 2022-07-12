using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
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

    //从Resources中读取本卡牌对应的素材图片
    public Sprite GetSpriteSurface()
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
        return Resources.Load<Sprite>("Cards/" + cardSuit.ToString() + "s/" + cardSuit.ToString() + "_" + valueString);
    }
    
}
public enum CardSuit
{ 
    club,
    spade,
    diamon,
    heart
}
