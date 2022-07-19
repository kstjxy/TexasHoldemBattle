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
            if (GolbalVar.minBetCoin != int.Parse(smallBlindInjection.text))
                UIManager.instance.PrintLog("最低小盲注数更新：<color=#FFCAC9>" + smallBlindInjection.text + "</color>");
            if (GolbalVar.totalRoundNum != int.Parse(maximumGames.text))
                UIManager.instance.PrintLog("游戏最大次数更新：<color=#C9FFDD>" + maximumGames.text + "</color>");
            if (GolbalVar.maxBetCount != int.Parse(raisisLimit.text))
            UIManager.instance.PrintLog("每轮加注限制更新：<color=#C9FFF9>" + raisisLimit.text + "</color>");
            GolbalVar.minBetCoin = int.Parse(smallBlindInjection.text);
            GolbalVar.totalRoundNum = int.Parse(maximumGames.text);
            GolbalVar.maxBetCount = int.Parse(raisisLimit.text);
            UIManager.instance.UpdateGameRounds();
        }
        else
        {
            panelAnim.Play("In");
            smallBlindInjection.text = GolbalVar.minBetCoin.ToString();
            maximumGames.text = GolbalVar.totalRoundNum.ToString();
            raisisLimit.text = GolbalVar.maxBetCount.ToString();
            UIManager.instance.Pause_ButtonClicked();
        }
        panelCalled = !panelCalled;
    }
}
