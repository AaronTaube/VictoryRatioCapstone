using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitsManager : MonoBehaviour
{
    [SerializeField]
    GameObject playerUnits;
    [SerializeField]
    GameObject enemyUnits;
    [SerializeField]
    GameObject npcUnits;

    [SerializeField]
    private Tilemap tilemap;
	
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private LayerMask mask;

    private Dictionary<Vector3Int, Unit> allPlayerUnits = new Dictionary<Vector3Int, Unit>();
	private Dictionary<Vector3Int, Unit> allEnemyUnits = new Dictionary<Vector3Int, Unit>();
	private Dictionary<Vector3Int, Unit> allNPCUnits = new Dictionary<Vector3Int, Unit>();
	// Start is called before the first frame update
	void Start()
    {
        PopulateUnitDicts();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, mask);

            if (hit.collider != null)
            {
                Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int clickPosition = tilemap.WorldToCell(mouseWorldPos);
                if (allPlayerUnits.ContainsKey(clickPosition))
                {
                    Debug.Log("hit");
                }
                else
                    Debug.Log(clickPosition);


            }
        }
    }
	/// <summary>
	/// Finds and begins to track all units on the map in dictionaries. 
	/// Will be modified as different unit control types are introduced. 
	/// </summary>
    void PopulateUnitDicts()
	{
		UpdatePlayerDict();
		UpdateEnemyDict();
		UpdateNPCDict();
        
    }
	/// <summary>
	/// Make sure all units under player control are tracked. 
	/// 
	/// Not necessarily the most performant way to update unit positions, 
	/// But cost should be negligible and makes for a nice, easy to maintain place to do it.
	/// </summary>
	public void UpdatePlayerDict()
	{
		allPlayerUnits = new Dictionary<Vector3Int, Unit>();
		foreach (Transform child in playerUnits.transform)
		{
			Vector3Int childPos = new Vector3Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y), 0);
			Vector3Int mapPosition = tilemap.WorldToCell(childPos);
			allPlayerUnits.Add(mapPosition, child.GetComponent<Unit>());
		}
	}
	public void UpdateEnemyDict()
	{
		allEnemyUnits = new Dictionary<Vector3Int, Unit>();
		foreach (Transform child in enemyUnits.transform)
		{
			Vector3Int childPos = new Vector3Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y), 0);
			Vector3Int mapPosition = tilemap.WorldToCell(childPos);
			allEnemyUnits.Add(mapPosition, child.GetComponent<Unit>());
		}
	}
	public void UpdateNPCDict()
	{
		allNPCUnits = new Dictionary<Vector3Int, Unit>();
		foreach (Transform child in npcUnits.transform)
		{
			Vector3Int childPos = new Vector3Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y), 0);
			Vector3Int mapPosition = tilemap.WorldToCell(childPos);
			allNPCUnits.Add(mapPosition, child.GetComponent<Unit>());
		}
	}
	public Dictionary<Vector3Int, Unit> GetAllPlayerUnits()
	{
		return allPlayerUnits;
	}
	public Dictionary<Vector3Int, Unit> GetAllEnemyUnits()
	{
		return allEnemyUnits;
	}
	public Dictionary<Vector3Int, Unit> GetAllNPCUnits()
	{
		return allNPCUnits;
	}
	public Unit GetPlayerUnit(Vector3Int cellPos)
	{
		return allPlayerUnits[cellPos];
	}
	public Unit GetUnit(Vector3 worldPos)
	{
		Vector3Int cellPos = tilemap.WorldToCell(worldPos);
		if (allPlayerUnits.ContainsKey(cellPos))
			return allPlayerUnits[cellPos];
		else if (allEnemyUnits.ContainsKey(cellPos))
			return allEnemyUnits[cellPos];
		else
			return allNPCUnits[cellPos];
	}
	public bool AnyPlayerMovesLeft()
	{
		foreach (var unit in allPlayerUnits)
		{
			if (!unit.Value.hasMoved)
				return true;
		}
		return false;
	}
	/// <summary>
	/// Reset unit tracking of if they've been moved this turn so next turn they can be moved again.
	/// </summary>
	public void ResetMoves()
	{
		foreach (var unit in allPlayerUnits)
		{
			unit.Value.hasMoved = false;
		}
	}
	
}
