using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetManager : MonoBehaviour
{
	public GameObject targetPrefab;
	public float spawnDelay = 2f;
	public float timeBetweenSpawnsMin = 1f;
	public float timeBetweenSpawnsMax = 5f;
	public float spawnRadius = 10f;
	public float maxSpawnHeight = 40f;
	public int maxNumTargets = 20;

	[Range (0, 1), Tooltip ("How much % of point value is removed when target is at stopping distance (0 = 0%, 1 = 100%)")]
	public float pointsValueLoss;
	private List<Target> spawnedTargets = new List<Target> ();
	private Queue<Target> inactiveTargets = new Queue<Target> ();

	public Queue<Target> InactiveTargets {
		get { return inactiveTargets; }
	}

	void Awake ()
	{
		// Disable on game start because this is controlled by game manager
		this.enabled = false;
	}

	void OnEnable ()
	{
		StartCoroutine (SpawnTarget ());
	}

	void OnDisable ()
	{
		StopCoroutine (SpawnTarget ());
		ResetAllTargets ();
	}

	public void InitTargets ()
	{
		// Create target parent game object (for a cleaner outline)
		GameObject targetParent = new GameObject ();
		targetParent.name = "Targets";

		// Instantiate all targets
		for (int i = 0; i < maxNumTargets; i++) {
			Target targetInstance = (Instantiate (targetPrefab) as GameObject).GetComponent<Target> ();

			// Register target to manager
			targetInstance.Targetmanager = this;

			// Set parent
			targetInstance.transform.parent = targetParent.transform;

			// Initialize target
			targetInstance.InitTarget ();

			// Add to target lists
			spawnedTargets.Add (targetInstance);
		}

		ResetAllTargets ();
	}

	private IEnumerator SpawnTarget ()
	{
		// Wait before spawning
		yield return new WaitForSeconds (spawnDelay);

		// Spawning loop
		while (this.isActiveAndEnabled) {
			if (inactiveTargets.Count > 0) {
				// Get inactive target from queue
				Target target = inactiveTargets.Dequeue ();

				// Move target to position and make sure it is visible for the player
				Vector3 position;

				do {
					position = transform.position + Random.onUnitSphere * spawnRadius;
				} while(position.y < transform.position.y || position.y > maxSpawnHeight);

				target.transform.position = position;

				// Activate target
				target.Activate ();
			}

			// Get random wait time
			float waitTime = Random.Range (timeBetweenSpawnsMin, timeBetweenSpawnsMax);

			yield return new WaitForSeconds (waitTime);
		}
	}

	private void ResetAllTargets ()
	{
		// Clear targets queue
		inactiveTargets.Clear ();

		// Reset each target
		foreach (Target target in spawnedTargets) {
			target.Reset ();
		}
	}
}
