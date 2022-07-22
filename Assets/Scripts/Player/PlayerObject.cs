using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    [Header("PlayerInfo")]
    public Player player;

    [Header("PlayerPanelElements")]
    public Text nameText;
    public Text coinsText;
    public Text sumBetsText;
    public Image avatarImage;
    public Image card1Image;
    public Image card2Image;

    /// <summary>
    /// 对PlayerObject进行初始化
    /// </summary>
    /// <param name="p">与之关联的Player本身</param>
    public void InitializeThePlayer(Player p)
    {
        player = p;
        nameText.text = player.playerName;
        coinsText.text = GlobalVar.initCoin.ToString();
    }

    /// <summary>
    /// 直接根据 PlayerCardList 来显示卡牌。请注意：调用该方法时，player已经调用过.AddPlayerCards()
    /// </summary>
    public void ShowCards()
    {
        if (UIManager.instance.isShowingCards && player.playerCardList.Count>=2)
        {
            card1Image.sprite = player.playerCardList[0].GetSpriteSurface();
            card2Image.sprite = player.playerCardList[1].GetSpriteSurface();
        }
        else
        {
            Sprite cardBack = Resources.Load<Sprite>("Cards/card_back");
            card1Image.sprite = cardBack;
            card2Image.sprite = cardBack;
            //显示牌背面
        }
        StartCoroutine(UIManager.instance.FlopAnim(card1Image.GetComponent<RectTransform>()));
        StartCoroutine(UIManager.instance.FlopAnim(card2Image.GetComponent<RectTransform>()));
    }

    /// <summary>
    /// 更新筹码数量，输入正数增加筹码，负数减少，
    /// 输入0则是单纯刷新当前筹码显示。
    /// </summary>
    /// <param name="change"></param>
    /*public void UpdateCoinsCount(int change)
    {
        if (change + player.coin < 0)
            return;
        else
            player.coin += change;
        UIManager.instance.UpdateCoinsPool(-change);//玩家赢钱奖池减少，玩家赌钱奖池增多
        coinsText.text = player.coin.ToString();
    }*/
    //已弃用

    /// <summary>
    /// 更新玩家筹码数量
    /// </summary>
    public void UpdateCoinsCount()
    {
        coinsText.text = player.coin.ToString();
    }
    /// <summary>
    /// 更新玩家当局总下注筹码数量
    /// </summary>
    public void UpdateBetCoinsCount()
    {
        sumBetsText.text = player.betCoin.ToString();
    }

    /// <summary>
    /// 更改头像为：行动中
    /// </summary>
    public void HightLightAction_AvatarChange()
    {
        if (player.isFold == true) 
            return;
        switch (player.role)
        {
            case Player.PlayerRole.button:
                avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_button_actioning");
                break;
            case Player.PlayerRole.bigBlind:
                avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_BB_actioning");
                break;
            case Player.PlayerRole.smallBlind:
                avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_SB_actioning");
                break;
            default:
                avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_actioning");
                break;
        }
    }

    /// <summary>
    /// 更改头像为：等待中
    /// </summary>
    public void BackToWaiting_AvatarChange()
    {
        switch (player.role)
        {
            case Player.PlayerRole.button:
                avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_button_waiting");
                break;
            case Player.PlayerRole.bigBlind:
                avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_BB_waiting");
                break;
            case Player.PlayerRole.smallBlind:
                avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_SB_waiting");
                break;
            default:
                avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_waiting");
                break;
        }
    }

    /// <summary>
    /// 已经弃牌或者All_in，除了最终牌组搭建以外本局内不再有后续操作
    /// </summary>
    public void NoMoreActions_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_noMoreActions");
    }

    /// <summary>
    /// 更改头像为：离开游戏！
    /// </summary>
    public void QuitTheGame_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_empty");
    }

    /// <summary>
    /// 胜利特效：面板变金色
    /// </summary>
    public void PlayerWin()
    {
        this.GetComponent<Image>().color = new Color(255f / 255, 204f / 255, 0, 100f / 255);
    }

    /// <summary>
    /// 结束胜利特效，变回黑色
    /// </summary>
    public void PlayerWinEnded()
    {
        this.GetComponent<Image>().color = new Color(0, 0, 0, 100f / 255);
    }

}
