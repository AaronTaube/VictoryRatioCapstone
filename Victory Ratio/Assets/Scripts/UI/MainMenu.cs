using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	GameObject mainPanel, optionsPanel;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void ShowOptions()
	{
		optionsPanel.SetActive(true);
		mainPanel.SetActive(false);
	}
	
	public void ReturnToMainMenu()
	{
		optionsPanel.SetActive(false);
		mainPanel.SetActive(true);
	}

}
