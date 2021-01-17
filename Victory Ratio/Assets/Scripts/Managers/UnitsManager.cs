﻿using System.Collections;
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
        /*foreach (Transform child in enemyUnits.transform)
        {
            Vector3Int intPosition = Vector3Int.FloorToInt(child.position);
            allPlayerUnits.Add(intPosition, child.GetComponent<Unit>());
        }
        foreach (Transform child in npcUnits.transform)
        {
            Vector3Int intPosition = Vector3Int.FloorToInt(child.position);
            allPlayerUnits.Add(intPosition, child.GetComponent<Unit>());
        }*/
        foreach (KeyValuePair<Vector3Int, Unit> unit in allPlayerUnits)
        {
            Debug.Log(unit.Key);

        }
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
	public Dictionary<Vector3Int, Unit> GetAllPlayerUnits()
	{
		return allPlayerUnits;
	}
	public Unit GetPlayerUnit(Vector3Int cellPos)
	{
		return allPlayerUnits[cellPos];
	}
}
