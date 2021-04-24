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
    
    void Update(){
        positionRequest_.Update();
    }
}
