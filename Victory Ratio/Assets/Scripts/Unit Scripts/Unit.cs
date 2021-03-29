using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[ExecuteAlways]
public class Unit : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField]
    int count;
	[SerializeField]
	UnitType type;

	public Alignment alignment { get; private set; }

	[Header("Sprites")]
	[SerializeField] Sprite notFound;
	[SerializeField] Sprite playerSpear, playerAxe, playerSword, playerArcher, playerCavalry;
	[SerializeField] Sprite enemySpear, enemyAxe, enemySword, enemyArcher, enemyCavalry;

	SpriteRenderer thisSpriteRenderer;

	UnitsManager unitManager;
	Tilemap tilemap;
	[Header("UI")]
	[SerializeField] Color baseColor;
	[SerializeField] Color movedColor;
	private Vector3Int boardPos;
	public Vector3Int BoardPos
	{
		get
		{
			boardPos = tilemap.WorldToCell(transform.position);
			return boardPos;
		}// Vector3Int mapPosition = tilemap.WorldToCell(childPos); }
		set
		{
			boardPos = tilemap.WorldToCell(transform.position);
		}
	}

	int movementSpeed;
	public bool hasMoved { get; set; } = false;

    int defaultMovementSpeed = 3;
    int cavalryMovementSpeed = 5;

	private void Start()
	{
		SetMovementSpeed();
		thisSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
		SetSprite();
		GameObject go = GameObject.FindGameObjectWithTag("Field");
		tilemap = go.GetComponent<Tilemap>();
		
	}
	// Update is called once per frame
	void Update()
	{
		//Only run SetSprite to load the map or on editor updates. No need to constantly run it in play.
		if (!Application.IsPlaying(gameObject))
		{
			SetSprite();
			AdjustPos();
		}
	}
	void SetSprite()
	{
		SetAlignment();
		switch (alignment)
		{
			case Alignment.Player:
				SetPlayerSprite();
				break;
			case Alignment.Enemy:
				SetEnemySprite();
				break;
			default:
				SetUnitErrorSprite();
				break;
		}
	}
	void SetPlayerSprite()
	{
		switch (type)
		{
			case UnitType.Spear:
				thisSpriteRenderer.sprite = playerSpear;
				break;
			case UnitType.Axe:
				thisSpriteRenderer.sprite = playerAxe;
				break;
			case UnitType.Sword:
				thisSpriteRenderer.sprite = playerSword;
				break;
			case UnitType.Archer:
				thisSpriteRenderer.sprite = playerArcher;
				break;
			case UnitType.Cavalry:
				thisSpriteRenderer.sprite = playerCavalry;
				break;
			default:
				SetUnitErrorSprite();
				break;
		}
	}
	void SetEnemySprite()
	{
		
		switch (type)
		{
			case UnitType.Spear:
				thisSpriteRenderer.sprite = enemySpear;
				break;
			case UnitType.Axe:
				thisSpriteRenderer.sprite = enemyAxe;
				break;
			case UnitType.Sword:
				thisSpriteRenderer.sprite = enemySword;
				break;
			case UnitType.Archer:
				thisSpriteRenderer.sprite = enemyArcher;
				break;
			case UnitType.Cavalry:
				thisSpriteRenderer.sprite = enemyCavalry;
				break;
			default:
				SetUnitErrorSprite();
				break;
		}
	}
	void SetUnitErrorSprite()
	{
		thisSpriteRenderer.sprite = notFound;
	}
	void SetAlignment()
	{
		string parentTag = transform.parent.tag;
		switch (parentTag)
		{
			case "PlayerUnits":
				alignment = Alignment.Player;
				break;
			case "EnemyUnits":
				alignment = Alignment.Enemy;
				break;
			case "NPCUnits":
				break;
			default:
				break;
		}
		
	}

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
	public int GetCount()
	{
		return count;
	}
	/// <summary>
	/// Snaps the unit to it's position in the grid.
	/// Currently used only in editor to keep developer from poorly placing units.
	/// May also be used after running lerp functions if the amount that lerp falls short 
	/// of final position by proves significant. If not, can probably make this private. 
	/// </summary>
	public void AdjustPos()
	{
		Vector3Int adjustedPos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);
		transform.position = adjustedPos;
	}
	public void TakeDamage()
	{
		if(count > 0)
			count--;
	}

	public void SetMoved()
	{
		hasMoved = true;
		thisSpriteRenderer.color = movedColor;
	}
	public void SetReady()
	{
		hasMoved = false;
		thisSpriteRenderer.color = baseColor;
	}
	public void Die()
	{
		
		Destroy(gameObject);
	}
}
