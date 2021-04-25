using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalPerson : MonoBehaviour
{
    public int id;
    public float smoothDamp = 0.1f;    

    PositionRequest positionRequest_;

    void Start() {
        positionRequest_ = new PositionRequest("GetPosition", "position", id + "", this, smoothDamp);
    }

    private void OnDestroy() {
        positionRequest_.OnDestroy();
    }

    void Update(){
        positionRequest_.Update();
    }
}
