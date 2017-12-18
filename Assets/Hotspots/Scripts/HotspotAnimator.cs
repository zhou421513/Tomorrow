using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotspotAnimator : MonoBehaviour {

    public float rotationSpeed;
    public float sinSpeed;
    public float heightMultiplier;
	
    void Start()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.up);
        transform.position += Vector3.up * 2f;
    }

	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
        transform.position += new Vector3(0, Mathf.Sin(Time.time*sinSpeed)*heightMultiplier, 0);
    }
}
