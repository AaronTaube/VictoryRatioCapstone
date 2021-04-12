using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
	private SceneManager sceneManager;
	private BoardManager boardManager;
	//private UnitsManager unitsManager;
	[SerializeField]
	GameObject winScreen, loseScreen;
	[SerializeField]
	float endMenuDelay = 1.5f;
	public GameState phase { get; set; }
	public Turn turn { get; set; }
	public AIState ai { get; set; }
	public MatchState progress { get; set; }
	/// <summary>
	/// Which state of gameplay the game is in.
	/// </summary>
	public enum GameState
	{
		SceneOpen,
		MovementSelection,
		AttackSelection,
		MathIncentive,
		Combat,
		TurnSwitch

	}
	public enum MatchState
	{
		Win,
		Lose,
		InProgress
	}
	/// <summary>
	/// Which unit group is taking it's turn
	/// </summary>
	public enum Turn
	{
		Player,
		Enemy,
		NPC
	}

	public enum AIState
	{
		Waiting,
		Moving,
		SwitchingToAttack,
		Attacking,
		Switching
	}
	public bool EnemiesRemain { get; set; } = true;
	public bool PlayerUnitsRemain { get; set; } = true;
	// Start is called before the first frame update
	void Start()
    {
		sceneManager = FindObjectOfType<SceneManager>();
		boardManager = FindObjectOfType<BoardManager>();
		//unitsManager = FindObjectOfType<UnitsManager>();
		progress = MatchState.InProgress;
	}

    // Update is called once per frame
    void Update()
    {
        if(progress == MatchState.Lose)
		{
			StartCoroutine(Lose());
		}
		if(progress == MatchState.Win)
		{
			StartCoroutine(Win());
		}
		boardManager.CheckForVictoryOrDefeat();
    }

	IEnumerator Win()
	{
		//Delay
		float elapsedTime = 0f;
		while (elapsedTime < endMenuDelay)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		winScreen.SetActive(true);
		//Play victory music if added
	}
	IEnumerator Lose()
	{
		//Delay
		float elapsedTime = 0f;
		while (elapsedTime < endMenuDelay)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		loseScreen.SetActive(true);
		//Play defeat music if added

	}

}
