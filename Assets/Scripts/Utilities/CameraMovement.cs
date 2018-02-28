using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    private int MARGIN = Screen.width / 100 * 10;
    private const float SPEED = 3f;
    private const float ZOOM_SPEED = 25f;
    private const int MAX_FOV_DEGREES = 1000;
    private const int MIN_FOV_DEGREES = 450;

    GameObject cameraObject;
    Camera camera;

	// Use this for initialization
	void Awake () {
        cameraObject = this.gameObject;
        camera = cameraObject.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftControl))
            return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 previousPosition = cameraObject.transform.position;
        Vector3 newPosition = previousPosition;

        //down
        if (mousePos.y < MARGIN)
        {
            newPosition.y = previousPosition.y - (MARGIN - mousePos.y) * Time.deltaTime * SPEED;
        }
        //up
        else if(mousePos.y > Screen.height - MARGIN)
        {
            newPosition.y = previousPosition.y - (Screen.height - MARGIN - mousePos.y) * Time.deltaTime * SPEED;
        }

        //left
        if (mousePos.x < MARGIN)
        {
            newPosition.x = previousPosition.x - (MARGIN - mousePos.x) * Time.deltaTime * SPEED;
        }
        //right
        else if (mousePos.x > Screen.width - MARGIN)
        {
            newPosition.x = previousPosition.x - (Screen.width - MARGIN - mousePos.x) * Time.deltaTime * SPEED;
        }

        cameraObject.transform.position = newPosition;


        //zoom
        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.01f || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKey(KeyCode.W))
            {
                camera.orthographicSize -= ZOOM_SPEED + Time.deltaTime;
            }
            else if((Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKey(KeyCode.S)))
                camera.orthographicSize += ZOOM_SPEED + Time.deltaTime;

            if (camera.orthographicSize > MAX_FOV_DEGREES)
                camera.orthographicSize = MAX_FOV_DEGREES;
            else if (camera.orthographicSize < MIN_FOV_DEGREES)
                camera.orthographicSize = MIN_FOV_DEGREES;
        }
	}
}
