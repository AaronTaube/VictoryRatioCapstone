using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatManager : MonoBehaviour
{
	[Header("Modifiers")]
	[SerializeField]
	double damageChance = .5;
	[SerializeField]
	double coverModifier = .5;

	[Header("Management")]
	[SerializeField]
	UnitsManager unitsManager;
	[SerializeField]
	Tilemap forestTiles;
    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	*/
	/// <summary>
	/// Initiates combat sequence
	/// </summary>
	/// <param name="attacker"></param>
	/// <param name="defender"></param>
	public void Fight(Vector3 attackerPos, Vector3 defenderPos)
	{
		Unit attacker = unitsManager.GetUnit(attackerPos);
		Unit defender = unitsManager.GetUnit(defenderPos);
		double attackerStrength = CalculateStrength(attacker);
		double defenderStrenth = CalculateStrength(defender);
		double odds = CalculateVictoryOdds(attackerStrength, defenderStrenth);
		bool hasAdvantage = RollAdvantage(odds);
		
		if (hasAdvantage)
		{
			//Attacker swings first
			Attack(attacker, defender);
			//If unit destroyed, do death sequence instead
			if(defender.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(defender);
				return;//Exit function without retaliation.
			}
			//If alive, defender attacks
			Attack(defender, attacker);
			if (attacker.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(attacker);
				Debug.Log(unitsManager.GetAllPlayerUnits().Count);
				return;//Exit function without retaliation.
			}
		}
		else
		{
			//Defender swings first
			Attack(defender, attacker);
			if (attacker.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(attacker);
				return;
			}
			//if alive, attacker attacks
			Attack(attacker, defender);
			if (defender.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(defender);
				return;
			}
		}
	}
	void UnitDestroyed(Unit dead)
	{
		
		unitsManager.RemoveUnit(dead);
		

		
		dead.Die();
		unitsManager.PopulateUnitDicts();
	}
	/// <summary>
	/// Deal damage to unit and play animations. May need to make this an IEnumerator or otherwise tie it to keyframes
	/// WIll likely see large restructuring
	/// </summary>
	/// <param name="damageDealer"></param>
	/// <param name="damageTaker"></param>
	private void Attack(Unit damageDealer, Unit damageTaker)
	{
		System.Random rand = new System.Random();
		for (int i = 0; i < damageDealer.GetCount(); i++)
		{
			if (rand.NextDouble() < damageChance)
				damageTaker.TakeDamage();
		}
	}
	/// <summary>
	/// TODO: Determine strength based on unit count and which tile the unit is in. 
	/// </summary>
	/// <param name="unitCount"></param>
	/// <returns></returns>
	private double CalculateStrength(Unit unit)
	{
		double result;
		if(forestTiles.HasTile(unit.BoardPos))
		{
			result = unit.GetCount() + (unit.GetCount()* coverModifier);
		}
		else
		{
			result = unit.GetCount();
		}
		Debug.Log("Unit pos = " + unit.BoardPos);
		Debug.Log("Unit strength = " + result + " Unit Count = " + unit.GetCount());
		return result;
	}
	/// <summary>
	/// Calculates chance of "Victory", which, in this context, 
	/// means the chance that this group of units will get to deal damage first.
	/// </summary>
	/// <param name="attackStrength"></param>
	/// <param name="defenseStrength"></param>
	/// <returns></returns>
	private double CalculateVictoryOdds(double attackStrength, double defenseStrength)
	{
		double victoryOdds = 0;
		double totalStrength = attackStrength + defenseStrength;
		victoryOdds = attackStrength / defenseStrength;
		return victoryOdds;
	}
	/// <summary>
	/// determines if the attacker wins advantage and swings first.
	/// </summary>
	/// <param name="odds"></param>
	/// <returns></returns>
	private bool RollAdvantage(double odds)
	{
		System.Random rand = new System.Random();
		double roll = rand.NextDouble();
		if (roll < odds)
			return true;
		else
			return false;

	}
}
