using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    int count;

    int movementSpeed;

    int defaultMovementSpeed = 3;
    int cavalryMovementSpeed = 5;

	/// <summary>
	/// All classes of unit in game. Will be referenced to find type advantages and other stats. 
	/// </summary>
    public enum UnitType
    {
        Sword,
        Spear,
        Axe,
        Cavalry,
        Archer
    }
	/// <summary>
	/// All alignments in game.
	/// Could probably do with Boolean, but if added escort missions or NPCs, may be better this way.
	/// </summary>
	public enum Alignment
    {
        Player,
        Enemy
    }

    [SerializeField]
    UnitType type;
    [SerializeField]
    Alignment alignment;
	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="count"></param>
	/// <param name="alignment"></param>
    public Unit(UnitType type, int count = 0, Alignment alignment = Alignment.Player)
    {
        this.count = count;
        this.type = type;
        this.alignment = alignment;
    }

	/// <summary>
	/// 
	/// </summary>
    void SetMovementSpeed()
    {
        switch (type)
        {
            case UnitType.Cavalry:
                movementSpeed = cavalryMovementSpeed;
                break;
            default:
                movementSpeed = defaultMovementSpeed;
                break;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
