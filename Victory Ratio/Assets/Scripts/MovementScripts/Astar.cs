using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { GRASS,WATER,FOREST}

public class Astar : MonoBehaviour
{
	int clickCount = 0;


	private TileType tileType;

	[SerializeField]
	private Tilemap tilemap;

	[SerializeField]
	private Tile[] tiles;

	[SerializeField]
	UnitsManager unitsManager;

	[SerializeField]
	private Camera camera;

	[SerializeField]
	private LayerMask mask;

	private Vector3Int startPos, goalPos;
	private Node current;
	private HashSet<Node> openList;
	private HashSet<Node> closedList;
	private Stack<Vector3Int> path;

	private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();

    // Update is called once per frame
	/// <summary>
	/// Current method of getting debug information in the editor. Will likely be changed to better suit our needs. 
	/// </summary>
    void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, mask);

			if(hit.collider != null)
			{
				Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int clickPosition = tilemap.WorldToCell(mouseWorldPos);
				clickCount++;
				ChangeTile(clickPosition);
				
			}
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			//Run my algorithm
			Algorithm();

		}
    }
	private void Initialize()
	{
		current = GetNode(startPos);

		openList = new HashSet<Node>();

		closedList = new HashSet<Node>();
		//Adding start to the opoen list
		openList.Add(current);
	}
	/// <summary>
	/// Begins the process of finding the most direct path from the starting location to the target location based on A* pathfinding. 
	/// </summary>
	private void Algorithm()
	{
		if (current == null)
		{
			Initialize();
		}

		while(openList.Count > 0 && path == null)
		{
			List<Node> neighbors = FindNeighbors(current.Position);

			ExamineNeighbors(neighbors, current);

			UpdateCurrentTile(ref current);

			path = GeneratePath(current);
		}

		if (path != null)
		{
			foreach (Vector3Int position in path)
			{
				if (position != goalPos)
				{
					tilemap.SetTile(position, tiles[2]);
				}
			}
		}

		AstarDebugger.MyInstance.CreateTiles(openList, closedList, allNodes, startPos, goalPos, path);
	}
	/// <summary>
	/// Returns list of the neighbors in the pile above, below, left, and right of the given tile position
	/// </summary>
	/// <param name="parentPosition"></param>
	/// <returns></returns>
	private List<Node> FindNeighbors(Vector3Int parentPosition)
	{
		List<Node> neighbors = new List<Node>();

		for(int x = -1; x <=1; x++)
		{
			Vector3Int neighborPosition = new Vector3Int(parentPosition.x - x, parentPosition.y, parentPosition.z);
			if (x != 0)
			{
				if (neighborPosition != startPos && tilemap.GetTile(neighborPosition))
				{
					Node neighbor = GetNode(neighborPosition);
					neighbors.Add(neighbor);
				}


			}
		}

		for(int y = -1; y <= 1; y++)
		{
			Vector3Int neighborPosition = new Vector3Int(parentPosition.x, parentPosition.y - y, parentPosition.z);
			if (y != 0)
			{
				if (neighborPosition != startPos && tilemap.GetTile(neighborPosition))
				{
					Node neighbor = GetNode(neighborPosition);
					neighbors.Add(neighbor);
				}


			}
		}

		return neighbors;
	}
	/// <summary>
	/// Assign gScore values to neighbor nodes, overwriting higher values, as this is a shorter path. 
	/// </summary>
	/// <param name="neighbors"></param>
	/// <param name="current"></param>
	private void ExamineNeighbors(List<Node> neighbors, Node current)
	{
		for(int i = 0; i < neighbors.Count; i++)
		{
			Node neighbor = neighbors[i];

			int gScore = DetermineGScore(neighbors[i].Position, current.Position);

			if (openList.Contains(neighbor))
			{
				if(current.G + gScore < neighbor.G)
				{
					CalcValues(current, neighbor, gScore);
				}
			}

			else if (!closedList.Contains(neighbor))
			{
				CalcValues(current, neighbor, gScore);
				openList.Add(neighbor);
			}
		}
	}
	/// <summary>
	/// Assign values to the node based on the parent's own values. 
	/// G is the weight of movement purely by number of tiles,
	/// H is the weight of movement based off the heuristic of distance from the goal
	/// F is the combination of the two weights. 
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="neighbor"></param>
	/// <param name="cost"></param>
	private void CalcValues(Node parent, Node neighbor, int cost)
	{
		neighbor.Parent = parent;

		neighbor.G = parent.G + cost;

		neighbor.H = ((Math.Abs(neighbor.Position.x - goalPos.x) + Math.Abs(neighbor.Position.y - goalPos.y)) * 10);

		neighbor.F = neighbor.G + neighbor.H;
	}
	/// <summary>
	/// Method likely to be discarded. Useful if tiles that slow or increase movement are introduced later,
	/// or if AI needs to avoid certain locations, but currently just returns a set GScore. 
	/// </summary>
	/// <param name="neighbor"></param>
	/// <param name="current"></param>
	/// <returns></returns>
	private int DetermineGScore(Vector3Int neighbor, Vector3Int current)
	{
		int gScore = 0;
		/*int x = current.x - neighbor.x;
		int y = current.y - neighbor.y;
		if(Math.Abs(x-y) % 2 == 1)
		{
			gScore = 10;
		}
		else
		{
			gScore = 14;
		}*/
		//Currently gScore will always equal 10. If different terrain types are added that affect gScore, this will change.
		gScore = 10;
		return gScore;
	}
	/// <summary>
	/// Handles iterating to the next tile
	/// </summary>
	/// <param name="current"></param>
	private void UpdateCurrentTile(ref Node current)//Uses ref to Node as extra precaution.
	{
		openList.Remove(current);

		closedList.Add(current);

		if(openList.Count > 0)
		{
			current = openList.OrderBy(x => x.F).First();
		}
	}
	/// <summary>
	/// Return node in that position. If node in that position is not being tracked, start tracking it. 
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	private Node GetNode(Vector3Int position)
	{
		if (allNodes.ContainsKey(position))
		{
			return allNodes[position];
		}
		else
		{
			Node node = new Node(position);
			allNodes.Add(position, node);
			return node;
		}
	}
	/// <summary>
	/// Changes tile to start or goal position based on click count
	/// Will be completely changed to assign these values from outside classes. 
	/// </summary>
	/// <param name="clickPos"></param>
	private void ChangeTile(Vector3Int clickPos)
	{
		

		if(clickCount % 2 == 1)
		{
			startPos = clickPos;
		}
		else
		{
			goalPos = clickPos;
		}
	}
	//Not necessary for current project, but kept for reference for now.
	/*private bool ConnectedDiagonally(Node currentNode, Node neighbor)
	{
		Vector3Int direct = currentNode.Position - neighbor.Position;

		Vector3Int first = new Vector3Int(current.Position.x + (direct.x * -1), current.Position.y, current.Position.z);
		Vector3Int second = new Vector3Int(current.Position.x, current.Position.y + (direct.y * -1), current.Position.z);

		if(waterTiles.Contains(first) || waterTiles.Contains(second))
		{
			return false;
		}
		return true;
	}*/
	/// <summary>
	/// Iterats backwards from the goal position to the start position to find the nodes a unit will travel. 
	/// </summary>
	/// <param name="current"></param>
	/// <returns></returns>
	private Stack<Vector3Int> GeneratePath(Node current)
	{
		if(current.Position == goalPos)
		{
			Stack<Vector3Int> finalPath = new Stack<Vector3Int>();

			while(current.Position != startPos)
			{
				finalPath.Push(current.Position);

				current = current.Parent;
			}
			return finalPath;
		}
		
		return null;
	}
}
