using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MathIncentive : MonoBehaviour
{
	[SerializeField]
	BoardManager boardManager;
	[SerializeField]
	CombatManager combatManager;
	[SerializeField]
	TMP_InputField leftSide, rightSide, solution;
	[SerializeField]
	GameObject mathCanvas;
	[SerializeField]
	Button submitButton;
	Vector3Int playerPos, targetPos;
	[SerializeField]
	float answerDisplayTime = 1.5f;
	[SerializeField]
	float postMathDelay = .5f;
	[SerializeField]
	Color green, red, yellow;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		//Debug.Log(leftSide.text);
		//Debug.Log("Success");
		if (double.TryParse(leftSide.text, out double leftNum) && double.TryParse(rightSide.text, out double rightNum))
		{
			PresentPercentage(leftNum, rightNum);
		}
	}
	public void AnswerSubmitted()
	{
		submitButton.interactable = false;
		//Check if the player has the correct answer
		if (double.TryParse(leftSide.text, out double leftNum) && double.TryParse(rightSide.text, out double rightNum))
		{
			playerPos = boardManager.GetPlayerUnitPos();
			targetPos = boardManager.GetPlayerTargetPos();
			double ratio = GetEnteredRatio(leftNum, rightNum);
			double expected = combatManager.GetOdds(playerPos, targetPos);
			if (Math.Abs(ratio - expected) < .0005)//a rough tolerance for doubles to be not quite equal in system
				StartCoroutine(CorrectAnswer());
			else
				StartCoroutine(WrongAnswer());
		}
		//Assume the player has skipped and proceed with combat
		else
		{
			StartCoroutine(NormalAttack());

		}
		
	}
	void PresentPercentage(double left, double right)
	{
		double result = GetEnteredRatio(left, right);
		solution.text = result.ToString("0%");//("P", CultureInfo.InvariantCulture);
	}
	double GetEnteredRatio(double left, double right)
	{
		double result = left / right;
		return result;
	}
	IEnumerator CorrectAnswer()
	{
		float elapsedTime = 0f;
		SetTextGreen();
		while (elapsedTime < answerDisplayTime)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		yield return BoostedAttack();
	}
	IEnumerator WrongAnswer()
	{
		float elapsedTime = 0f;
		SetTextRed();
		while (elapsedTime < answerDisplayTime)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		yield return NormalAttack();
	}
	IEnumerator BoostedAttack()
	{
		ExitMathPhase();
		float elapsedTime = 0f;
		while (elapsedTime < postMathDelay)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		yield return combatManager.FreeSwing(playerPos, targetPos);
		yield return NormalAttack();
	}
	IEnumerator NormalAttack()
	{
		ExitMathPhase();
		float elapsedTime = 0f;
		while (elapsedTime < postMathDelay)
		{
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		yield return boardManager.AttackCoroutine();
		
	}
	void ExitMathPhase()
	{
		ResetMath();
		SetTextYellow();
		submitButton.interactable = true;
		mathCanvas.SetActive(false);
	}
	void ResetMath()
	{
		leftSide.text = "";
		rightSide.text = "";
		solution.text = "";
	}
	void SetTextRed()
	{
		leftSide.textComponent.color = red;
		rightSide.textComponent.color = red;
		solution.textComponent.color = red;
	}
	void SetTextGreen()
	{
		leftSide.textComponent.color = green;
		rightSide.textComponent.color = green;
		solution.textComponent.color = green;
	}
	void SetTextYellow()
	{
		leftSide.textComponent.color = yellow;
		rightSide.textComponent.color = yellow;
		solution.textComponent.color = yellow;
	}
}
