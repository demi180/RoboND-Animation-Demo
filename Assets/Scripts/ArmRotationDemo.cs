using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotationDemo : MonoBehaviour
{
	public Transform targetJoint;
	public Transform axesObject;
	public Transform[] tips;
	public Transform pivotPoint;
	public Space rotationSpace;
	public float guiOffset;
	public string refFrame;
	public Vector3 axisPosition;
	public SphereCollider pivotSphere;

	Transform sphereTransform;
	Camera cam;
	Quaternion startRotation;
	Vector3 xPos;
	Vector3 yPos;
	Vector3 zPos;

	string info = "";
	float infoTime;
	float infoEndTime;

	bool isPlayingSequence;
	bool visible;

	void OnEnable ()
	{
		sphereTransform = pivotSphere.transform;
	}

	void Awake ()
	{
//		StartCoroutine ( DoAnimation () );
		startRotation = targetJoint.rotation;
		cam = Camera.main;
	}

	void LateUpdate ()
	{
		if ( rotationSpace == Space.World )
			axesObject.rotation = Quaternion.identity;
		else
			axesObject.rotation = targetJoint.rotation;

		visible = cam.transform.InverseTransformPoint ( axesObject.position ).z >= 0.2f;
	}

	void OnDisable ()
	{
		Time.timeScale = 1;
	}

	public void PlayRotations (float[] angles, Vector3[] axes)
	{
		if ( !isPlayingSequence )
		{
			isPlayingSequence = true;
			StartCoroutine ( DoSequence ( angles, axes ) );
		}
	}

	public void ResetArm ()
	{
		StopAllCoroutines ();
		targetJoint.rotation = startRotation;
		isPlayingSequence = false;
	}

	public void SetRotationSpace (Space refSpace)
	{
		rotationSpace = refSpace;
	}

	IEnumerator DoSequence (float[] angles, Vector3[] axes)
	{
		Debug.Log ( "Starting sequence" );
		yield return new WaitForSeconds ( 1 );

		int count = angles.Length;
		float start;
		float end;
		float length = 3;
		float angle;
		Vector3 axis;
		Vector3 frameAngle;

		for ( int i = 0; i < count; i++ )
		{
			start = Time.time;
			end = start + length;
			angle = angles [ i ];
			axis = axes [ i ];
			if ( rotationSpace == Space.Self && pivotPoint != null )
				axis = targetJoint.TransformDirection ( axis );
//			if ( rotationSpace == Space.Self && pivotPoint == null )
//				axis = targetJoint.InverseTransformDirection ( axis );
			frameAngle = axis * angle / length;

			Debug.Log ( "starting rotation at " + start + " until " + end + " of " + angle + " degrees about " + axis );

			while ( Time.time < end )
			{
				if ( pivotPoint != null )
				{
					Vector3 point = pivotPoint.position;
					targetJoint.RotateAround ( point, axis, angle / length * Time.deltaTime );
				} else
					targetJoint.Rotate ( frameAngle * Time.deltaTime, rotationSpace );
				yield return null;
			}

			if ( i < count - 1 )
				yield return new WaitForSeconds ( 1 );
		}

		isPlayingSequence = false;
		Debug.Log ( "Finished sequence" );
	}

	public void SetPivotX (float value)
	{
		float radius = sphereTransform.localScale.x;
		Vector3 pos = pivotPoint.position;
		pos.x = sphereTransform.position.x + radius * ( value - 0.5f );
		pivotPoint.position = pos;
		axesObject.position = pivotPoint.position;
	}

	public void SetPivotZ (float value)
	{
		float radius = sphereTransform.localScale.y;
		Vector3 pos = pivotPoint.position;
		pos.y = sphereTransform.position.y + radius * ( value - 0.5f );
		pivotPoint.position = pos;
		axesObject.position = pivotPoint.position;
	}

	public void SetPivotY (float value)
	{
		float radius = sphereTransform.localScale.z;
		Vector3 pos = pivotPoint.position;
		pos.z = sphereTransform.position.z + radius * ( value - 0.5f );
		pivotPoint.position = pos;
		axesObject.position = pivotPoint.position;
	}

	public void ResetPivot ()
	{
		pivotPoint.position = sphereTransform.position;
		axesObject.position = pivotPoint.position;
	}

	void OnGUI ()
	{
		if ( !visible )
			return;
		
		GUIStyle label = GUI.skin.label;
		int fontSize = label.fontSize;
		FontStyle fontStyle = label.fontStyle;

		label.fontSize = (int) ( 42f * Screen.height / 1080 );
		label.fontStyle = FontStyle.Bold;
		Vector2 screenPos = cam.WorldToScreenPoint ( tips[0].position );
		screenPos.y = Screen.height - screenPos.y - 10;
		Vector2 size = new Vector2 ( 25, 25 );

		GUI.color = Color.black;
		GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "X" );
		GUI.color = Color.red;
		GUI.Label ( new Rect ( screenPos - size, size * 2 ), "X" );

		screenPos = cam.WorldToScreenPoint ( tips[1].position );
		screenPos.y = Screen.height - screenPos.y - 10;
		GUI.color = Color.black;
		GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "Y" );
		GUI.color = Color.green;
		GUI.Label ( new Rect ( screenPos - size, size * 2 ), "Y" );

		screenPos = cam.WorldToScreenPoint ( tips[2].position );
		screenPos.y = Screen.height - screenPos.y - 10;
		GUI.color = Color.black;
		GUI.Label ( new Rect ( screenPos - size + Vector2.one, size * 2 ), "Z" );
		GUI.color = Color.blue;
		GUI.Label ( new Rect ( screenPos - size, size * 2 ), "Z" );
	}
}