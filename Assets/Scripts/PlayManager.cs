using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
	[SerializeField] BallController ballController;
	[SerializeField] CameraController camController;

	// Update is called once per frame
	void Update()
	{
		var inputActive = Input.GetMouseButton(0) && ballController.IsMove() == false;
		camController.SetInputActive(inputActive);
	}
}
