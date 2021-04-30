using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalPerson : Person
{
    public int id;
    public float smoothDamp = 0.1f;    

    PositionRequest positionRequest_;

    public override string Id { get => id + ""; set => id = int.Parse(value); }

    protected override void Start() {
        base.Start();
        positionRequest_ = new PositionRequest("GetPosition", "position", id + "", this, smoothDamp);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        positionRequest_.OnDestroy();
    }

    void Update(){
        positionRequest_.Update();
    }
}
