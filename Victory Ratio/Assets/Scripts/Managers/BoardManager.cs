using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
	[SerializeField]
	private Tilemap movementBoard;
	[SerializeField]
	private Tile moveTile;

	[SerializeField]
	private Tile[] tiles;

	[SerializeField]
	UnitsManager unitsManager;
	[SerializeField]
	ValidMoves movesManager;
	[SerializeField]
	private Astar pathfinder;

	[SerializeField]
	private Grid grid;

	[SerializeField]
	private Camera camera;

	[SerializeField]
	private LayerMask boardMask;
	[SerializeField]
	private LayerMask moveOptionMask;

	private List<GameObject> movementTiles = new List<GameObject>();
	Dictionary<Vector3Int, Node> validMoves = new Dictionary<Vector3Int, Node>();

	//Unit info
	Vector3Int unitPos;
	Vector3Int targetPos;
	int unitMoveRange = 3;
	
	/// <summary>
	/// Checks player clicks to handle unit selection and actions. 
	/// </summary>
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D boardHit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, boardMask);
			RaycastHit2D movementHit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, moveOptionMask);
			/*if (movementHit.collider != null)
			{
				Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int clickPosition = movementBoard.WorldToCell(mouseWorldPos);
				targetPos = clickPosition;
				Debug.Log("Unit at  " + unitPos + " target at " + targetPos);
				//Run method to start movement
				MoveUnit();
				return;
			}*/

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
	private void MoveUnit()
	{
		Transform selectedUnit = unitsManager.GetPlayerUnit(unitPos).transform;
		Queue<Vector3Int> path = GetPath();
		while(path.Count > 0)
		{
			selectedUnit.transform.position = path.Dequeue();
		}
		unitsManager.UpdatePlayerDict();
		ResetMovementTiles();
	}
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
		validMoves = movesManager.GetValidMoves(startPos, unitMoveRange);

		foreach (var tile in validMoves)
		{
			Vector3Int pos = tile.Key;//validMoves.Pop().Position;
			SetToMovementTile(pos);
		}
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
		foreach(var tile in validMoves)
		{
			movementBoard.SetTile(tile.Key, null);
		}
		validMoves = new Dictionary<Vector3Int, Node>();
		
	}
}
