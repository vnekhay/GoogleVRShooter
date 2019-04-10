using UnityEngine;

public class GUIManager : MonoBehaviour
{
	public GameObject mainMenu;
	public GameObject ingame;
	public GameObject gameOver;
	public GameObject playerCrosshair;
	public GameObject victory;

	void Start ()
	{
		ShowMainMenu ();
	}

	public void ShowMainMenu ()
	{
		mainMenu.SetActive (true);
		ingame.SetActive (false);
		victory.SetActive (false);


	}

	public void ShowIngameGUI ()
	{
		mainMenu.SetActive (false);
		ingame.SetActive (true);
		gameOver.SetActive (false);
		playerCrosshair.SetActive (true);
	}

	public void ShowGameOver ()
	{
		gameOver.SetActive (true);
		playerCrosshair.SetActive (false);
	}

	public void ShowVictory ()
	{
		ingame.SetActive (false);
		victory.SetActive (true);
	}
}
