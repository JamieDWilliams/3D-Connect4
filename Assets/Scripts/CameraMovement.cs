using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera cornerCam;
    [SerializeField] private Transform target;

    private Vector3 previousPosition;

    private void Start()
    {
        mainCam.transform.position = target.position;
        mainCam.transform.Translate(new Vector3(0, 0, -10));

        cornerCam.transform.position = target.position;
        cornerCam.transform.Translate(new Vector3(0, 0, -10));
        cornerCam.transform.eulerAngles = new Vector3(90, mainCam.transform.eulerAngles.y, mainCam.transform.eulerAngles.z);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            previousPosition = mainCam.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 direction = previousPosition - mainCam.ScreenToViewportPoint(Input.mousePosition);

            mainCam.transform.position = target.position;

            mainCam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            mainCam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
            mainCam.transform.Translate(new Vector3(0, 0, -10));

            cornerCam.transform.eulerAngles = new Vector3(90, mainCam.transform.eulerAngles.y, mainCam.transform.eulerAngles.z);

            previousPosition = mainCam.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}
