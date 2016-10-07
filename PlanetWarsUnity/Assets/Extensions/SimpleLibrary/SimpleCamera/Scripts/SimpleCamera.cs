using System.Collections;
using UnityEngine;

public class SimpleCamera : MonoBehaviour
{
	private Camera cam;
	private Transform trans;
	private Transform oldTrans;

	public enum CameraMode
	{
		Orbit
	}

	public bool ShowAllSettings = true;

	[Header("General Settings")]
	public CameraMode CurrentCameraMode;

	private CameraMode LastCameraMode;

	public bool EnableInput = true;
	public bool Orbit_TouchInput = true;

	public enum OrbitPositionMode
	{
		POSITIONBASED,
		ROTATIONBASED
	}

	[Header("Orbit Settings")]
	public Vector3 OrbitCenter;

	public OrbitPositionMode CurrentOrbitPositionMode = OrbitPositionMode.ROTATIONBASED;

	[Range(0, 1f)]
	public float Orbit_RotationBasedSpeed = 0.1f;

	[Range(0, 1f)]
	public float Orbit_RotationBasedDamping = 0.1f;

	public string Orbit_HorizontalAxis = "Mouse X";
	public float OrbitMouseXSpeed = 10f;
	public string Orbit_VerticalAxis = "Mouse Y";
	public float OrbitMouseYSpeed = 10f;

	public float MinXRotation = 10;
	public float MaxXRotation = 80;

	public float MinYRotation = 10;
	public float MaxYRotation = 170;

	public float OrbitTouchXSpeed = 1f;
	public float OrbitTouchYSpeed = 1f;

	[Header("Orbit Position Constraint Settings")]
	public float MinOrbitY = -float.MaxValue;

	public bool OrbitCheckSightDown = true;
	public LayerMask CheckSightDownMask;
	public float OversightDown = 10f;
	public float OversightDownDistance = 1f;

	[Header("Orbit Zoom Settings")]
	public string Orbit_ZoomAxis = "Mouse ScrollWheel";

	public float OrbitMouseZSpeed = 40f;
	public float MinOrbitDistance = 2f;
	public float MaxOrbitDistance = 50f;
	public bool OrbitZoomSpeedBasedOnDistance = true;
	public float OrbitDistanceZoomMult = 3f;
	public float OrbitMinZoomSpeed = 0.05f;
	public float OrbitMaxZoomSpeed = 5f;

	public float Orbit_PinchZoomSpeed = 1f;

	[Header("Orbit Zoom Constraint Settings")]
	public bool OrbitCheckSight = true;

	public LayerMask CheckSightMask;
	public float SightDistance = 10f;
	public float Oversight = 10f;

	[Header("Orbit Look Rotation Settings")]
	public bool OrbitHoldMouseButtonForRotation = true;

	public int OrbitMouseButtonForRotation = 1;
	public bool OrbitSmoothStartLookAt = true;
	public float OrbitLookAtTime = 1f;
	public AnimationCurve LerpCurve;

	public bool OrbitLookAtUseSpeed = true;
	public float OrbitLookAtSpeed = 0.5f;
	public float OrbitMinLookRotation = 0.1f;

	private bool LookAtFinished = false;
	private float delta = 0f;
	private Vector3 Difference;
	private float distance = 0;
	private float LookAtTimer = 0f;

	private float WantedYRot = 0;
	private float WantedXRot = 0;

	private int LastTouchCount = 0;
	private int LastTouchCountDrag = 0;
	private Vector2 LastTouchCenter = Vector2.zero;

	public float ScreenValueX = 0.5f;

	public float DiffChangeSpeed = 0.2f;
	public Vector3 currentPos = Vector3.zero;

	public float CurrentDistance
	{
		get
		{
			wantedPosition = trans.position;
			position = wantedPosition;
			Difference = position - OrbitCenter;
			distance = Difference.magnitude;
			return distance;
		}
	}

	public Vector3 CurrentDifference
	{
		get
		{
			return Difference;
		}
	}

	private Vector3 wantedRotation = Vector3.zero;
	private Vector3 angularVelocity = Vector3.zero;

	private bool hashit = false;

	[Header("Position Transition")]
	public Vector3 TransitionPosTarget;

	public float TransitionSpeed = 0.2f;
	public float TransitionMinDistance = 0.05f;
	private Vector3 transitionStartPos;
	public bool WaitForRotationTransition = true;

	private void Awake()
	{
		cam = GetComponent<Camera>();
		trans = GetComponent<Transform>();
		position = trans.position;
		SetWantedPosition(position);

		oldTrans = new GameObject().transform;
	}

	public SimpleLibrary.Timer TransitionTimer;

	public void StartTransition(Vector3 pos)
	{
		transitionStartPos = trans.position;
		TransitionPosTarget = pos;

		currentVelocity = Vector3.zero;
		angularVelocity = Vector3.zero;
		TransitionTimer.Reset();
	}

	private float wantedDistance;

	private float lastWantedDistance = 10f;

	public void StartTransition(Vector3 orbit, float wantedDistances, bool forceDistance)
	{
		maxAngle = Quaternion.Angle(Quaternion.LookRotation(OrbitCenter - trans.position), Quaternion.LookRotation(orbit - trans.position));

		OrbitCenter = orbit;
		wantedDistance = CurrentDistance;

		if (wantedDistances < lastWantedDistance)
			wantedDistance *= 0.9f;
		else if (wantedDistances > lastWantedDistance)
			wantedDistance *= 1.1f;

		if (forceDistance)
			wantedDistance = wantedDistances;
		wantedDistance = Mathf.Clamp(wantedDistance, MinOrbitDistance, MaxOrbitDistance);

		lastWantedDistance = wantedDistances;

		Vector3 pos;
		Vector3 diff = OrbitCenter - trans.position;
		pos = OrbitCenter - diff.normalized * wantedDistance;

		LookAtFinished = false;
		LookAtTimer = 0f;
		StartTransition(pos);
	}

	public void UpdateCurrentRot()
	{
		Difference = position - OrbitCenter;
		wantedRotation = trans.eulerAngles;

		WantedXRot = wantedRotation.x + 90;
		WantedYRot = wantedRotation.y;

		if (wantedRotation.x < -360f)
			wantedRotation.x += 360f;
		if (wantedRotation.x > 360f)
			wantedRotation.x -= 360f;
		if (wantedRotation.y < -360f)
			wantedRotation.y += 360f;
		if (wantedRotation.y > 360f)
			wantedRotation.y -= 360f;

		if (wantedRotation.x < MinXRotation)
		{
			wantedRotation.x = MinXRotation;
		}
		if (wantedRotation.x > MaxXRotation)
		{
			wantedRotation.x = MaxXRotation;
		}
		if (wantedRotation.y < MinYRotation)
		{
			wantedRotation.y = MinYRotation;
		}
		if (wantedRotation.y > MaxYRotation)
		{
			wantedRotation.y = MaxXRotation;
		}
	}

	public void StartOrbit()
	{
		wantedPosition = trans.position;
		position = wantedPosition;
		Difference = position - OrbitCenter;
		distance = Difference.magnitude;
		wantedRotation = trans.eulerAngles;
		angularVelocity = Vector3.zero;
		UpdateCurrentRot();
	}

	public void SetScreenValueX(float value)
	{
		ScreenValueX = value;
	}

	private void Update()
	{
		position = transform.position;

		delta = Time.deltaTime / 0.016f;

		if (!TransitionTimer.Finished)
		{
			if (WaitForRotationTransition && !LookAtFinished)
				return;

			/*
			Vector3 difference = (TransitionPosTarget - trans.position);

			float maxDistance = Vector3.Distance(transitionStartPos, TransitionPosTarget);

			float proc = 1f;
			if (maxDistance > 0)
				proc = Mathf.Clamp01(difference.magnitude / maxDistance);

			Vector3 move = difference;
			move *= delta * TransitionSpeed;
			move *= (1f - Mathf.Clamp(proc, 0.501f, 0.999f)) * 2f;

			trans.position += move;

			if (Vector3.Distance(trans.position, TransitionPosTarget) <= TransitionMinDistance)
			{
				TransitionTimer.Finish();
				StartOrbit();
			}*/

			TransitionTimer.Update();
			trans.position = Vector3.Lerp(transitionStartPos, TransitionPosTarget, LerpCurve.Evaluate(TransitionTimer.Procentage));
			if (TransitionTimer.Finished)
				StartOrbit();
		}
		else
		{
			UpdateWantedPosition();
			UpdateMovement();
		}

		UpdateRotation();
	}

	private float lastDistance = 0.0f;
	public bool Orbit_NormalizedPinch = false;

	private void UpdateWantedPosition()
	{
		if (!EnableInput)
			return;
		if (CurrentCameraMode == CameraMode.Orbit)
		{
			float inputX = Input.GetAxis(Orbit_HorizontalAxis) * OrbitMouseXSpeed;
			float inputY = -Input.GetAxis(Orbit_VerticalAxis) * OrbitMouseYSpeed;
			float inputZ = -Input.GetAxis(Orbit_ZoomAxis) * OrbitMouseZSpeed;

			Vector2 touchCenter = Input.mousePosition;
			float zoomInput = 0f;

			if (Orbit_TouchInput)
			{
				//Touch Input
				if (Input.touchCount >= 2)
				{
					Vector2 touchZero;
					Vector2 touchOne;
					float pinchInput = 0f;
					float currentDistance = 0f;
					float distanceHere = 0;
					for (int index1 = 0; index1 < Input.touches.Length; index1++)
					{
						distanceHere = 0f;
						for (int index2 = 0; index2 < Input.touches.Length; index2++)
						{
							if (index1 == index2)
								continue;

							touchZero = Input.GetTouch(index1).position;
							touchOne = Input.GetTouch(index2).position;

							if (Orbit_NormalizedPinch)
							{
								touchZero.x /= (float)Screen.width;
								touchZero.y /= (float)Screen.height;
								touchOne.x /= (float)Screen.width;
								touchOne.y /= (float)Screen.height;
							}

							distanceHere += Vector2.Distance(touchZero, touchOne);
						}
						distanceHere /= Input.touchCount - 1;
						currentDistance += distanceHere;
					}
					currentDistance /= Input.touchCount;
					pinchInput = lastDistance - currentDistance;
					lastDistance = currentDistance;

					if (LastTouchCount == Input.touchCount)
					{
						zoomInput = pinchInput * Orbit_PinchZoomSpeed;
					}
					LastTouchCount = Input.touchCount;
				}
				else
				{
					LastTouchCount = 0;
					zoomInput = inputZ;
				}

				if (Input.touchCount >= 1)
				{
					inputX = 0f;
					inputY = 0f;
					touchCenter = Vector2.zero;
					foreach (var touch in Input.touches)
					{
						touchCenter += touch.position;
					}
					touchCenter /= (float)Input.touchCount;

					if (LastTouchCountDrag == Input.touchCount)
					{
						Vector2 diff = touchCenter - LastTouchCenter;
						inputX = diff.x * OrbitTouchXSpeed;
						inputY = -diff.y * OrbitTouchYSpeed;
					}
					LastTouchCenter = touchCenter;
					LastTouchCountDrag = Input.touchCount;
				}
				else
				{
					LastTouchCountDrag = 0;
					touchCenter = Input.mousePosition;
					if (OrbitHoldMouseButtonForRotation && !Input.GetMouseButton(OrbitMouseButtonForRotation))
					{
						inputX = 0;
						inputY = 0;
					}
				}
			}
			else
			{
				//Axis Input
				zoomInput = inputZ;
				touchCenter = Input.mousePosition;
				if (OrbitHoldMouseButtonForRotation && !Input.GetMouseButton(OrbitMouseButtonForRotation))
				{
					inputX = 0;
					inputY = 0;
				}
			}

			Difference = wantedPosition - OrbitCenter;

			if (OrbitZoomSpeedBasedOnDistance)
			{
				float zoomSpeed = Mathf.Clamp(Difference.magnitude, MinOrbitDistance, MaxOrbitDistance);
				zoomSpeed = Mathf.Clamp(zoomSpeed * OrbitDistanceZoomMult, OrbitMinZoomSpeed, OrbitMaxZoomSpeed);
				zoomInput *= zoomSpeed;
			}

			float minDistance = MinOrbitDistance;

			RaycastHit hit;

			hashit = false;
			if (OrbitCheckSight)
			{
				Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
				ray.origin -= ray.direction * Oversight;
				if (Physics.SphereCast(ray, 0.05f, out hit, SightDistance + Oversight, CheckSightMask))
				{
					minDistance = SightDistance + Vector3.Distance(OrbitCenter, hit.point);
					hashit = true;
				}
			}

			distance = Mathf.Clamp(distance + zoomInput, minDistance, MaxOrbitDistance);

			wantedPosition = OrbitCenter + Difference;

			wantedPosition = RotateAroundPoint(wantedPosition, OrbitCenter, Quaternion.Euler(Vector3.up * inputX + trans.right * inputY));

			wantedPosition.y = Mathf.Max(wantedPosition.y, OrbitCenter.y + MinOrbitY);
			if (OrbitCheckSightDown && Physics.Raycast(wantedPosition + Vector3.up * OversightDown, Vector3.down, out hit, OversightDown + OversightDownDistance, CheckSightDownMask))
			{
				wantedPosition.y = hit.point.y + OversightDownDistance;
			}

			if (CurrentOrbitPositionMode == OrbitPositionMode.POSITIONBASED)
			{
			}
			else if (CurrentOrbitPositionMode == OrbitPositionMode.ROTATIONBASED)
			{
				//WantedXRot += inputY;
				//WantedYRot += inputX;

				inputX /= Screen.width;
				inputY /= Screen.height;

				WantedXRot = Mathf.LerpAngle(WantedXRot, WantedXRot + inputY, 1f);
				WantedYRot = Mathf.LerpAngle(WantedYRot, WantedYRot + inputX, 1f);

				angularVelocity.x += inputY;
				angularVelocity.y += inputX;
			}
		}
	}

	public Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
	{
		return angle * (point - pivot) + pivot;
	}

	private float computeSpeed(float input, float procent, float multiply)
	{
		if ((input > 0 && procent < 0.5f) || (input < 0 && procent > 0.5f))
		{
			return input;
		}
		else
		{
			return input * multiply;
		}
	}

	private float computeBorderSpeed(float targetPos, float minPos, float maxPos, float borderSize)
	{
		float output = 0.0f;

		if (targetPos < (minPos + borderSize))
		{
			//Objekt ist auf Linker Seite (output 0 -> bordersize)
			output = Mathf.Clamp(targetPos - (minPos + borderSize), -borderSize, 0) + borderSize;
		}
		else if (targetPos > (maxPos - borderSize))
		{
			//Objekt ist auf Rechter Seite (output 0 -> bordersize)
			output = Mathf.Clamp(targetPos - (maxPos - borderSize), 0, borderSize) - borderSize;
		}
		else
		{
			output = borderSize;
		}
		output = Mathf.Clamp((Mathf.Abs(output) / borderSize), 0, 1);

		return output;
	}

	private void UpdateRotation()
	{
		if (CurrentCameraMode == CameraMode.Orbit)
		{
			Vector3 LookAtPos = OrbitCenter;
			if (LookAtFinished)
			{
				trans.LookAt(LookAtPos);
			}
			else
			{
				if (OrbitSmoothStartLookAt)
				{
					if (OrbitLookAtUseSpeed)
					{
						Quaternion targetRotation = Quaternion.LookRotation(LookAtPos - trans.position);
						angle = Quaternion.Angle(trans.rotation, targetRotation);

						float proc = 1f;
						if (maxAngle > 0)
							proc = Mathf.Clamp01(angle / maxAngle);

						float speed = angle / 180f;
						speed *= delta * OrbitLookAtSpeed;
						speed *= 1f - Mathf.Clamp(LerpCurve.Evaluate(proc), 0.05f, 0.95f);

						trans.rotation = Quaternion.Lerp(trans.rotation, targetRotation, speed);

						if (angle <= OrbitMinLookRotation)
						{
							trans.rotation = targetRotation;
							LookAtFinished = true;
						}
					}
					else
					{
						LookAtTimer += Time.deltaTime;
						float value = 1f;
						if (OrbitLookAtTime > 0f)
							value = Mathf.Clamp01(LookAtTimer / OrbitLookAtTime);
						Quaternion targetRotation = Quaternion.LookRotation(LookAtPos - trans.position);
						trans.rotation = Quaternion.Slerp(trans.rotation, targetRotation, LerpCurve.Evaluate(value));
						if (value >= 1f || Quaternion.Angle(trans.rotation, targetRotation) < OrbitMinLookRotation)
							LookAtFinished = true;
					}
				}
				else
				{
					LookAtFinished = true;
				}
			}
		}

		oldTrans.rotation = trans.rotation;
	}

	public float angle;
	public float maxAngle;

	public void SetOrbitCenter(Transform target)
	{
		SetOrbitCenter(target.position);
	}

	public void SetOrbitCenter(Vector3 position, bool instant = false)
	{
		OrbitCenter = position;
		Difference = trans.position - OrbitCenter;
		LookAtFinished = false;
		LookAtTimer = 0f;
		if (instant)
		{
			LookAtFinished = true;
			trans.rotation = Quaternion.LookRotation(OrbitCenter - trans.position);
		}
		wantedRotation = Quaternion.LookRotation(Difference).eulerAngles;
		angularVelocity = Vector3.zero;
	}

	#region Movement

	private Vector3 wantedPosition;

	public enum MovementMode
	{
		LERP,
		VELOCITY
	}

	[Header("Camera Movement Settings")]
	public MovementMode Mode = MovementMode.VELOCITY;

	[Header("Lerp Settings")]
	[Range(0.01f, 5f)]
	public float LerpSpeed = 1f;

	[Header("Velocity Settings")]
	[Range(0.01f, 5f)]
	public float Force = 2f;

	[Range(0f, 1f)]
	public float Dampening = 0.6f;

	[Range(0.01f, 25f)]
	public float MaximumSpeed = 5f;

	[Range(0f, 1000f)]
	public float SlowDownDistance = 20f;

	private Vector3 currentVelocity;

	public void SetWantedPosition(Vector3 position)
	{
		wantedPosition = position;
	}

	private Vector3 position;

	public void RepairAngle(ref float angle)
	{
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;
	}

	private void UpdateMovement()
	{
		if (CurrentCameraMode == CameraMode.Orbit && CurrentOrbitPositionMode == OrbitPositionMode.ROTATIONBASED)
		{
			Difference = position - OrbitCenter;
			float length = Mathf.Lerp(Difference.magnitude, distance, Mathf.Clamp01(delta * Orbit_RotationBasedSpeed) * (hashit ? 2f : 1f));

			angularVelocity -= Vector3.ClampMagnitude(angularVelocity * delta * 0.2f, angularVelocity.magnitude);

			Vector3 diff = angularVelocity * Time.deltaTime;

			wantedRotation = trans.eulerAngles;
			wantedRotation.x += 90;

			wantedRotation += diff;
			if (wantedRotation.x < -360f)
				wantedRotation.x += 360f;
			if (wantedRotation.x > 360f)
				wantedRotation.x -= 360f;
			if (wantedRotation.y < -360f)
				wantedRotation.y += 360f;
			if (wantedRotation.y > 360f)
				wantedRotation.y -= 360f;

			if (wantedRotation.x < MinXRotation)
			{
				wantedRotation.x = MinXRotation;
				angularVelocity.x = 0;
			}
			if (wantedRotation.x > MaxXRotation)
			{
				wantedRotation.x = MaxXRotation;
				angularVelocity.x = 0;
			}
			if (wantedRotation.y < MinYRotation)
			{
				wantedRotation.y = MinYRotation;
				angularVelocity.y = 0;
			}
			if (wantedRotation.y > MaxYRotation)
			{
				wantedRotation.y = MaxXRotation;
				angularVelocity.y = 0;
			}
			wantedRotation.x -= 90;

			Quaternion rotation = Quaternion.Euler(wantedRotation.x, wantedRotation.y, 0);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -length);
			position = rotation * negDistance + OrbitCenter;

			if (position.y < MinOrbitY)
			{
				position.y = MinOrbitY;
				//WantedXRot = currentRot.x;
			}

			//currentOrbitDiff = RotateAroundPoint(currentOrbitDiff, Vector3.zero, Quaternion.Euler(rotDiff));

			RaycastHit hit;
			if (OrbitCheckSightDown && Physics.Raycast(position + Vector3.up * OversightDown, Vector3.down, out hit, OversightDown + OversightDownDistance, CheckSightDownMask))
			{
				position.y = hit.point.y + OversightDownDistance;
				//WantedXRot = currentRot.x;
			}
		}
		else
		{
			if (Mode == MovementMode.LERP)
			{
				position = Vector3.Lerp(position, wantedPosition, Mathf.Clamp01(delta * LerpSpeed));
			}
			else if (Mode == MovementMode.VELOCITY)
			{
				Vector3 difference = wantedPosition - position;
				difference = delta * difference.normalized * Force * (Mathf.Min(difference.magnitude, SlowDownDistance) / (SlowDownDistance <= 0 ? 1f : SlowDownDistance));
				currentVelocity = Vector3.ClampMagnitude(currentVelocity + difference, MaximumSpeed);
				currentVelocity -= Vector3.ClampMagnitude(delta * currentVelocity * Dampening, currentVelocity.magnitude);
				position += delta * currentVelocity;
			}
		}

		trans.position = position;

		oldTrans.position = position;

		if (CurrentCameraMode == CameraMode.Orbit)
		{
			//trans.position = Vector3.Lerp(trans.position, trans.position + currentOrbitDiff, delta * 0.2f);
		}
	}

	#endregion Movement
}