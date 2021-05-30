using System;

public static class GameResources {

    public static event EventHandler OnResourceAmountChanged;

    private static int resourceAmount;

    public static void AddGoldAmount(int amount) {
        resourceAmount += amount;
        if (OnResourceAmountChanged != null) OnResourceAmountChanged(null, EventArgs.Empty);
    }

    public static int GetGoldAmount() {
        return resourceAmount;
    }
}