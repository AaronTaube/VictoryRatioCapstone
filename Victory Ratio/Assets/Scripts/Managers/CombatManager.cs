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
	[SerializeField]
	float swingSpeed = .25f;
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
	public IEnumerator Fight(Vector3 attackerPos, Vector3 defenderPos)
	{
		Unit attacker = unitsManager.GetUnit(attackerPos);
		Unit defender = unitsManager.GetUnit(defenderPos);
		double attackerStrength = CalculateStrength(attacker, defender.GetUnitType());
		double defenderStrenth = CalculateStrength(defender, attacker.GetUnitType());
		double odds = CalculateVictoryOdds(attackerStrength, defenderStrenth);
		bool hasAdvantage = RollAdvantage(odds);

		if (RangeAdvantage(attacker, defender))
		{
			yield return StartCoroutine(Attack(attacker, defender));
			if (defender.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(defender);
				
			}
			yield break;//Exit function without retaliation.
		}

		if (hasAdvantage)
		{
			//Attacker swings first
			yield return StartCoroutine(Attack(attacker, defender));
			//If unit destroyed, do death sequence instead
			if(defender.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(defender);
				yield break;//Exit function without retaliation.
			}
			//If alive, defender attacks
			yield return StartCoroutine(Attack(defender, attacker));
			if (attacker.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(attacker);
				Debug.Log(unitsManager.GetAllPlayerUnits().Count);
				yield break;//Exit function without retaliation.
			}
		}
		else
		{
			//Defender swings first
			yield return StartCoroutine(Attack(defender, attacker));
			if (attacker.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(attacker);
				yield break;
			}
			//if alive, attacker attacks
			yield return StartCoroutine(Attack(attacker, defender));
			if (defender.GetCount() <= 0)
			{
				//TODO death stuff
				UnitDestroyed(defender);
				yield break;
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
	private IEnumerator Attack(Unit damageDealer, Unit damageTaker)
	{
		float elapsedTime = 0.0f;
		Vector3 goalPosistion = new Vector3(((damageTaker.transform.position.x - damageDealer.transform.position.x) / 2) + damageDealer.transform.position.x,
												((damageTaker.transform.position.y - damageDealer.transform.position.y) / 2) + damageDealer.transform.position.y,
												damageDealer.transform.position.z);// damageTaker.transform.position;
		Vector3 startPos = damageDealer.transform.position;
		while (elapsedTime < swingSpeed)
		{
			damageDealer.transform.position = Vector3.Lerp(startPos, goalPosistion, (elapsedTime / swingSpeed));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		System.Random rand = new System.Random();
		for (int i = 0; i < damageDealer.GetCount(); i++)
		{
			if (rand.NextDouble() < damageChance)
				damageTaker.TakeDamage();
		}
		elapsedTime = 0.0f;
		while (elapsedTime < swingSpeed)
		{
			damageDealer.transform.position = Vector3.Lerp(goalPosistion, startPos, (elapsedTime / swingSpeed));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}
	/// <summary>
	/// TODO: Determine strength based on unit count and which tile the unit is in. 
	/// </summary>
	/// <param name="unitCount"></param>
	/// <returns></returns>
	private double CalculateStrength(Unit unit, Unit.UnitType opponentType)
	{
		double result;
		double typeAdvantage = 0;
		if(unit.HasTypeAdvantage(opponentType))
		{
			typeAdvantage = unit.GetCount() * .5;
		}
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
		return result + typeAdvantage;
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
	bool RangeAdvantage(Unit attacker, Unit defender)
	{
		if (defender.GetUnitType() == Unit.UnitType.Archer)
			return false;
		Vector3Int aPos = attacker.BoardPos;
		Vector3Int dPos = defender.BoardPos;
		int xDif = System.Math.Abs( aPos.x - dPos.x);
		int yDif = System.Math.Abs(aPos.y - dPos.y);
		int totalDif = xDif + yDif;
		if (totalDif < 2)
			return false;
		else
			return true;
	}
}
