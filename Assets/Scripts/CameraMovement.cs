using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform target; // Position the cameras will orbit  

    [SerializeField] private Camera cornerCam;

    private Vector3 previousPosition;

    private void Start()
    {
        // Initial positions of both cameras, ten units behind the target
        mainCam.transform.position = target.position; 
        mainCam.transform.Translate(new Vector3(0, 0, -10));

        cornerCam.transform.position = target.position;
        cornerCam.transform.Translate(new Vector3(0, 0, -10));

        // Initial rotation of the corner camera (birdseye view with same orientation as main camera)
        cornerCam.transform.eulerAngles = topdownAngles();
    }
    // Update is called once per frame
    void Update()
    {
        // Store the position of the mouse when right mouse is pressed
        if (Input.GetMouseButtonDown(1))
        {
            previousPosition = mainCam.ScreenToViewportPoint(Input.mousePosition);
        }

        // While right mouse is still pressed update cameras' transforms based on the change of mouse position
        // Main camera rotates in a similar fashion to you using your hand to spin a globe
        if (Input.GetMouseButton(1))
        {
            // Direction the mouse has moved
            Vector3 direction = previousPosition - mainCam.ScreenToViewportPoint(Input.mousePosition);

            
            mainCam.transform.position = target.position;
            // Bounds how high and low the main camera can view
            if (mainCam.transform.eulerAngles.x + direction.y * 180 > 5 && mainCam.transform.eulerAngles.x + direction.y * 180 < 85)
            {
                // Rotate around the local x axis by distance mouse moved in the y axis
                mainCam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            }
            // Rotate around the global y axis by distance mouse moved in the x axis
            mainCam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
            mainCam.transform.Translate(new Vector3(0, 0, -10));

            // Re-orient corner camera to match main cameras rotation from a topdown perspective
            cornerCam.transform.eulerAngles = topdownAngles(); 

            // Update previousPosition 
            previousPosition = mainCam.ScreenToViewportPoint(Input.mousePosition);
        }
    }
    // Returns the angles for a topdown view with same orientation as the main camera (birdseye view)
    Vector3 topdownAngles()
    {
        return new Vector3(90, mainCam.transform.eulerAngles.y, mainCam.transform.eulerAngles.z);
    }
}
