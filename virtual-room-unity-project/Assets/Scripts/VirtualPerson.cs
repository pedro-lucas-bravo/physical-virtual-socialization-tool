﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPerson : MonoBehaviour {

    public float smoothDamp = 0.03f;
    public TextMesh label;
    public string Id { get; set; }

    PositionRequest positionRequest_;

    void Start() {
        positionRequest_ = new PositionRequest("GetVirtualPosition", "get_virtual_position", Id + "", this, smoothDamp);
    }

    private void OnDestroy() {
        positionRequest_.OnDestroy();
    }
    void Update() {
        positionRequest_.Update();
    }
}
