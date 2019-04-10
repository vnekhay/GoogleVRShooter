using UnityEngine;
using System.Collections;

[RequireComponent (typeof(MoveTowards))]
[RequireComponent (typeof(RotateTowards))]
public class Target : MonoBehaviour
{
	public TargetZone[] targetZones;
	public GameObject geometryContainer;
	public GameObject destructionParticleContainer;
	public AudioSource hitClip;

	private TargetManager targetManager;
	private MoveTowards moveTowards;
	private RotateTowards rotateTowards;
	private GameObject player;
	private float pointsValueLoss;
	private Vector3 startPosition;
	private float pitchVariance = 0.2f;
	private float originalPitch;

	public TargetManager Targetmanager {
		set { targetManager = value; }
	}

	public GameObject Player {
		set { player = value; }
	}

	public void InitTarget ()
	{
		// Get components
		moveTowards = GetComponent<MoveTowards> ();
		rotateTowards = GetComponent<RotateTowards> ();

		// Set points value loss
		pointsValueLoss = targetManager.pointsValueLoss;

		// Set target transform
		moveTowards.target = player.transform;
		rotateTowards.target = player.transform;

		// Enable scripts
		moveTowards.enabled = true;
		rotateTowards.enabled = true;
	}

	public void Reset ()
	{
		// Add to inactive targets list
		targetManager.InactiveTargets.Enqueue (this);

		// Disable target
		gameObject.SetActive (false);
	}

	public void Activate ()
	{
		// Store starting position for points value loss calculations
		startPosition = transform.position;

		// Enable target
		gameObject.SetActive (true);
	}

	public void Hit (RaycastHit hit)
	{
		// Get points
		int points = GetPoints (hit.collider);

		StartCoroutine (Destroy ());
	}

	private IEnumerator Destroy ()
	{
		// Set random pitch and play audio
		hitClip.pitch = Random.Range (originalPitch - pitchVariance, originalPitch + pitchVariance);
		hitClip.Play ();

		// Disable geometry
		geometryContainer.SetActive (false);

		// Enable particles
		destructionParticleContainer.SetActive (true);

		// Total time for particles to finish
		float maxParticleDuration = 0;

		// Wait until particles have finished
		yield return new WaitForSeconds (maxParticleDuration);

		// Reset game object and hide it
		Reset ();

		yield return new WaitForEndOfFrame ();
	}

	private int GetPoints (Collider hitTargetZone)
	{
		foreach (TargetZone targetZone in targetZones) {
			if (targetZone.collider != hitTargetZone)
				continue;

			return CalculatePointLosses (targetZone.points);
		}

		return 0;
	}

	private int CalculatePointLosses (int pointsBase)
	{
		// Get distances to compare
		float startDistanceToTarget = Vector3.Distance (startPosition, moveTowards.StoppingPosition);
		float currentDistanceToTarget = Vector3.Distance (transform.position, moveTowards.StoppingPosition);

		// Get distance percentage
		float distancePercentage = (startDistanceToTarget * currentDistanceToTarget) / 100;
		distancePercentage = Mathf.Max (0, distancePercentage);

		// Hold minimum and maximum points values
		float maxPoints = pointsBase;
		float minPoints = maxPoints - (pointsBase * pointsValueLoss);

		// Calculate linear point loss
		float pointsValue = Mathf.Lerp (minPoints, maxPoints, distancePercentage);
		pointsValue = Mathf.Max (0, pointsValue);

		// Round to whole number and return  new points
		return Mathf.RoundToInt (pointsValue);
	}

	[System.Serializable]
	public struct TargetZone
	{
		public Collider collider;
		public int points;
	}
}