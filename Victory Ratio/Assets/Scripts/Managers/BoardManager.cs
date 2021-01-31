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

	[Header("Game Config")]
	[SerializeField]
	float moveSpeed = .15f;
	/// <summary>
	/// Checks player clicks to handle unit selection and actions. 
	/// </summary>
	void Update()
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
					Debug.Log("selectable");
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
		unitsManager.UpdatePlayerDict();

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
		return unitsManager.GetAllPlayerUnits().ContainsKey(position);
	}
	/// <summary>
	/// Sets all tiles within range of a given unit position to be overlayed with a provided tile to indicate it is within range.
	/// Will later need to pull the unitMoveRange from the selected unit. 
	/// </summary>
	/// <param name="startPos"></param>
	public void CreateMovementTiles(Vector3Int startPos)
	{
		validMoves = movesManager.GetValidMoves(startPos, unitMoveRange, unitAttackRange);
		validAttacks = movesManager.GetValidAttacks();
		foreach (var tile in validMoves)
		{
			Vector3Int pos = tile.Key;//validMoves.Pop().Position;
			SetToMovementTile(pos);
		}
		foreach (var tile in validAttacks)
		{
			Vector3Int pos = tile.Key;//validMoves.Pop().Position;
			SetToAttackTile(pos);
		}

		movementBoard.SetTile(unitPos, null);

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
	public void SetToMovementTile(Vector3Int position)
	{
		movementBoard.SetTile(position, moveTile);
	}
	public void SetToAttackTile(Vector3Int position)
	{
		movementBoard.SetTile(position, attackTile);
	}
	/// <summary>
	/// Clear board of movement tiles
	/// </summary>
	public void ResetMovementTiles()
	{
		foreach(var tile in validMoves)
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

}
