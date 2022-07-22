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
    /// ��PlayerObject���г�ʼ��
    /// </summary>
    /// <param name="p">��֮������Player����</param>
    public void InitializeThePlayer(Player p)
    {
        player = p;
        nameText.text = player.playerName;
        coinsText.text = GlobalVar.initCoin.ToString();
    }

    /// <summary>
    /// ֱ�Ӹ��� PlayerCardList ����ʾ���ơ���ע�⣺���ø÷���ʱ��player�Ѿ����ù�.AddPlayerCards()
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
            //��ʾ�Ʊ���
        }
        StartCoroutine(UIManager.instance.FlopAnim(card1Image.GetComponent<RectTransform>()));
        StartCoroutine(UIManager.instance.FlopAnim(card2Image.GetComponent<RectTransform>()));
    }

    /// <summary>
    /// ���³��������������������ӳ��룬�������٣�
    /// ����0���ǵ���ˢ�µ�ǰ������ʾ��
    /// </summary>
    /// <param name="change"></param>
    /*public void UpdateCoinsCount(int change)
    {
        if (change + player.coin < 0)
            return;
        else
            player.coin += change;
        UIManager.instance.UpdateCoinsPool(-change);//���ӮǮ���ؼ��٣���Ҷ�Ǯ��������
        coinsText.text = player.coin.ToString();
    }*/
    //������

    /// <summary>
    /// ������ҳ�������
    /// </summary>
    public void UpdateCoinsCount()
    {
        coinsText.text = player.coin.ToString();
    }
    /// <summary>
    /// ������ҵ�������ע��������
    /// </summary>
    public void UpdateBetCoinsCount()
    {
        sumBetsText.text = player.betCoin.ToString();
    }

    /// <summary>
    /// ����ͷ��Ϊ���ж���
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
    /// ����ͷ��Ϊ���ȴ���
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
    /// �Ѿ����ƻ���All_in�����������������Ȿ���ڲ����к�������
    /// </summary>
    public void NoMoreActions_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_noMoreActions");
    }

    /// <summary>
    /// ����ͷ��Ϊ���뿪��Ϸ��
    /// </summary>
    public void QuitTheGame_AvatarChange()
    {
        avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_empty");
    }

    /// <summary>
    /// ʤ����Ч�������ɫ
    /// </summary>
    public void PlayerWin()
    {
        this.GetComponent<Image>().color = new Color(255f / 255, 204f / 255, 0, 100f / 255);
    }

    /// <summary>
    /// ����ʤ����Ч����غ�ɫ
    /// </summary>
    public void PlayerWinEnded()
    {
        this.GetComponent<Image>().color = new Color(0, 0, 0, 100f / 255);
    }

}
