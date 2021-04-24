using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPerson : MonoBehaviour {

    public float smoothDamp = 0.03f;
    public string Id { get; set; }

    PositionRequest positionRequest_;

    void Start() {
        positionRequest_ = new PositionRequest("GetVirtualPosition", "get_virtual_position", Id + "", this, smoothDamp);
    }

    void Update() {
        positionRequest_.Update();
    }
}
