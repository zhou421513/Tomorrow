using Maps;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    public Transform playerTransform;
    public Transform cameraTransform;
    public MapFunctionality map;

    [Range(0, 5)]
    public float lookOverHeight;
    [Range(0, 10)]
    public float minZoomHeight;
    [Range(10, 1000)]
    public float maxZoomHeight;
    [Range(0, 50)]
    public float cameraDistance;
    [Range(100, 4000)]
    public int radius;
    [Range(100, 4000)]
    public int refreshTreshold;

    Vector3 lastPosition;

    private void Start()
    {
        lastPosition = map.transform.position;
        transform.position = map.transform.position;
        map.ShowMapArea(lastPosition, radius);
    }

    //Disable this script if no camera is set or if no player prefab is given
    //Set the cameras position and rotation
    void OnEnable()
    {
        if (cameraTransform == null || playerTransform == null)
        {
            enabled = false;
            return;
        }
        
        playerTransform.parent = transform;
        cameraTransform.SetParent(transform);
        cameraTransform.position = playerTransform.position - playerTransform.forward * cameraDistance + new Vector3(0, minZoomHeight + (maxZoomHeight - minZoomHeight) / 2, 0);
        Zoom(0);

		InputManager.Instance.pinch += Zoom;
		InputManager.Instance.rotate += RotateVertical;
        InputManager.Instance.move += Move;
        InputManager.Instance.locationChanged += ChangePosition;
    }

    void OnDisable()
	{
		InputManager.Instance.pinch -= Zoom;
		InputManager.Instance.rotate -= RotateVertical;
        InputManager.Instance.move -= Move;
        InputManager.Instance.locationChanged -= ChangePosition;
    }

    //Moves the camera up or down and points the camera between players position and lookOverHeight
    public void Zoom(float amount)
    {
            Vector3 cameraPos = cameraTransform.position;
            float distanceRatio = (cameraPos.y - minZoomHeight) / (maxZoomHeight - minZoomHeight);
            cameraTransform.position = new Vector3(cameraPos.x, Mathf.Clamp(cameraPos.y + (1 + distanceRatio) * amount, minZoomHeight, maxZoomHeight), cameraPos.z);
            cameraTransform.LookAt(playerTransform.position + new Vector3(0, lookOverHeight * (1 - distanceRatio), 0));
    }

    public void RotateVertical(float amount)
    {
        if (amount != 0)
        {
            Quaternion playerRotation = playerTransform.rotation;
            transform.RotateAround(transform.position, Vector2.up, amount);
            playerTransform.rotation = playerRotation;
        }
    }

    public void ChangePosition(float lon, float lat)
    {
        Vector3 newPos = map.MapPositionAt(lon, lat);
        Vector3 direction = newPos - transform.position;
        playerTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        StartCoroutine(MovePlayer(newPos, 2f));
        UpdateMap();
    }

    public void Move(Vector3 direction)
    {
        if(direction.magnitude != 0)
        {
            transform.Translate(direction);
            //transform.position += direction;
            playerTransform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
            UpdateMap();
        }
    }

    private void UpdateMap()
    {
        if (Vector3.Distance(map.gameObject.transform.GetChild(0).InverseTransformPoint(transform.position), lastPosition) > refreshTreshold)
        {
            lastPosition = map.gameObject.transform.GetChild(0).InverseTransformPoint(transform.position);
            map.ShowMapArea(transform.position, radius);
        }
    }

    IEnumerator MovePlayer(Vector3 target, float overTime)
    {
        Vector3 source = transform.position;
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            transform.position = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        transform.position = target;
    }
}
