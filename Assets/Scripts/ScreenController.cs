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

    void OnEnable()
    {
        EasyTouch.On_PinchIn += camYchangedMinus;
        EasyTouch.On_PinchOut += camYchangedPlus;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    // Unsubscribe to events
    void UnsubscribeEvent()
    {
        EasyTouch.On_PinchIn -= camYchangedMinus;
        EasyTouch.On_PinchOut -= camYchangedPlus;
    }

    void camYchangedPlus(Gesture gesture)
    {
        if ((Camera.mainCamera.transform.position.y - gesture.deltaPinch / 100) > 2)
        {
            Camera.mainCamera.transform.position = new Vector3(Camera.mainCamera.transform.position.x, Camera.mainCamera.transform.position.y - gesture.deltaPinch / 100, Camera.mainCamera.transform.position.z);
        }
    }

    void camYchangedMinus(Gesture gesture)
    {
        Camera.mainCamera.transform.position = new Vector3(Camera.mainCamera.transform.position.x, Camera.mainCamera.transform.position.y + gesture.deltaPinch / 100, Camera.mainCamera.transform.position.z);
        
    }
}
