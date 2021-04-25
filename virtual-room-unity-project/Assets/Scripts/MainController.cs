using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public struct NetworkVitualPerson {
        public string Id;
        public string UserName;
        public NetworkVitualPerson(string id, string userName) {
            Id = id;
            UserName = userName;
        }
    }

    [Header("Network Settings")]
    public string ip = "192.168.1.101";
    public int port = 5000;
    public string user = "Me";

    [Header("Virtual Environment")]
    public VirtualPerson virtualPersonPrefab;
    public Transform virtualPersonParent;
    public float periodForCheckingVirtuals = 1;

    [Header("UI")]
    public InputField ipInput;
    public InputField userInput;
    public TextMesh userLabel;

    public Action OnConnect { get; set; }
    public Action OnChangeUser { get; set; }

    public static MainController Instance;

    public void Awake() {
        Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        virtual_people_ = new List<VirtualPerson>();
        virtual_people_from_network_ = new List<NetworkVitualPerson>();

        //Ip Info
        var ipFromPrefs = PlayerPrefs.GetString("ip");
        if (!string.IsNullOrEmpty(ipFromPrefs))
            ip = ipFromPrefs;
        else
            PlayerPrefs.SetString("ip", ip);
        ipInput.text = ip;

        //User info
        var userFromPrefs = PlayerPrefs.GetString("user");
        if (!string.IsNullOrEmpty(userFromPrefs))
            user = userFromPrefs;
        else
            PlayerPrefs.SetString("user", user);
        userInput.text = userLabel.text = user;
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

                ParseVirtualPeople(all_virtual_Request.ResponseText, virtual_people_from_network_);

                //Remove and destroy if needed
                for (int i = 0; i < virtual_people_.Count; i++) {
                    var virtualPerson = virtual_people_[i];
                    if (!virtual_people_from_network_.Any(vp => vp.Id == virtualPerson.Id)) {
                        virtual_people_.Remove(virtualPerson);
                        Destroy(virtualPerson.gameObject);
                        i--;
                    }
                }

                if (virtual_people_from_network_.Count == 0)
                    break;                

                //Add if someone new or update user
                for (int i = 0; i < virtual_people_from_network_.Count; i++) {
                    var foundVp = virtual_people_.Where(p => p.Id == virtual_people_from_network_[i].Id).FirstOrDefault();
                    if (foundVp == null) {
                        var newInstance = GameObject.Instantiate<VirtualPerson>(virtualPersonPrefab);
                        newInstance.Id = virtual_people_from_network_[i].Id;
                        newInstance.label.text = virtual_people_from_network_[i].UserName;
                        newInstance.transform.parent = virtualPersonParent;
                        newInstance.transform.localPosition = Vector3.zero;
                        virtual_people_.Add(newInstance);
                    } else {
                        foundVp.label.text = virtual_people_from_network_[i].UserName;
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

    public void SetUser() {
        userLabel.text = user = userInput.text.Trim();
        PlayerPrefs.SetString("user", user);
        if (OnChangeUser != null)
            OnChangeUser();
    }

    #endregion

    float timer_;
    bool requesting_;
    WrapperWebRequest all_virtual_Request;
    List<VirtualPerson> virtual_people_;
    List<NetworkVitualPerson> virtual_people_from_network_;

    #region Utils

    //Assuming format x,y,z
    public static Vector3 ParsePosition(string strPos) {
        var pos = strPos.Split(',');
        return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
    }

    public static void ParseVirtualPeople(string strPeople, List<NetworkVitualPerson> result) {
        result.Clear();
        var people = strPeople.Split(',');
        if (people == null || (people != null && people.Length == 1 && people[0] == ""))
            return;                   
        for (int i = 0; i < people.Length; i++) {
            var data = people[i].Split('-');
            result.Add(new NetworkVitualPerson(data[0], data[1]));
        }

    }


    #endregion
}
