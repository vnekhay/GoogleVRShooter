using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	public LayerMask targetLayerMask;
	public AudioSource shootingClip;
	public Slider reloadIndicator;

	private Rect inputRect;
	private Camera cam;
	private float pitchVariance = 0.1f;

	void Start ()
	{
		inputRect = new Rect (Screen.width / 2, 0, Screen.width, Screen.height * 0.75f);
		cam = GetComponentInChildren<Camera> ();
	}

	void OnEnable ()
	{
		
	}

	void Update ()
	{
		TouchInput ();
	}

	private void TouchInput ()
	{
		if (Input.touchCount > 0) {
			foreach (Touch touch in Input.touches) {
				if (touch.phase != TouchPhase.Began)
					continue;

				if (!inputRect.Contains (touch.position))
					continue;

				Shoot ();
			}
		}
	}

	#if UNITY_EDITOR
	private void KeyboardInput ()
	{
		if (Input.GetButtonDown ("Fire1")) {
			Shoot ();
		}
	}
	#endif

	private void Shoot ()
	{
		// Only shoot when clip has finished playing
		if (shootingClip.isPlaying)
			return;

		StartCoroutine (ReloadIndicator ());
		// Check for target
		RaycastHit hit;
		if (Physics.Raycast (cam.transform.position, cam.transform.forward, out hit, 100f, targetLayerMask)) {
			Target target = hit.collider.GetComponentInParent<Target> ();
			target.Hit (hit);
		}
	}

	private IEnumerator ReloadIndicator ()
	{
		reloadIndicator.gameObject.SetActive (true);

		do {
			float t = shootingClip.time / shootingClip.clip.length;
			reloadIndicator.value = t;

			yield return new WaitForEndOfFrame ();
		} while(shootingClip.isPlaying);

		reloadIndicator.gameObject.SetActive (false);
	}
}
