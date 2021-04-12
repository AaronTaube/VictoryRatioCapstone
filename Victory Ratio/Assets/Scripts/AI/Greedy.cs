using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greedy : MonoBehaviour
{
	private GameStateManager stateManager;
	private UnitsManager unitsManager;
	private BoardManager boardManager;
	private CombatManager combatManager;
	private Astar astar;

	private Queue<Unit> units;
	private Unit current;
	private Unit target;
	bool currentlyMoving;
	bool currentlyAttacking;
	// Start is called before the first frame update
	void Start()
    {
		stateManager = FindObjectOfType<GameStateManager>();
		unitsManager = FindObjectOfType<UnitsManager>();
		boardManager = FindObjectOfType<BoardManager>();
		combatManager = FindObjectOfType<CombatManager>();
		astar = FindObjectOfType<Astar>();
		stateManager.ai = GameStateManager.AIState.Waiting;
	}

    // Update is called once per frame
    void Update()
    {
		TakeAppropriateActions();
	}
	/// <summary>
	/// Filters gamestate and AIState to determine what action the AI should be doing in this moment, if any.
	/// </summary>
	void TakeAppropriateActions()
	{
		
		//Swap to player's turn if all units in queue have been used.
		if (stateManager.ai == GameStateManager.AIState.Switching && units.Count == 0)
		{
			SwitchTurn();
			return;
		}
		//Get the next unit to be moved and initiates movement
		if (stateManager.ai == GameStateManager.AIState.Switching && units.Count > 0)
		{
			unitsManager.PopulateUnitDicts();
			current = units.Dequeue();
			if (current == null)
				return;
			stateManager.ai = GameStateManager.AIState.Moving;
			
			StartCoroutine(MovementAction());
		}
		if (stateManager.ai == GameStateManager.AIState.SwitchingToAttack)
		{
			stateManager.ai = GameStateManager.AIState.Attacking;
			StartCoroutine(AttackAction());
		}
		//Should only run at the very start of the AI's turn
		if (stateManager.turn == GameStateManager.Turn.Enemy && stateManager.ai == GameStateManager.AIState.Waiting)
		{
			InitiateTurn();
			/*//Unit currentUnit;
			while (units.Count > 0)
			{
				unitsManager.PopulateUnitDicts();
				current = units.Dequeue();
				if (current == null)
					continue;
				yield return MovementAction();

				yield return AttackAction();
				yield return new WaitForEndOfFrame();//Makes sure race conditions are cleaned up
			}*/
		
		}



		
	}
	public void InitiateTurn()
	{
		
		units = new Queue<Unit>();
		GetAIUnits();
		stateManager.ai = GameStateManager.AIState.Switching;
		
	}
	void SwitchTurn()
	{
		stateManager.turn = GameStateManager.Turn.Player;
		stateManager.ai = GameStateManager.AIState.Waiting;
		boardManager.playerHasControl = true;
	}
	void GetAIUnits()
	{
		units = new Queue<Unit>();
		foreach (KeyValuePair<Vector3Int, Unit> unit in unitsManager.GetAllEnemyUnits())
		{
			units.Enqueue(unit.Value);
		}
	}
	
	IEnumerator MovementAction()
	{
		float elapsedTime = 0f;
		while (elapsedTime < .25f)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		target = FindTarget(current);
		boardManager.MoveEnemyUnit(current, target);
		
	}
	/// <summary>
	/// Could make this target the unit position instead
	/// </summary>
	/// <param name="unit"></param>
	/// <returns></returns>
	Unit FindTarget(Unit unit)
	{
		Unit result = null;
		int closestDistance = 0;
		int thisDistance;
		foreach(KeyValuePair<Vector3Int, Unit> playerUnit in unitsManager.GetAllPlayerUnits())
		{
			Stack<Vector3Int> path = astar.GetPath(unit.BoardPos, playerUnit.Key);
			if(closestDistance == 0)
			{
				closestDistance = path.Count;
				result = playerUnit.Value;
			}
			else
			{
				thisDistance = path.Count;
				if(thisDistance < closestDistance)
				{
					result = playerUnit.Value;
					closestDistance = thisDistance;
				}
			}
		}
		return result;
	}
	IEnumerator AttackAction()
	{
		
		Stack<Vector3Int> pathToTarget = astar.GetPath(current.BoardPos, target.BoardPos);
		
		if(pathToTarget.Count <= current.AttackRange)
		{
			yield return combatManager.Fight(current.BoardPos, target.BoardPos);
			
		}

		stateManager.ai = GameStateManager.AIState.Switching;
		//boardManager.CheckForVictoryOrDefeat();
	}
}
