﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AstarDebugger : MonoBehaviour
{
	private static AstarDebugger instance;

	public static AstarDebugger MyInstance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<AstarDebugger>();
			}

			return instance;
		}
	}
	[SerializeField]
	private Grid grid;

	[SerializeField]
	private Tilemap tilemap;

	[SerializeField]
	private Tile tile;



	[SerializeField]
	private Color openColor, closedColor, pathColor, currentColor, startColor, goalColor;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private GameObject debugTextPrefab;

	private List<GameObject> debugObjects = new List<GameObject>();

	public void CreateTiles(HashSet<Node> openList, HashSet<Node> closedList, Dictionary<Vector3Int, Node> allNodes, Vector3Int start, Vector3Int goal, Stack<Vector3Int> path = null)
	{
		foreach(Node node in openList)
		{
			ColorTile(node.Position, openColor);
		}

		foreach(Node node in closedList)
		{
			ColorTile(node.Position, closedColor);
		}

		if(path != null)
		{
			foreach(Vector3Int pos in path)
			{
				if(pos != start && pos != goal)
				{
					ColorTile(pos, pathColor);
				}
			}
		}

		ColorTile(start, startColor);
		ColorTile(goal, goalColor);

		foreach(KeyValuePair<Vector3Int, Node> node in allNodes)
		{
			if(node.Value.Parent != null)
			{
				GameObject go = Instantiate(debugTextPrefab, canvas.transform);
				go.transform.position = grid.CellToWorld(node.Key);
				debugObjects.Add(go);
				GenerateDebugText(node.Value, go.GetComponent<DebugText>());
			}
		}
	}

	private void GenerateDebugText(Node node, DebugText debugText)
	{
		debugText.P.text = $"P:{node.Position.x},{node.Position.y}";
		debugText.F.text = $"F:{node.F}";
		debugText.G.text = $"G:{node.G}";
		debugText.H.text = $"H:{node.H}";

		if (node.Parent.Position.x < node.Position.x && node.Parent.Position.y == node.Position.y)
		{
			debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
		}
		else if (node.Parent.Position.x < node.Position.x && node.Parent.Position.y > node.Position.y)
		{
			debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 135));
		}
		else if (node.Parent.Position.x < node.Position.x && node.Parent.Position.y < node.Position.y)
		{
			debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 225));
		}
		else if (node.Parent.Position.x > node.Position.x && node.Parent.Position.y == node.Position.y)
		{
			debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
		}
		else if (node.Parent.Position.x > node.Position.x && node.Parent.Position.y > node.Position.y)
		{
			debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 45));
		}
		else if (node.Parent.Position.x > node.Position.x && node.Parent.Position.y < node.Position.y)
		{
			debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, -45));
		}
		else if (node.Parent.Position.x == node.Position.x && node.Parent.Position.y > node.Position.y)
		{
			debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
		}
		else if (node.Parent.Position.x == node.Position.x && node.Parent.Position.y < node.Position.y)
		{
			debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 270));
		}
	}

	public void ColorTile(Vector3Int position, Color color)
	{
		tilemap.SetTile(position, tile);
		tilemap.SetTileFlags(position, TileFlags.None);
		tilemap.SetColor(position, color);
	}
	//Need to plug into debug UI button
	public void ShowHide()
	{
		canvas.gameObject.SetActive(!canvas.isActiveAndEnabled);
		Color c = tilemap.color;
		c.a = c.a != 0 ? 0 : 1;
		tilemap.color = c;
	}

	public void Reset()
	{
		foreach (GameObject go in debugObjects)
		{
			Destroy(go);
		}

		
	}
}
