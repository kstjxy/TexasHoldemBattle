using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUseEffectControl : MonoBehaviour
{
    public void BackToObjectPool()
    {
        this.gameObject.SetActive(false);
        UIManager.instance.textEffectsPool.Enqueue(this.gameObject);
        UIManager.instance.activeTextEffects.Remove(this.gameObject);
    }
}
