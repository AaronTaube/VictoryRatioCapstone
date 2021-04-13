using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[SerializeField]
	GameObject mainPanel, tutorialPanel, mathTipsPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void ShowInstructions()
	{
		tutorialPanel.SetActive(true);
		mainPanel.SetActive(false);
	}
	public void ShowMathTips()
	{
		mathTipsPanel.SetActive(true);
		mainPanel.SetActive(false);
	}
	public void ReturnToPauseMenu()
	{
		tutorialPanel.SetActive(false);
		mathTipsPanel.SetActive(false);
		mainPanel.SetActive(true);
	}
}
