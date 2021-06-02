using System;
using UnityEngine.SceneManagement;

public static class GameResources {

    public static event EventHandler OnResourceAmountChanged;

    private static int resourceAmount;

    public static void AddGoldAmount(int amount, bool updatable) {
        resourceAmount += amount;
        if ((OnResourceAmountChanged != null) && (updatable)) OnResourceAmountChanged(null, EventArgs.Empty);
    }

    public static int GetGoldAmount() {
        return resourceAmount;
    }
}