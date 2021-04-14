using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Line", menuName = "Dialogue")]
public class DialogueLine : ScriptableObject
{
	[TextArea(20,20)]
	public string textLine;
}
