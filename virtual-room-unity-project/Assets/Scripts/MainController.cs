using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public enum UserType { Virtual, Physical1, Physical2 }
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
    public GameObject mainUser;

    [Header("UI")]
    public InputField ipInput;
    public InputField userInput;
    public TextMesh userLabel;
    public Dropdown userTypeDropdown;

    public Action OnConnect { get; set; }
    public Action OnChangeUser { get; set; }
    public UserType MainUserType { get; private set; }
    public string UniqueID { get; private set; }

    public static MainController Instance;

    public void Awake() {        
        Instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        virtual_people_ = new List<VirtualPerson>();
        virtual_people_from_network_ = new List<NetworkVitualPerson>();

        //Unique id
        var uniqueId = PlayerPrefs.GetString("id");
        if (string.IsNullOrEmpty(uniqueId) || uniqueId.Contains('-')) {
#if UNITY_WEBGL && !UNITY_EDITOR
            uniqueId = System.Guid.NewGuid().ToString().Replace("-", "");
#else
            uniqueId = SystemInfo.deviceUniqueIdentifier.Replace("-", "");
#endif
            PlayerPrefs.SetString("id", uniqueId);
        }
        UniqueID = uniqueId;

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

        //Type info
        var userTypeFromPrefs = PlayerPrefs.GetInt("user_type");
        if (userTypeFromPrefs >= 0)
            MainUserType = (UserType)userTypeFromPrefs;
        else
            PlayerPrefs.SetInt("user_type", (int)MainUserType);
        userTypeDropdown.value = (int)MainUserType;


        ////UDP TEST
        //UdpClient udpClient = new UdpClient(4100);
        //try {
        //    udpClient.Connect(ip, port);

        //    // Sends a message to the host to which you have connected.
        //    var sendBytes = Encoding.ASCII.GetBytes("TESSSS");

        //    udpClient.Send(sendBytes, sendBytes.Length);

        //    //IPEndPoint object will allow us to read datagrams sent from any source.
        //    var RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        //    // Blocks until a message returns on this socket from a remote host.
        //    //Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
        //    var  receiveResult = udpClient.ReceiveAsync();
        //    receiveResult.
        //    string returnData = Encoding.ASCII.GetString(receiveBytes);

        //    // Uses the IPEndPoint object to determine which of these two hosts responded.
        //    Debug.Log("This is the message you received " +
        //                                 returnData.ToString());
        //    Debug.Log("This message was sent from " +
        //                                RemoteIpEndPoint.Address.ToString() +
        //                                " on their port number " +
        //                                RemoteIpEndPoint.Port.ToString());

        //    udpClient.Close();
        //} catch (Exception e) {
        //    Debug.Log(e.ToString());
        //}
    }

    private void Start() {
        SetWebRequests();        
    }

    void SetWebRequests() {
        all_virtual_Request_uri = Url() + "/get_all_virtual/" + UniqueID;
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
        using (UnityWebRequest webRequest = UnityWebRequest.Get(all_virtual_Request_uri)) {
            yield return webRequest.SendWebRequest();
            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var responseText = webRequest.downloadHandler.text;
                    if (responseText == null) break;

                    ParseVirtualPeople(responseText, virtual_people_from_network_);

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
            }
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

    public void OnUserTypeChange(int selection) {
        MainUserType = (UserType)selection;
        mainUser.SetActive(selection == 0);
        PlayerPrefs.SetInt("user_type", (int)MainUserType);
    }
#endregion

    float timer_;
    bool requesting_;
    string all_virtual_Request_uri;
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
