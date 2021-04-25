using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    [Header("Network Settings")]
    public string ip = "192.168.1.101";
    public int port = 5000;

    [Header("Virtual Environment")]
    public VirtualPerson virtualPersonPrefab;
    public Transform virtualPersonParent;
    public float periodForCheckingVirtuals = 1;

    [Header("UI")]
    public InputField ipInput;

    public Action OnConnect { get; set; }

    public static MainController Instance;

    public void Awake() {
        Instance = this;
        virtual_people_ = new List<VirtualPerson>();
        var ipFromPrefs = PlayerPrefs.GetString("ip");
        if (!string.IsNullOrEmpty(ipFromPrefs))
            ip = ipFromPrefs;
        else
            PlayerPrefs.SetString("ip", ip);
        ipInput.text = ip;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start() {
        SetWebRequests();        
    }

    void SetWebRequests() {
        all_virtual_Request = new WrapperWebRequest("GetAllVirtualPeople", MainController.Instance.Url() + "/get_all_virtual/" + SystemInfo.deviceUniqueIdentifier, "GET");
    }

    private void Update() {
        timer_ += Time.deltaTime;
        if (timer_ > periodForCheckingVirtuals && !requesting_) {
            StartCoroutine(GetAllVirtualPeople());
            timer_ = 0;
        }
    }

    public string Url() {
        return "http://"+ip.Trim()+":"+port+"";
    }

    public IEnumerator GetAllVirtualPeople() {
        requesting_ = true;
        all_virtual_Request.SendAsync();
        while (all_virtual_Request.Requesting) {
            yield return null;
        }
        switch (all_virtual_Request.ErrorStatus) {
            case WrapperWebRequest.ErroType.None:
                if (all_virtual_Request.ResponseText == null) break;

                var people = all_virtual_Request.ResponseText.Split(',');

                //Remove and destroyif needed
                for (int i = 0; i < virtual_people_.Count; i++) {
                    var virtualPerson = virtual_people_[i];
                    if (!people.Contains(virtualPerson.Id)) {
                        virtual_people_.Remove(virtualPerson);
                        Destroy(virtualPerson.gameObject);
                        i--;
                    }
                }

                if (people == null || (people != null && people.Length == 1 && people[0] == ""))
                    break;                

                //Add if someone new
                for (int i = 0; i < people.Length; i++) {
                    if (!virtual_people_.Any(p => p.Id == people[i])) {
                        var newInstance = GameObject.Instantiate<VirtualPerson>(virtualPersonPrefab);
                        newInstance.Id = people[i];
                        newInstance.transform.parent = virtualPersonParent;
                        newInstance.transform.localPosition = Vector3.zero;
                        virtual_people_.Add(newInstance);
                    }
                }

                break;
            default:
                break;
        }
        requesting_ = false;
    }

    #region Events

    public void Connect() {
        ip = ipInput.text.Trim();
        PlayerPrefs.SetString("ip", ip);
        SetWebRequests();
        if (OnConnect != null)
            OnConnect();
    }

    #endregion

    float timer_;
    bool requesting_;
    WrapperWebRequest all_virtual_Request;
    List<VirtualPerson> virtual_people_;

    #region Utils

    //Assuming format x,y,z
    public static Vector3 ParsePosition(string strPos) {
        var pos = strPos.Split(',');
        return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
    }


    #endregion
}
