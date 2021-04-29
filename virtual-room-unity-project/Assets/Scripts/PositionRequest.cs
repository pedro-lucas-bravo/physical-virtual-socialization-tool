using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PositionRequest
{
    public PositionRequest(string requestName, string apiFunctionName, string id, MonoBehaviour handler, float smoothDamp) {
        handler_ = handler;
        smoothDamp_ = smoothDamp;
        requestName_ = requestName;
        apiFunction_ = apiFunctionName;
        id_ = id;
        MainController.Instance.OnConnect += OnConnect;
        OnConnect();
    }

    public void OnDestroy() {
        if (MainController.Instance != null)
            MainController.Instance.OnConnect -= OnConnect;
    }

    private void OnConnect() {
        position_Request_uri = MainController.Instance.Url() + "/" + apiFunction_ + "/" + id_;
    }

    public void Update() {
        if (!requesting_) {
            handler_.StartCoroutine(GetPosition());
        }
        Vector3 vel = Vector3.zero;
        handler_.transform.position = Vector3.SmoothDamp(handler_.transform.position, currentPosition_, ref vel, smoothDamp_);
    }

    IEnumerator GetPosition() {
        requesting_ = true;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(position_Request_uri)) {
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
                    currentPosition_ = MainController.ParsePosition(responseText);
                    break;
            }
        }
        requesting_ = false;
    }

    bool requesting_;
    string position_Request_uri;
    Vector3 currentPosition_;
    MonoBehaviour handler_;
    float smoothDamp_;

    string requestName_;
    string apiFunction_;
    string id_;
}
