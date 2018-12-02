﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseState 
{
 	[SerializeField] private List<Transform> patrolRouteParents = new List<Transform>();
	private List<List<Vector3>> patrolRoutes = new List<List<Vector3>>();

	private void Awake()
	{
		base.Awake();

		for(int x = 0; x < patrolRouteParents.Count; x++)
		{
			List<Vector3> temp = new List<Vector3>();
			for(int y = 0; y < patrolRouteParents[x].childCount; y++)
			{
				temp.Add(patrolRouteParents[x].GetChild(y).position);
			}
			patrolRoutes.Add(temp);
		}
	}

	public override void Construct()
	{
		Debug.Log("Entering Patrol State");

		float minDistance = Vector3.Distance(transform.position, patrolRoutes[0][0]);
		Vector2Int minDistID = new Vector2Int(0, 0);

		for(int x = 0; x < patrolRoutes.Count; x++)
		{
			for(int y = 0; y < patrolRoutes[x].Count; y++)
			{
				float distance = Vector3.Distance(transform.position, patrolRoutes[x][y]);
				if(distance < minDistance)
				{
					minDistance = distance;
					minDistID = new Vector2Int(x, y);
				}
			}
		}

		coroutine = Patrol(minDistID.x, minDistID.y);
		StartCoroutine(coroutine);
	}

	public override void Destruct()
	{
		StopCoroutine(coroutine);
	}

	private IEnumerator Patrol(int x, int y)
	{
		int patrolRouteIndex = y;

		while(true)
		{
			// Follow patrol route
			motor.agent.SetDestination(patrolRoutes[x][patrolRouteIndex]);

			while(Vector3.Distance(transform.position, motor.agent.destination) > 2)
			{
				yield return null;
			}

			patrolRouteIndex++;
			if(patrolRouteIndex >= patrolRoutes[x].Count)
			{
				patrolRouteIndex = 0;
			}
			yield return null;
			// TODO make the AI ocassionally change routes
		}
	}

	public override void Transition()
	{
		if(motor.CanSeePlayer())
		{
			motor.ChangeState("ChaseState");
		}
	}
}

