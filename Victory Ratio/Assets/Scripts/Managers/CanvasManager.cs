using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//[ExecuteAlways]
public class CanvasManager : MonoBehaviour
{
	[SerializeField]
	GameObject countPrefab;
	[SerializeField]
	UnitsManager unitManager;
	[SerializeField]
	GameObject allUnits;
	Transform playerUnits, enemyUnits, npcUnits;

	Dictionary<GameObject, GameObject> countUnitPairs;

	List<GameObject> countInstances = new List<GameObject>();
	
    // Start is called before the first frame update
    void Start()
    {
		ClearCounts();
		countUnitPairs = new Dictionary<GameObject, GameObject>();
		foreach (Transform child in allUnits.transform)
		{
			if (child.CompareTag("PlayerUnits"))
			{
				playerUnits = child;
			}
			if (child.CompareTag("EnemyUnits"))
			{
				enemyUnits = child;
			}
			if (child.CompareTag("NPCUnits"))
			{
				npcUnits = child;
			}
		}
		SetAllUnitCounts();
    }
	
    // Update is called once per frame
    void Update()
    {
		CountsFollowUnits();
		UpdateUnitCounts();//May make this called as needed later.
    }
	/// <summary>
	/// 
	/// </summary>
	void SetAllUnitCounts()
	{
		foreach (Transform child in allUnits.transform)
		{
			foreach (Transform grandchild in child)
			{
				var unitScript = grandchild.GetComponent<Unit>();
				if (unitScript != null)
				{
					MakeUnitCountPair(grandchild, unitScript.GetCount());
				}
			}
		}
	}
	void UpdateUnitCounts()
	{
		foreach(var pair in countUnitPairs)
		{
			Unit thisUnit = pair.Key.GetComponent<Unit>();
			TextMeshProUGUI thisCount = pair.Value.GetComponentInChildren<TextMeshProUGUI>();
			thisCount.text = thisUnit.GetCount().ToString();
		}
	}
	/// <summary>
	/// Uses dictionary to link canvas count objects to the units,
	/// as well as tracking each in a list.
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="count"></param>
	void MakeUnitCountPair(Transform unit, int count)
	{
		GameObject countDisplay = Instantiate(countPrefab, transform);
		countDisplay.transform.position = new Vector3(unit.position.x + .5f, unit.position.y + .5f, unit.position.z);
		countDisplay.GetComponentInChildren<TextMeshProUGUI>().SetText(count.ToString());
		countUnitPairs.Add(unit.gameObject, countDisplay);
		countInstances.Add(countDisplay);

	}
	/// <summary>
	/// Keeps the canvas count objects following the units even in editor mode. 
	/// </summary>
	void CountsFollowUnits()
	{
		foreach(var pair in countUnitPairs)
		{
			pair.Value.transform.position = pair.Key.transform.position + new Vector3(.5f, .5f, 0f);
		}
	}
	/// <summary>
	/// 
	/// </summary>
	void ClearCounts()
	{
		foreach(GameObject go in countInstances)
		{
			Destroy(go);
		}
		countInstances = new List<GameObject>();
	}
}
