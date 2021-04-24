using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRequest
{
    public PositionRequest(string requestName, string apiFunctionName, string id, MonoBehaviour handler, float smoothDamp) {
        handler_ = handler;
        smoothDamp_ = smoothDamp;
        position_Request = new WrapperWebRequest(requestName, MainController.Instance.Url() + "/"+ apiFunctionName + "/" + id, "GET");
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
        position_Request.SendAsync();
        while (position_Request.Requesting) {
            yield return null;
        }
        switch (position_Request.ErrorStatus) {
            case WrapperWebRequest.ErroType.None:
                currentPosition_ = MainController.ParsePosition(position_Request.ResponseText);
                break;
            default:
                break;
        }
        requesting_ = false;
    }

    bool requesting_;
    WrapperWebRequest position_Request;
    Vector3 currentPosition_;
    MonoBehaviour handler_;
    float smoothDamp_;
}
