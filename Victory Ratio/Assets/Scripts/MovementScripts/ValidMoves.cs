using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ValidMoves : MonoBehaviour
{
	[SerializeField]
	private Tilemap tilemap;
	
	private Vector3Int startPos;
	private Node current;
	private HashSet<Node> openList;
	private HashSet<Node> closedList;
	private Dictionary<Vector3Int, Node> validMoves;
	private Dictionary<Vector3Int, Node> validAttacks;

	private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();
	private int unitMoveRange;
	private int unitAttackRange;
	
	/// <summary>
	/// 
	/// </summary>
	private void Initialize()
	{
		allNodes = new Dictionary<Vector3Int, Node>();
		current = GetNode(startPos);

		openList = new HashSet<Node>();

		closedList = new HashSet<Node>();

		validMoves = new Dictionary<Vector3Int, Node>();
		validAttacks = new Dictionary<Vector3Int, Node>();
		//Adding start to the open list
		openList.Add(current);
	}
	/// <summary>
	/// Begins the process of finding all within range tiles. 
	/// </summary>
	private void Algorithm()
	{
		
		Initialize();
		

		while (openList.Count > 0)
		{
			List<Node> neighbors = FindNeighbors(current.Position);
			
			ExamineNeighbors(neighbors, current);
			
			UpdateCurrentTile(ref current);
			
		}
		
	}
	/// <summary>
	/// Returns list of the neighbors in the pile above, below, left, and right of the given tile position
	/// </summary>
	/// <param name="parentPosition"></param>
	/// <returns></returns>
	private List<Node> FindNeighbors(Vector3Int parentPosition)
	{
		List<Node> neighbors = new List<Node>();

		for (int x = -1; x <= 1; x++)
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

		for (int y = -1; y <= 1; y++)
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
		for (int i = 0; i < neighbors.Count; i++)
		{
			Node neighbor = neighbors[i];

			int gScore = DetermineGScore(neighbors[i].Position, current.Position);

			if (openList.Contains(neighbor))
			{
				if (current.G + gScore < neighbor.G)
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
	/// Determines GScore of neighbor and assigns it to the node. 
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="neighbor"></param>
	/// <param name="cost"></param>
	private void CalcValues(Node parent, Node neighbor, int cost)
	{
		neighbor.Parent = parent;

		neighbor.G = parent.G + cost;
	}
	/// <summary>
	/// Method likely to be discarded. Useful if tiles that slow or increase movement are introduced later,
	/// but currently just returns a set GScore. 
	/// </summary>
	/// <param name="neighbor"></param>
	/// <param name="current"></param>
	/// <returns></returns>
	private int DetermineGScore(Vector3Int neighbor, Vector3Int current)
	{
		//Currently gScore will always equal 10. If different terrain types are added that affect gScore, this will change.
		int gScore = 10;
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
		AddToValid(current);
		if (openList.Count > 0)
		{
			current = openList.OrderBy(x => x.G).First();
		}
	}

	private void AddToValid(Node current)
	{
		if (current.G <= unitMoveRange * 10)
		{
			validMoves.Add(current.Position, current);
		}
		if (current.G <= (unitMoveRange + unitAttackRange) * 10 && !validMoves.ContainsKey(current.Position))
		{
			validAttacks.Add(current.Position, current);
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
	/// Returns the stack of tiles within range to the player based on the movement of the unit selected.
	/// Later will likely need to add tiles that are within striking distance of different unit types,
	/// but this may also be easier to do outside of this class.
	/// May also be easier to just pull the range from the unit in the selected tile rather than sending it in this method. 
	/// </summary>
	/// <param name="movement"></param>
	/// <returns></returns>
	public Dictionary<Vector3Int, Node> GetValidMoves(Vector3Int startPos, int moveRange, int attackRange)
	{
		//validMoves = new Dictionary<Vector3Int, Node>();
		this.startPos = startPos;
		unitMoveRange = moveRange;
		unitAttackRange = attackRange;
		Algorithm();
		
		return validMoves;
	}
	public Dictionary<Vector3Int, Node> GetValidAttacks()
	{
		return validAttacks;
	}
	public Dictionary<Vector3Int, Node> GetValidAttacks(Vector3Int startPos, int attackRange)
	{
		//validMoves = new Dictionary<Vector3Int, Node>();
		this.startPos = startPos;
		unitMoveRange = 0;
		unitAttackRange = attackRange;
		Algorithm();

		return validAttacks;
	}

}
