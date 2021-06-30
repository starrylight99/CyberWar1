using UnityEngine;
using CodeMonkey;
using System;
using CodeMonkey.Utils;
public class ResourceNode {
    public int serialNumber;
    private Transform resourceNodeTransform;
    private int resourceAmount;
    public static event EventHandler OnResourceNodeClicked;
    public ResourceNode(Transform resourceNodeTransform, int serialNumber) {
        this.resourceNodeTransform = resourceNodeTransform;
        this.serialNumber = serialNumber;
        resourceAmount = 6;
        resourceNodeTransform.GetComponent<Button_Sprite>().ClickFunc = () => {
            if (OnResourceNodeClicked != null) OnResourceNodeClicked(this, EventArgs.Empty);
        };
    }

    public Vector2 GetPosition() {
        return resourceNodeTransform.position;
    }
    public void GrabResource() {
        resourceAmount -= 1;
        //CMDebug.TextPopupMouse("resourceAmount:"+resourceAmount);
    }
    public bool HasResources() {
        return resourceAmount > 0;
    }
}