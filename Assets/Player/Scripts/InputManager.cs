using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : GenericSingleton<InputManager>
{
	public event Action<float> pinch;
	public event Action<float> rotate;
    public event Action<float, float> locationChanged;
    public event Action<Vector3> move;
    public bool isEditor = false;

    float longitude, latitude;

    private void Start()
    {
        isEditor = Application.isEditor;
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Gps not enabled");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            Debug.Log("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine location");
            yield break;
        }

        yield break;
    }

    void Update()
	{
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (isEditor)
        {
            EditorInput();
        }
        else
        {
            MobileInput();
        }
	}

	void EditorInput()
	{
		Pinch (Input.GetAxis("Mouse ScrollWheel") * 10);
		Rotate (Input.GetAxis("Mouse X"));
        Move(new Vector3(Input.GetAxis("Horizontal"), 0 , Input.GetAxis("Vertical")));
	}

	void MobileInput()
	{
        Touch[] touches = Input.touches;
		if (Input.touchCount == 2 && (touches[0].phase == TouchPhase.Moved || touches[1].phase == TouchPhase.Moved))
            TouchPinchAndRotate(touches[0], touches[1]);

        float lon = Input.location.lastData.longitude;
        float lat = Input.location.lastData.latitude;
        if (lon != longitude || lat != latitude)
        {
            longitude = lon;
            latitude = lat;
            locationChanged(longitude, latitude);
        }
    }

    //Handles multitouch zoom and rotation
    void TouchPinchAndRotate(Touch touch0, Touch touch1)
    {
        Vector2 touch = touch0.position - touch1.position;
        Vector2 prevTouch = (touch0.position - touch0.deltaPosition) -
            (touch1.position - touch1.deltaPosition);

        //Zoom
        Pinch((prevTouch.magnitude - touch.magnitude) * 0.2f);

        //Rotation
        touch = touch.x > 0 ? touch : -touch;
        prevTouch = prevTouch.x > 0 ? prevTouch : -prevTouch;

        float amount = Vector2.Dot(Vector2.up, touch.normalized) - Vector2.Dot(Vector2.up, prevTouch.normalized);
        if (Mathf.Abs(amount) <= 1)
            Rotate(amount * 100);
    }

    void Pinch(float amount)
    {
        if(pinch != null)
        {
            pinch(amount);
        }
    }

    void Rotate(float amount)
    {
        if (rotate != null)
        {
            rotate(amount);
        }
    }

    void LocationChanged(float lon, float lat)
    {
        if (locationChanged != null)
        {
            locationChanged(lon, lat);
        }
    }

    void Move(Vector3 vec)
    {
        if (move != null)
        {
            move(vec);
        }
    }
}
