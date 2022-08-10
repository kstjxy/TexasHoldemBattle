using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    Animator panelAnim;
    public bool panelCalled;

    [Header("UI Elements")]
    public Button callPanelButton;
    public InputField smallBlindInjection;
    public InputField maximumGames;
    public InputField raisisLimit;
    // Start is called before the first frame update
    void Start()
    {
        panelAnim = this.GetComponent<Animator>();
        panelAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        panelCalled = false;
        callPanelButton.onClick.AddListener(delegate { CallPanelButtonClicked(); });
    }

    void CallPanelButtonClicked()
    {
        if (panelCalled)
        {
            panelAnim.Play("Out");
            UIManager.instance.Continue_ButtonClicked();

            if (smallBlindInjection.text == "")
                UIManager.instance.PrintLog("最低小盲注未填入，更新失败！");
            else if (int.Parse(smallBlindInjection.text) <= 0)
                UIManager.instance.PrintLog("最低小盲注必须为正数，更新失败！");
            else if (GlobalVar.minBetCoin != int.Parse(smallBlindInjection.text))
            {
                UIManager.instance.PrintLog("最低小盲注数更新：<color=#FFCAC9>" + smallBlindInjection.text + "</color>");
                GlobalVar.minBetCoin = int.Parse(smallBlindInjection.text);
            }

            if (maximumGames.text == "" )
                UIManager.instance.PrintLog("最大游戏局数未填入，更新失败！");
            else if (int.Parse(smallBlindInjection.text) <= GlobalVar.curRoundNum)
                UIManager.instance.PrintLog("最大游戏局数小于已进行游戏局数，更新失败！");
            else if (GlobalVar.totalRoundNum != int.Parse(maximumGames.text))
            {
                UIManager.instance.PrintLog("游戏最大次数更新：<color=#C9FFDD>" + maximumGames.text + "</color>");
                GlobalVar.totalRoundNum = int.Parse(maximumGames.text);
            }

            if (raisisLimit.text == "")
                UIManager.instance.PrintLog("每轮加注限制未填入，更新失败！");
            else if (int.Parse(raisisLimit.text) <= 0)
                UIManager.instance.PrintLog("每轮加注限制必须为正数，更新失败！");
            else if (GlobalVar.maxBetCount != int.Parse(raisisLimit.text))
            {
                UIManager.instance.PrintLog("每轮加注限制更新：<color=#C9FFF9>" + raisisLimit.text + "</color>"); 
                GlobalVar.maxBetCount = int.Parse(raisisLimit.text);
            }
            
            UIManager.instance.UpdateGameRounds();
        }
        else
        {
            panelAnim.Play("In");
            smallBlindInjection.text = GlobalVar.minBetCoin.ToString();
            maximumGames.text = GlobalVar.totalRoundNum.ToString();
            raisisLimit.text = GlobalVar.maxBetCount.ToString();
            UIManager.instance.Pause_ButtonClicked();
        }
        panelCalled = !panelCalled;
    }
}
