using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public enum UnitType
	{
		Sword,
		Spear,
		Axe,
		Cavalry,
		Archer
	}
	//Could probably do with Boolean, but if added escort missions or NPCs, may be better this way.
	public enum Alignment
	{
		Player,
		Enemy
	}
	[SerializeField] int count;
	int movementSpeed;
	[SerializeField] UnitType type;
	public Stats(UnitType type, int count = 0)
	{
		this.count = count;
		this.type = type;
	}
	void Start()
	{

	}
}
