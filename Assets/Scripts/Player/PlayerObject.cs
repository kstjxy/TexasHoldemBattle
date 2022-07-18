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
        coinsText.text = GolbalVar.initCoin.ToString();
    }

    /// <summary>
    /// 直接根据 PlayerCardList 来显示卡牌。请注意：调用该方法时，player已经调用过.AddPlayerCards()
    /// </summary>
    public void ShowCards()
    {
        card1Image.sprite = player.playerCardList[0].GetSpriteSurface();
        card2Image.sprite = player.playerCardList[1].GetSpriteSurface();
        StartCoroutine(UIManager.instance.FlopAnim(card1Image.GetComponent<RectTransform>()));
        StartCoroutine(UIManager.instance.FlopAnim(card2Image.GetComponent<RectTransform>()));
    }

    /// <summary>
    /// 更新筹码数量，输入正数增加筹码，负数减少，
    /// 输入0则是单纯刷新当前筹码显示。
    /// </summary>
    /// <param name="change"></param>
    public void UpdateCoinsCount(int change)
    {
        if (change + player.coin < 0)
            return;
        else
            player.coin += change;

        coinsText.text = player.coin.ToString();
    }

    /// <summary>
    /// 更改头像为：行动中
    /// </summary>
    public void HightLightAction_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_actioning");
    }

    /// <summary>
    /// 更改头像为：等待中
    /// </summary>
    public void BackToWaiting_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_wating");
    }

    /// <summary>
    /// 更改头像为：离开游戏！
    /// </summary>
    public void QuitTheGame_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_empty");
    }

}
