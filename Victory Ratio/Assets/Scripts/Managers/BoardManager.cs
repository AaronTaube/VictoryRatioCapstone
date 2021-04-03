using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
	[Header("Board")]
	[SerializeField]
	private Tilemap movementBoard;
	[SerializeField]
	private Tile moveTile;
	[SerializeField]
	private Tile attackTile;


	[Header("Managers")]
	[SerializeField]
	UnitsManager unitsManager;
	[SerializeField]
	ValidMoves movesManager;
	[SerializeField]
	CombatManager combatManager;
	[SerializeField]
	private Astar pathfinder;

	[SerializeField]
	private Grid grid;

	[Header("Raycast Variables")]
	[SerializeField]
	private Camera camera;
	[SerializeField]
	private LayerMask boardMask;
	[SerializeField]
	private LayerMask moveOptionMask;

	private List<GameObject> movementTiles = new List<GameObject>();
	Dictionary<Vector3Int, Node> validMoves = new Dictionary<Vector3Int, Node>();
	Dictionary<Vector3Int, Node> validAttacks = new Dictionary<Vector3Int, Node>();

	//Unit info
	Vector3Int unitPos;
	Vector3Int targetPos;
	int unitMoveRange = 3;
	int unitAttackRange = 1;

	private bool playerHasControl = true;
	private GameStateManager stateManager;

	[Header("Game Config")]
	[SerializeField]
	float moveSpeed = .15f;

	private void Start()
	{
		//currently setting it to start as player turn and movement phase, will likely not be the case later.
		stateManager = FindObjectOfType<GameStateManager>();
		stateManager.phase = GameStateManager.GameState.MovementSelection;
		stateManager.turn = GameStateManager.Turn.Player;
		Debug.Log(stateManager.phase);
	}

	/// <summary>
	/// Checks player clicks to handle unit selection and actions. 
	/// </summary>
	void Update()
	{
		switch (stateManager.turn)
		{
			case GameStateManager.Turn.Player:
				PlayerTurnLogic();
				break;
			case GameStateManager.Turn.Enemy:
				EnemyTurnLogic();
				break;
			case GameStateManager.Turn.NPC:
				NPCTurnLogic();
				break;
			default:
				break;

		}
		
		
	}
	#region Turn States
	/// <summary>
	/// Directs logic based on gamestate
	/// </summary>
	void PlayerTurnLogic()
	{
		if (playerHasControl)
		{
			switch (stateManager.phase)
			{
				case GameStateManager.GameState.MovementSelection:
					RunMovementControls();
					break;
				case GameStateManager.GameState.AttackSelection:
					if (validAttacks.Count == 0) CreateAttackTiles(unitPos);
					RunAttackControls();
					break;
				default:
					break;
			}
		}
		
	}
	/// <summary>
	/// Will likely outsource to another class to handle AI
	/// </summary>
	void EnemyTurnLogic()
	{

	}
	/// <summary>
	/// Will likely outsource to another class to handle AI
	/// </summary>
	void NPCTurnLogic()
	{

	}
	#endregion

	#region Movement Code
	private void RunMovementControls()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D boardHit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, boardMask);
			RaycastHit2D movementHit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, moveOptionMask);

			if (boardHit.collider != null)
			{

				Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int clickPosition = movementBoard.WorldToCell(mouseWorldPos);
				if (validMoves.ContainsKey(clickPosition))
				{
					targetPos = clickPosition;
					Debug.Log("Unit at  " + unitPos + " target at " + targetPos);
					//Run method to start movement
					MoveUnit();
					return;
				}
				if (SelectableUnitClicked(clickPosition))
				{
					ResetMovementTiles();
					unitPos = clickPosition;
					CreateMovementTiles(clickPosition);
				}

			}
		}
	}
	/// <summary>
	/// Begins the process of units moving through the path, then resets the movement tiles
	/// </summary>
	private void MoveUnit()
	{
		Transform selectedUnit = unitsManager.GetPlayerUnit(unitPos).transform;
		StartCoroutine(MoveBetweenNodes(selectedUnit, moveSpeed));
		ResetMovementTiles();
	}
	/// <summary>
	/// Coroutine to run through Astar path one node at a time
	/// without halting the game. 
	/// Updates player dictionary at completion.
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="time"></param>
	/// <returns></returns>
	private IEnumerator MoveBetweenNodes(Transform unit, float time)
	{
		playerHasControl = false;
		ParticleSystem dustEmitter = unit.GetComponentInChildren<ParticleSystem>();
		dustEmitter.Play();
		Queue<Vector3Int> path = GetPath();
		while(path.Count > 0)
		{
			float elapsedTime = 0;
			Vector3 nextPos = path.Dequeue();
			Vector3 startPos = unit.position;
			Debug.Log("Next pos = " + nextPos);
			while (elapsedTime < time)
			{
				unit.position = Vector3.Lerp(startPos, nextPos, (elapsedTime / time));
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			
		}
		dustEmitter.Stop();
		EndOfMovementUpdates(unit);
		

	}
	/// <summary>
	/// Update unit positions and move to attack phase
	/// </summary>
	void EndOfMovementUpdates(Transform unit)
	{
		unitsManager.PopulateUnitDicts();
		unitPos = new Vector3Int(Mathf.RoundToInt(unit.position.x), Mathf.RoundToInt(unit.position.y), 0);
		stateManager.phase = GameStateManager.GameState.AttackSelection;
		Debug.Log(stateManager.phase);
		playerHasControl = true;
	}
	
		/// <summary>
		/// Calls our Astar algorithm to get the movement path and converts it to a form
		/// usable by our board manager for the purpose of moving the unit. 
		/// </summary>
		/// <returns></returns>
	Queue<Vector3Int> GetPath()
	{
		Stack<Vector3Int> tilemapPath = pathfinder.GetPath(unitPos, targetPos);
		Queue<Vector3Int> worldCoordPath = new Queue<Vector3Int>();
		while(tilemapPath.Count > 0)
		{
			Vector3Int worldPos = Vector3Int.RoundToInt(movementBoard.CellToWorld(tilemapPath.Pop()));
			worldCoordPath.Enqueue(worldPos);
		}
		return worldCoordPath;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	bool SelectableUnitClicked(Vector3Int position)
	{
		Dictionary<Vector3Int, Unit> allUnits = unitsManager.GetAllPlayerUnits();
		bool contain = allUnits.ContainsKey(position);
		if (!contain) return false;
		else
		{
			Unit unit = allUnits[position];
			unitAttackRange = unit.AttackRange;
			unitMoveRange = unit.MovementSpeed;
			if (unit.HasMoved) return false;
			else return true;
		}
	}
	/// <summary>
	/// Sets all tiles within range of a given unit position to be overlayed with a provided tile to indicate it is within range.
	/// Will later need to pull the unitMoveRange from the selected unit. 
	/// </summary>
	/// <param name="startPos"></param>
	public void CreateMovementTiles(Vector3Int startPos)
	{
		unitsManager.PopulateUnitDicts();
		validMoves = movesManager.GetValidMoves(startPos, unitMoveRange, unitAttackRange);
		validAttacks = movesManager.GetValidAttacks();
		RemoveOccupiedTiles(validMoves);
		foreach (var tile in validMoves)
		{
			Vector3Int pos = tile.Key;
			SetToMovementTile(pos);
		}
		foreach (var tile in validAttacks)
		{
			Vector3Int pos = tile.Key;
			SetToAttackTile(pos);
		}
		validMoves.Remove(unitPos);
		movementBoard.SetTile(unitPos, null);

	}
	public void SetToMovementTile(Vector3Int position)
	{
		movementBoard.SetTile(position, moveTile);
	}
	/// <summary>
	/// Clear board of movement tiles
	/// </summary>
	public void ResetMovementTiles()
	{
		foreach (var tile in validMoves)
		{
			movementBoard.SetTile(tile.Key, null);
		}
		foreach (var tile in validAttacks)
		{
			movementBoard.SetTile(tile.Key, null);
		}
		validMoves = new Dictionary<Vector3Int, Node>();
		validAttacks = new Dictionary<Vector3Int, Node>();

	}
	void RemoveOccupiedTiles(Dictionary<Vector3Int, Node> moves)
	{
		Dictionary<Vector3Int, Unit> units = unitsManager.GetAllPlayerUnits();
		foreach(KeyValuePair<Vector3Int, Unit> unit in units)
		{
			if (moves.ContainsKey(unit.Key))
				moves.Remove(unit.Key);
		}
		units = unitsManager.GetAllEnemyUnits();
		foreach (KeyValuePair<Vector3Int, Unit> unit in units)
		{
			if (moves.ContainsKey(unit.Key))
				moves.Remove(unit.Key);
		}
		units = unitsManager.GetAllNPCUnits();
		foreach (KeyValuePair<Vector3Int, Unit> unit in units)
		{
			if (moves.ContainsKey(unit.Key))
				moves.Remove(unit.Key);
		}
	}
	void SetUnitMoved()
	{

		unitsManager.GetUnit(unitPos).SetMoved();
	}
	#endregion

	#region Attack Code
	private void RunAttackControls()
	{

		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D boardHit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, boardMask);
			RaycastHit2D movementHit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, moveOptionMask);

			if (boardHit.collider != null)
			{

				Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int clickPosition = movementBoard.WorldToCell(mouseWorldPos);
				if (validAttacks.ContainsKey(clickPosition) && 
					unitsManager.GetAllEnemyUnits().ContainsKey(clickPosition))
				{
					targetPos = clickPosition;
					Debug.Log("Unit at  " + unitPos + " target at " + targetPos);
					StartCoroutine(AttackUnit());
					EndCombatPhase();

					return;
				}
				else if(unitPos == clickPosition)
				{
					EndCombatPhase();
				}
				

			}
		}
	}
	private IEnumerator AttackUnit()
	{
		ResetMovementTiles();
		yield return StartCoroutine(combatManager.Fight(unitPos, targetPos));
		
		//Debug.Log("ATTACK");

	}
	
	private void EndCombatPhase()
	{
		ResetMovementTiles();
		stateManager.phase = GameStateManager.GameState.MovementSelection;
		SetUnitMoved();
		if (!unitsManager.AnyPlayerMovesLeft())
		{
			unitsManager.ResetMoves();
			//TODO: Change to enemy phase
			stateManager.turn = GameStateManager.Turn.Enemy;
		}
	}
	public void CreateAttackTiles(Vector3Int startPos)
	{
		validAttacks = movesManager.GetValidAttacks(startPos, unitAttackRange);
		
		foreach (var tile in validAttacks)
		{
			Vector3Int pos = tile.Key;//validMoves.Pop().Position;
			SetToAttackTile(pos);
		}

		movementBoard.SetTile(unitPos, null);
	}
	
	public void SetToAttackTile(Vector3Int position)
	{
		movementBoard.SetTile(position, attackTile);
	}
	
	
	#endregion
	
}
