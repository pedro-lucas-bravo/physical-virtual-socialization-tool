using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PokeAction : MonoBehaviour
{

    public Animator senderAnimator;
    public string sendPokeAnimation;
    public Animator receiverAnimator;
    public string receivePokeAnimation;

    public Person Target { get; private set; }

    private void Start() {
        Target = GetComponent<Person>();
    }

    public void SendPoke() {
        senderAnimator.Play(sendPokeAnimation, 0, 0);        
    }

    public void ReceivePoke() {
        receiverAnimator.Play(receivePokeAnimation);
    }

       
}
