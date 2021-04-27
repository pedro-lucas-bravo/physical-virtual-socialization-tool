using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestPoint : MonoBehaviour
{
    public GameObject content;

    private void OnTriggerEnter2D(Collider2D collision) {
        ActivateContent(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        ActivateContent(collision, false);
    }

    void ActivateContent(Collider2D collision, bool act) {
        switch (MainController.Instance.MainUserType) {
            case MainController.UserType.Virtual: {
                    if (collision.GetComponent<MainUser>() != null)
                        content.SetActive(act);
                }
                break;
            case MainController.UserType.Physical1: {
                    var phy = collision.GetComponent<PhysicalPerson>();
                    if (phy != null && phy.id == 1)
                        content.SetActive(act);
                }
                break;
            case MainController.UserType.Physical2: {
                    var phy = collision.GetComponent<PhysicalPerson>();
                    if (phy != null && phy.id == 2)
                        content.SetActive(act);
                }
                break;
            default:
                break;
        }
    }
}
