using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BallController : MonoBehaviour
{
	[SerializeField] Collider col;
	[SerializeField] Rigidbody rb;
	[SerializeField] float force;
	
	bool shoot;
	// Update is called once per frame
	void Update()
	{	
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		Debug.DrawRay(transform.position, ray.direction * 10000f, Color.red);
		if(Input.GetMouseButtonDown(0))
		{
			if(Physics.Raycast(ray,out hitInfo))
			{
				Debug.Log(hitInfo.collider.name);	
				if(hitInfo.collider == col)
					shoot = true;
			}
		}
	}
	
	private void FixedUpdate() 
	{
		if(shoot)
		{
			shoot = false;
			Vector3 direction = Camera.main.transform.forward; 
			direction.y = 0;
			rb.AddForce(direction * force, ForceMode.Impulse);
		}	
		
		if(rb.velocity.sqrMagnitude < 0.01f && rb.velocity.sqrMagnitude > -0.01f)
		{
			rb.velocity = Vector3.zero;
		}
	}
	
	public bool IsMove()
	{
		return rb.velocity != Vector3.zero;
	}
}
