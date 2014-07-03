using UnityEngine;
using System.Collections;

public class ScreenController : MonoBehaviour {
    public GameObject joystickMove;
    public Transform followTransform;

	// Use this for initialization
	void Start () {
        var radJ = joystickMove.GetComponent<EasyJoystick>().zoneRadius;
        joystickMove.GetComponent<EasyJoystick>().joystickPosition = new Vector2(Screen.width - radJ - 20, Screen.height / 2);

	}
	
	// Update is called once per frame
	void Update () {
        var radJ = joystickMove.GetComponent<EasyJoystick>().zoneRadius;
        joystickMove.GetComponent<EasyJoystick>().joystickPosition = new Vector2(Screen.width - radJ - 20, Screen.height / 2);

        transform.position = new Vector3(followTransform.position.x, transform.position.y, followTransform.position.z);
	}
}
