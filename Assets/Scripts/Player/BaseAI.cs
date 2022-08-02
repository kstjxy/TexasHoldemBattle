using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI
{
    public enum aiType
    {
        BaseAI,
        LuaAI,
        WebAI
    }
    public aiType type = aiType.BaseAI;
    public string name = "Undefined";

}
