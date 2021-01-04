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

    [SerializeField]
    UnitType type;
    [SerializeField]
    Alignment alignment;

    public Unit(UnitType type, int count = 0, Alignment alignment = Alignment.Player)
    {
        this.count = count;
        this.type = type;
        this.alignment = alignment;
    }

 



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
