using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public bool walkable; //Check if a surface is able to be walked on
    public Vector3 worldPos; //The point in the world the node represents
    public int gridX; //The size of the node on the x axis
    public int gridY; //The size of the node on the Y axis

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;
    
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)   //A method that holds all of the parameters needed to create a node
    {
        walkable = _walkable;// Assigning paramaters to variables
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
    
		
    public int fCost//The fCost is calculated by adding the gCost and hCost together
    {
        get
        {
            return gCost + hCost;
        }

    }

    public int HeapIndex// A property that gets and sets the value of each index in the heap
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);//Compare the fcost of the current node and the nodeToCompare 
        if(compare == 0)//If the fCost for both nodes are the same...
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);//Compare the hCost instead
        }
        return -compare;//return the result of the compared node. 
    }


	}

