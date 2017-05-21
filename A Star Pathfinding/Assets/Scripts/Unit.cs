using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingUnit
{
    public abstract Vector3 Mover(Transform seeker, Vector3 waypoint, float speed);//An abstract method that holds the starting position, ending position, and speed of the unit


}

public class Movement: MovingUnit
{

    public override Vector3 Mover(Transform seeker, Vector3 waypoint, float speed)
    {
        return Vector3.MoveTowards(seeker.position, waypoint, speed * Time.deltaTime);//Overrides the abstract and uses its parameters to make the character move towards each waypoint in the path

    }

}


public class Unit : MonoBehaviour {
	public Transform target;//A reference of the target to find a path to
	float speed = 20;//The speed that the unit moves at
	Vector3[] path;//An array that holds each node that the unit will travel through
	int targetIndex;//The position of the target's index in the array
    List<int> colourTest = new List<int>();// A list that holds some colours for drawing gizmos
    delegate void Drawing(Vector3 size);//A delegate that determines the size of each node


    void Start()
	{
		PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);//On the first frame, each unit will request a path from the path request manager
        
    }

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful) {//If a path has been found...
			path = newPath;//Then the path now becomes the new path to travel on
			StartCoroutine ("FollowPath");//This starts the method that makes the unit follow the path

		}
	}
	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];//The first waypoint in the array is always the first waypoint to travel to
        Movement moveTo = new Movement();//A variable to declare the mover method
		while (true) {
			if (transform.position == currentWaypoint) {//If the unit's current position lands on the curren waypoint
				targetIndex++;// The target's index is incremented as the current waypoint is destroyed
				if (targetIndex >= path.Length) {//If the target's index is greater than the size of the path...
					yield break;//break the loop
				}
				currentWaypoint = path [targetIndex];// The current waypoint becomes the target's index in the path
			}

            transform.position = moveTo.Mover(transform, currentWaypoint, speed);//A call to the Mover method, using the unit's transform, the current waypoint, and travel speed
			yield return null;

            

        }

	}
   

	public void OnDrawGizmos()
	{
		if (path != null) {//If a path exists...
			for (int i = targetIndex; i < path.Length; i++)
            { 
                colourTest.Add(i);//Add each waypoint to the colourTest list

                int oddNum = colourTest.FindIndex(x => x % 2 != 0);//A lambda expression to get the odd number of each index in the array

                if (i == oddNum)//If the number is odd, each node is blue, otherwise it's black
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.black;
                }

                Drawing d = size => Gizmos.DrawCube(path[i], size);//A call to the delegate which defines the size of each node in the path array
                d.Invoke(Vector3.one);// Invoke the delegate and use 1,1,1 as the size of each node

                if (i == targetIndex) {// If the index reaches the current index, draw a line from the units current position and the next point in the path
					Gizmos.DrawLine (transform.position, path [i]);
				} else {//Otherwise, draw a line between the previous and next nodes.
					Gizmos.DrawLine (path [i - 1], path [i]);
				}



			}
		}

	}
}
