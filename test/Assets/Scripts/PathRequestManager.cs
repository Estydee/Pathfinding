using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour {
	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>{};// A queue that holds each path request
	PathRequest currentPathRequest;//The current path request

	static PathRequestManager instance;//Gets an instance of the same path request manager across all units requesting it.
	PathFinding pathfinding;// A reference to the pathfinding script

	bool isProcessingPath;//A bool to check if a path is current being processed

	void Awake()
	{
		instance = this;//Initialise the instance and get a component of the pathfinding script
		pathfinding = GetComponent<PathFinding> ();
	}


	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
	{
		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback);//A new path request that takes a starting point, an ending point, and a callback to send the path to the unit
		instance.pathRequestQueue.Enqueue (newRequest);//Puts the requested path into a queue
		instance.TryProcessNext ();//Attempts to process the next path in the queue
	}


	void TryProcessNext()
	{
		if (!isProcessingPath && pathRequestQueue.Count > 0) //If a path is not being processed and the queue is not empty...
		{
			currentPathRequest = pathRequestQueue.Dequeue ();//Take the path request out of the queue
			isProcessingPath = true;//A path is now being processed
			pathfinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);//Call the method that starts to find a path
		}
	}

	public void FinishedProcessingPath(Vector3[] path, bool success)
	{
		currentPathRequest.callback (path, success);//After a path has finished processing, it sends a callback that contains the path and whether it was successful
		isProcessingPath = false;//A path is now no longer being processed
		TryProcessNext ();//Try processing the next path request in the queue
	}

	struct PathRequest
	{
		public Vector3 pathStart; //A position for the start of the path
		public Vector3 pathEnd;// A position for the end of the path
		public Action<Vector3[], bool> callback;//A callback that holds the path and its success

		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[],bool> _callback)//Assign these variables for each path request
		{
			pathStart =_start;
			pathEnd = _end;
			callback = _callback;
		}
	}


}
