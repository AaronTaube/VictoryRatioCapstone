using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
	[SerializeField]
	GameObject countPrefab;
	[SerializeField]
	UnitsManager unitManager;
	[SerializeField]
	GameObject allUnits;
	Transform playerUnits, enemyUnits, npcUnits;

	Dictionary<GameObject, GameObject> CountUnitPairs;

	List<GameObject> countInstances = new List<GameObject>();
	
    // Start is called before the first frame update
    void Start()
    {
		foreach(Transform child in allUnits.transform)
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
        
    }
	void SetAllUnitCounts()
	{
		foreach (Transform child in allUnits.transform)
		{
			foreach (Transform grandchild in child)
			{
				var unitScript = grandchild.GetComponent<Unit>();
				if (unitScript != null)
				{
					RenderCount(grandchild, unitScript.GetCount());
				}
			}
		}
	}
	void RenderCount(Transform unit, int count)
	{
		GameObject go = Instantiate(countPrefab, transform);
		go.transform.position = new Vector3(unit.position.x + .5f, unit.position.y + .5f, unit.position.z);
		go.GetComponentInChildren<TextMeshProUGUI>().SetText(count.ToString());
		countInstances.Add(go);

	}
	void ClearCounts()
	{
		foreach(GameObject go in countInstances)
		{
			Destroy(go);
		}
		countInstances = new List<GameObject>();
	}
}
