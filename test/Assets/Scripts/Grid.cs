using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid : MonoBehaviour {

    
    public bool displayGridGizmos;//A bool that lets you choose to display the nodes in the grid or not
    public Vector2 gridWorldSize; //The size of the grid in world coordinates
    public float nodeRadius; //The radius of each node
    float nodeDiameter;//The diameter of each node
    public LayerMask unwalkableMask;//A mask that checks if the node is on an unwalkable surface   
    Node[,] grid;
    

    int gridSizeX, gridSizeY; //The size of the grid on the x and y axis

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);//Round the grid size to a full number
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        
    }


    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;// Get the largest size of the grid
        }

    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];//grid contains an amount of nodes based on the boundaries set
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;//Get the bottom left of the world coordinates by dividing the size of the grid by two, as the grid starts at 0,0
        

        for(int x =0; x< gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius); //Get the size of each node in the world based on its radius and diameter
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius,unwalkableMask)); //Check if the node collides with anything
                grid[x,y] = new Node(walkable, worldPoint,x,y);//Create the grid by scanning and checking for collisions
            }
        }

    }

    public List<Node> GetNeighbours(Node node) //A list of nodes that are neighours to the current node.
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x<= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue; //If x and y are equal to 0, the node is the current node rather than a neighbouring node
                }
                int checkX = node.gridX + x; //A check that makes sure that neigbours aren't trying to be accessed outside of the grid
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) //Check if the nodes are between the min and max values of the grid
                {
                    neighbours.Add(grid[checkX, checkY]); //Add the neighbouring nodes to the list

                }
            }

        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x - transform.position.x) / gridWorldSize.x + 0.5f - (nodeRadius / gridWorldSize.x);//Find the x position of the grid based on the world space
        float percentY = (worldPos.z - transform.position.z) / gridWorldSize.y + 0.5f - (nodeRadius / gridWorldSize.y);//Find the y position of the grid based on the world space
        percentX = Mathf.Clamp01(percentX);//Clamp the position so no errors are called if a node is outside of the grid
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);//subtract the grid size by one to stay inside the array
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));//Draw a cube to set the boundaries of the walkable path

		if (grid != null&& displayGridGizmos)
            {
            var result = from Node n in grid //for each node in the grid that is walkable, colour the node white.
                         where n.walkable = true
                         select (Gizmos.color = Color.white);   
                        foreach(Node n in grid)
            {
                
            Gizmos.DrawCube(n.worldPos,Vector3.one * (nodeDiameter - .1f));    //Draw each node as a cube in the world space
            }


        }
        }
    }

