using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameResources {

    public static event EventHandler OnResourceAmountChanged;

    public static int atkResourceAmount, defResourceAmount;

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

    public static int GetGoldAmount(bool isAtk) {
        if (isAtk) {
            return atkResourceAmount;
        } else {
            return defResourceAmount;
        }
    }
}