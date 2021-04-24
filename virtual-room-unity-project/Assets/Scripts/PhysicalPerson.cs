using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalPerson : MonoBehaviour
{
    public int id;
    public float smoothDamp = 0.1f;
    //public float apiCallPeriod = 0.1f;


    void Start() {
        position_Request = new WrapperWebRequest("GetPosition", MainController.Instance.Url() + "/position/"+id, "GET");
    }

    
    void Update(){
        //timer_ += Time.deltaTime;
        //if (timer_ > apiCallPeriod) {
        if (!requesting_) {
            StartCoroutine(GetPosition());
            //timer_ = 0;
            //Debug.LogWarning("Tick");
        }
        Vector3 vel = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, currentPosition_, ref vel, smoothDamp);
    }

    public IEnumerator GetPosition() {
        requesting_ = true;
        position_Request.SendAsync();
        while (position_Request.Requesting) {
            yield return null;
        }
        switch (position_Request.ErrorStatus) {
            case WrapperWebRequest.ErroType.None:
                currentPosition_ = ParsePosition(position_Request.ResponseText);                
                break;
            default:
                break;
        }
        requesting_ = false;
    }

    //Assuming format x,y,z
    Vector3 ParsePosition(string strPos) {
        var pos = strPos.Split(',');
        return new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
    }

    WrapperWebRequest position_Request;
    Vector3 currentPosition_;
    float timer_;
    bool requesting_;
}
