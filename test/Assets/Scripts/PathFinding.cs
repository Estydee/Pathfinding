using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFinding : MonoBehaviour {

	PathRequestManager requestManager;
    Grid grid;
    

    void Awake()
    {
        grid = GetComponent<Grid>(); //Get a component to access the grid script
		requestManager = GetComponent<PathRequestManager>();//A component to the PathRequestManager script
    }



	public void StartFindPath(Vector3 startPos, Vector3 targetPos)
	{
		StartCoroutine(FindPath(startPos,targetPos));//Starts the method that will find a path
	}


	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {

		Vector3[] waypoints = new Vector3[0];//An array of waypoints that the unit will travel across
		bool pathSuccess = false;//A bool to check whether or not the path was successfully found.

        Node startNode = grid.NodeFromWorldPoint(startPos);//The node that the pathfinder starts at
        Node targetNode = grid.NodeFromWorldPoint(targetPos);//The node that the pathfinder needs to get to

		if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize); //An open list of nodes to check which nodes are able to be used
			HashSet<Node> closedSet = new HashSet<Node> ();//A closed list of nodes to indicate that they've already been used
			openSet.Add (startNode);//Add the starting node to the open set

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst (); //The current node will always start at the first node in the list
				closedSet.Add (currentNode);//Add the current node to the closed set as it should always be the first node to travel to

				if (currentNode == targetNode) {//If the current node has reached the target node, the path has been successfully found
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {//checks if the neighbouring node is traversable or hasn't already been checked
						continue;
					}

					int newMoveCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour); //New cost to move to a neighbour is gcost plus the distance between current and neighbour nodes
					if (newMoveCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) { //If neighbour cost is less than neighbour's gcost or neighbour node is not in the openset
						neighbour.gCost = newMoveCostToNeighbour;//neighbour gcost equals new neighbour cost
						neighbour.hCost = GetDistance(neighbour, targetNode);//neighbour hcost equals the distance from the neighbour to the target node
						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);//Add the neighbour to the openset if it isn't already
                        
						} else {
							openSet.UpdateItem (neighbour);//Update the heap to find the neibour with highest priority
						}
					}
				}

			}
		}
		yield return null;
		if (pathSuccess) {
			waypoints = RetracePath(startNode, targetNode);//Get each node used between the start and end nodes
		}
		requestManager.FinishedProcessingPath(waypoints, pathSuccess);// Call the method that tells the request manager that the path has finished processing
    }
  

	Vector3[] RetracePath (Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();//A list that holds all of the nodes used between the start and end nodes
        Node currentNode = endNode;// The current node will start at the end to trace back to the first node

        while (currentNode != startNode)//While the current node hasn't reached the start node...
        {
            path.Add(currentNode);// Add each node in the path
            
            currentNode = currentNode.parent;//The new current node becomes the one above it in the path
        }
		Vector3[] waypoints = SimplifyPath(path);//Call a method to simplify the path
		Array.Reverse(waypoints);//Reverse the waypoints so it doesn't look like the program was tracing backwards
		return waypoints;
        
    }

	Vector3[] SimplifyPath(List<Node> path)
	{
		List<Vector3> waypoints = new List<Vector3>();//Holds all waypoints on the path
		Vector2 directionOld = Vector2.zero;//The old direction travelled

		for (int i = 1; i < path.Count; i++) {//For each index in the path...
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX,path [i - 1].gridY - path [i].gridY);//The new direction is calculated by subtracting the pevious node and the current node's values
			if (directionNew != directionOld) {//If the old direction isn't the same as the new direction...
				waypoints.Add(path[i-1].worldPos);//Add a new waypoint to the path
			}
			directionOld = directionNew;//Set the new old direction to the old new direction
		}
		return waypoints.ToArray();//Return the list of waypoints back to the path array
	}

   int GetDistance(Node nodeA, Node nodeB)
    {
        int xDist = Mathf.Abs(nodeA.gridX - nodeB.gridX); //Get the distance of nodes A and B on the x axis
        int yDist = Mathf.Abs(nodeA.gridY - nodeB.gridY); //Get the distance of nodes A and B on the y axis

        if (xDist > yDist)// The equations used to calculate the cost of each node
            return 14*yDist + 10* (xDist - yDist);
        return 14*xDist + 10 * (yDist - xDist);
        
    }

    


}




