using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPerson : Person {

    public float smoothDamp = 0.03f;
    public TextMesh label;

    PositionRequest positionRequest_;

    protected override void Start() {
        base.Start();
        positionRequest_ = new PositionRequest("GetVirtualPosition", "get_virtual_position", Id + "", this, smoothDamp);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        positionRequest_.OnDestroy();
    }
    void Update() {
        positionRequest_.Update();
    }
}
