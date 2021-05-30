using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameResources {

    public static event EventHandler OnGoldAmountChanged;

    private static int goldAmount;

    public static void AddGoldAmount(int amount) {
        goldAmount += amount;
        if (OnGoldAmountChanged != null) OnGoldAmountChanged(null, EventArgs.Empty);
    }

    public static int GetGoldAmount() {
        return goldAmount;
    }
}