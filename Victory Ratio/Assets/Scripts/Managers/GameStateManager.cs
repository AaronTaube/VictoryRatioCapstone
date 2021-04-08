using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
	public GameState phase { get; set; }
	public Turn turn { get; set; }
	public AIState ai { get; set; }
	/// <summary>
	/// Which state of gameplay the game is in.
	/// </summary>
	public enum GameState
	{
		SceneOpen,
		MovementSelection,
		AttackSelection,
		Combat,
		TurnSwitch
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
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
