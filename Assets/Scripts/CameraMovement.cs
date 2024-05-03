using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    Transform mainTrans;

    [SerializeField] private Camera cornerCam;
    Transform cornerTrans;

    [SerializeField] private Transform target; // Position the cameras will orbit  

    private Vector3 previousPosition;

    private void Start()
    {
        mainTrans = mainCam.transform;
        mainTrans.position = target.position; 
        mainTrans.Translate(new Vector3(0, 0, -10));

        cornerTrans = cornerCam.transform;
        cornerTrans.position = target.position;
        cornerTrans.Translate(new Vector3(0, 0, -10));
        cornerTrans.eulerAngles = topdownAngles();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            previousPosition = mainCam.ScreenToViewportPoint(Input.mousePosition);
        }

        // While right mouse is still pressed update cameras' transforms based on the change of mouse position
        // Main camera rotates in a similar fashion to using your hand to spin a globe
        if (Input.GetMouseButton(1))
        {
            // Direction the mouse has moved
            Vector3 direction = previousPosition - mainCam.ScreenToViewportPoint(Input.mousePosition);

            mainTrans.position = target.position;
            // Bounds how high and low the main camera can view
            if (mainTrans.eulerAngles.x + direction.y * 180 > 5 && mainTrans.eulerAngles.x + direction.y * 180 < 85)
            {
                // Rotate around the local x axis by distance mouse moved in the y axis
                mainTrans.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            }
            // Rotate around the global y axis by distance mouse moved in the x axis
            mainTrans.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);

            mainTrans.Translate(new Vector3(0, 0, -10)); // Keeps constant distance away from target

            cornerTrans.eulerAngles = topdownAngles();

            previousPosition = mainCam.ScreenToViewportPoint(Input.mousePosition);
        }
    }
    // Returns the angles for a topdown view with same orientation as the main camera (birdseye view)
    Vector3 topdownAngles()
    {
        return new Vector3(90, mainTrans.eulerAngles.y, mainTrans.eulerAngles.z);
    }
}
