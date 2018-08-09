using UnityEngine;
using System.Collections;

public class MoonOrbit : MonoBehaviour {

	public Transform rotationPivot;
	public Vector3 rotationAxis;
	
	public float rotationSpeed = 1.0f;
	
	public void Update()
	{
		
		transform.RotateAround( rotationPivot.position, rotationAxis, rotationSpeed * Time.deltaTime);
		
	}
	
	public void OnDrawGizmos()
	{
		if( rotationPivot )
		{
			
			Gizmos.DrawRay( rotationPivot.position, rotationAxis * 5 );
			
		}
	
	}
}
