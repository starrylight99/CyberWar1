using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameResources {

    public static event EventHandler OnResourceAmountChanged;
    public static event EventHandler OnFOWAmountChanged;

    public static int atkResourceAmount, defResourceAmount;
    public static int atkFOWAmt, defFOWAmt;

    public static void AddGoldAmount(int amount, bool isAtk) {
        if (isAtk){
            atkResourceAmount += amount;
        } else {
            defResourceAmount += amount;
        }
        Debug.Log(atkResourceAmount);
        Debug.Log(defResourceAmount);
        if (OnResourceAmountChanged != null) OnResourceAmountChanged(null, EventArgs.Empty);
    }

    public static void AddFOWAmount(int amount, bool isAtk)
    {
        if (isAtk)
        {
            atkFOWAmt += amount;
        }
        else
        {
            defFOWAmt += amount;
        }
        Debug.Log(atkFOWAmt);
        Debug.Log(defFOWAmt);
        if (OnFOWAmountChanged != null) OnFOWAmountChanged(null, EventArgs.Empty);
    }

    public static int GetGoldAmount(bool isAtk) {
        if (isAtk) {
            return atkResourceAmount;
        } else {
            return defResourceAmount;
        }
    }

    public static int GetFOWAmount(bool isAtk)
    {
        if (isAtk)
        {
            return atkFOWAmt;
        }
        else
        {
            return defFOWAmt;
        }
    }

}