﻿using UnityEngine;
public class CameraControl : MonoBehaviour
{
	public void SetPlayerCamera(bool isPlayerOne = true) {
		Vector3 position, rotation;
		if(isPlayerOne) {
			position = new Vector3(22,22,-14);
			rotation = new Vector3(50,0,0);
		} else {
			position = new Vector3(60,22,-14);
			rotation = new Vector3(50,0,0);
		}
		iTween.MoveTo(Camera.main.gameObject, iTween.Hash("position", position,
			"delay", 0.5f,
			"easetype", "easeInQuad",
			"time", 5));
		iTween.RotateTo(Camera.main.gameObject, iTween.Hash("rotation", rotation,
			"delay", 0.5f,
			"easetype", "easeInQuad",
			"time", 5));
	}
	
	void Update()
		{
        // Init camera translation for this frame.
        var translation = Vector3.zero;
		var zoomDelta = Input.GetAxis("Mouse ScrollWheel")*Settings.ZoomSpeed*Time.deltaTime;
	
        if (zoomDelta!=0)
        {
            translation -= Vector3.up * Settings.ZoomSpeed * zoomDelta;
        }
	
		// Move camera with arrow keys
        translation += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	
	 	// Move camera if mouse pointer reaches screen borders
        if (Input.mousePosition.x < Settings.ScrollArea)
        {
            translation += Vector3.right * -Settings.ScrollSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.x >= Screen.width - Settings.ScrollArea)
        {
            translation += Vector3.right * Settings.ScrollSpeed * Time.deltaTime;
        }
	
        if (Input.mousePosition.y < Settings.ScrollArea)
        {
            translation += Vector3.forward * -Settings.ScrollSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.y > Screen.height - Settings.ScrollArea)
        {
            translation += Vector3.forward * Settings.ScrollSpeed * Time.deltaTime;
        }

		// Keep camera within level and zoom area
		if(GameControl.GameStarted()) {
            var desiredPosition = Camera.main.transform.position + translation;
            if (desiredPosition.x < 15 || Settings.LevelArea+15 < desiredPosition.x)
            {
                translation.x = 0;
            }
            if (desiredPosition.y <Settings.ZoomMin || Settings.ZoomMax < desiredPosition.y)
            {
                translation.y = 0;
            }
			if (desiredPosition.z < -15 || Settings.LevelArea-25 < desiredPosition.z)
            {
                translation.z = 0;
            }
		}

        // Finally move camera parallel to world axis
		if(GameControl.GameStarted()) {
        	Camera.main.transform.position += translation;
		}
    }
}