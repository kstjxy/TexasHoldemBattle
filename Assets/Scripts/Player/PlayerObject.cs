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
    /// ��PlayerObject���г�ʼ��
    /// </summary>
    /// <param name="p">��֮������Player����</param>
    public void InitializeThePlayer(Player p)
    {
        player = p;
        nameText.text = player.playerName;
        coinsText.text = GolbalVar.initCoin.ToString();
    }

    /// <summary>
    /// ֱ�Ӹ��� PlayerCardList ����ʾ���ơ���ע�⣺���ø÷���ʱ��player�Ѿ����ù�.AddPlayerCards()
    /// </summary>
    public void ShowCards()
    {
        card1Image.sprite = player.playerCardList[0].GetSpriteSurface();
        card2Image.sprite = player.playerCardList[1].GetSpriteSurface();
        StartCoroutine(UIManager.instance.FlopAnim(card1Image.GetComponent<RectTransform>()));
        StartCoroutine(UIManager.instance.FlopAnim(card2Image.GetComponent<RectTransform>()));
    }

    /// <summary>
    /// ���³��������������������ӳ��룬�������٣�
    /// ����0���ǵ���ˢ�µ�ǰ������ʾ��
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
    /// ����ͷ��Ϊ���ж���
    /// </summary>
    public void HightLightAction_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_actioning");
    }

    /// <summary>
    /// ����ͷ��Ϊ���ȴ���
    /// </summary>
    public void BackToWaiting_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_wating");
    }

    /// <summary>
    /// ����ͷ��Ϊ���뿪��Ϸ��
    /// </summary>
    public void QuitTheGame_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_empty");
    }

}
