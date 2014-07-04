using UnityEngine;
using System.Collections;

public class TriggerEndLevel : MonoBehaviour {
    public bool endLevel = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            //Debug.Log("COLLIDE");
            endLevel = true;
        }
    }
}
