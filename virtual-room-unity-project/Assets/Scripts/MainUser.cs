﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUser : MonoBehaviour
{
    public float speed;
    public float boostSpeed;

    void Update()
    {
        //Mouse Interaction
        if (Input.GetMouseButtonDown(0)) {
            var mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            var ray = Camera.main.ScreenPointToRay(mousePos);
            var hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null && hit.collider.CompareTag("Player")) {                
                grabbed_ = true;
            }
        }
        if (grabbed_) {
            transform.position = getPosition();
        }
        if (Input.GetMouseButtonUp(0)) {
            grabbed_ = false;
        }

        //Keyboard interaction
        var hAxis = Input.GetAxisRaw("Horizontal");
        var vAxis = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(hAxis) > 0.1f || Mathf.Abs(vAxis) > 0.1) {
            var modifySpeed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            transform.position += (!modifySpeed ? speed : boostSpeed) * Time.deltaTime * new Vector3(hAxis, vAxis, 0);
        }
    }

    Vector3 getPosition() {
        var mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
       return Camera.main.ScreenToWorldPoint(mousePos);
    }

    bool grabbed_;
}
