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
	private Grid grid;

	[SerializeField]
	private Camera camera;

	[SerializeField]
	private LayerMask mask;

	private List<GameObject> movementTiles = new List<GameObject>();

	int unitMoveRange = 3;
	/// <summary>
	/// Checks player clicks to handle unit selection and actions. 
	/// </summary>
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, mask);

			if (hit.collider != null)
			{
				Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int clickPosition = movementBoard.WorldToCell(mouseWorldPos);
				if (SelectableUnitClicked(clickPosition))
				{
					CreateMovementTiles(clickPosition);
				}
				
			}
		}
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
		Stack<Node> validMoves = movesManager.GetValidMoves(startPos, unitMoveRange);
		
		while(validMoves.Count > 0)
		{
			SetToMovementTile(validMoves.Pop().Position);
		}

	}
	public void SetToMovementTile(Vector3Int position)
	{
		movementBoard.SetTile(position, moveTile);
	}
	
	public void Reset()
	{
		foreach (GameObject go in movementTiles)
		{
			Destroy(go);
		}
	}
}
