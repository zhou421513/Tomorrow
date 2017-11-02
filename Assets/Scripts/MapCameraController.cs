﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform cameraTransform;

    [Range(0, 5)]
    public float lookOverHeight;

    [Range(0, 10)]
    public float minZoomHeight;

    [Range(10, 100)]
    public float maxZoomHeight;

    [Range(0, 50)]
    public float cameraDistance;

    private Transform playerTransform;

    //Disable this script if no camera is set or if no player prefab is given
    //Set the cameras position and rotation
    void OnEnable()
    {
        if (cameraTransform == null || playerPrefab == null)
        {
            enabled = false;
            return;
        }

        if (playerTransform == null)
            playerTransform = Instantiate(playerPrefab, transform).transform;

        cameraTransform.SetParent(transform);
        cameraTransform.position = playerTransform.position - playerTransform.forward * cameraDistance + new Vector3(0, minZoomHeight + (maxZoomHeight - minZoomHeight) / 2, 0);
        Zoom(0);

		InputManager.Instance.pinch += Zoom;
		InputManager.Instance.rotate += RotateVertical;
    }

	void OnDisable()
	{
		InputManager.Instance.pinch -= Zoom;
		InputManager.Instance.rotate -= RotateVertical;
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
        cameraTransform.RotateAround(playerTransform.position, Vector2.up, amount);//.Rotate(Vector2.up * amount);
    }

    public void MoveHorizontal(Vector2 direction)
    {
		transform.Translate(Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized * direction.y);
		transform.Translate(Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized * direction.x);
    }
}