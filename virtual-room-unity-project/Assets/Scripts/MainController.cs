using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [Header("Network Settings")]
    public string ip = "192.168.1.101";
    public int port = 5000;

    public static MainController Instance;

    public void Awake() {
        Instance = this;
    }

    public string Url() {
        return "http://"+ip.Trim()+":"+port+"";
    }
}
