using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float zoomSpeed = 1f;
    public float panSpeed = 1f;

    private Camera cam;
    private Vector3 lastMousePosition;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        // Zooming
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, 1f, Mathf.Infinity);

        // Panning
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 delta = cam.ScreenToWorldPoint(lastMousePosition) - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.Translate(delta);
            lastMousePosition = Input.mousePosition;
        }
    }
}
