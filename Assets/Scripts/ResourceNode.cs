using UnityEngine;
using CodeMonkey;
using System;
using CodeMonkey.Utils;
public class ResourceNode {
    private Transform resourceNodeTransform;
    private int resourceAmount;
    public static event EventHandler OnResourceNodeClicked;
    public ResourceNode(Transform resourceNodeTransform) {
        this.resourceNodeTransform = resourceNodeTransform;
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