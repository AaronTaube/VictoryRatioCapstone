using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
	[SerializeField]
	DialogueLine[] sceneDialogue;
	[SerializeField]
	TextMeshProUGUI dialogueLine;

	MusicPlayer music;
	GameStateManager stateManager;
	int size;
	int index;
    // Start is called before the first frame update
    void Start()
    {
		stateManager = FindObjectOfType<GameStateManager>();
		index = 1;
		dialogueLine.text = sceneDialogue[0].textLine;
		music = FindObjectOfType<MusicPlayer>();

    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			if (index < sceneDialogue.Length)
			{
				dialogueLine.text = sceneDialogue[index].textLine;
				index++;
			}
			else
				EndDialogue();
		}
    }

	void EndDialogue()
	{
		stateManager.turn = GameStateManager.Turn.Player;
		music.PlayNext();
		gameObject.SetActive(false);
	}
}
