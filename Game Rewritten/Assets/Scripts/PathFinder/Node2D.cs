using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node2D {
	public int x;
	public int y;
	public float fCost;
	public bool closed;

	public Node2D(int x, int y){
		this.x = x;
		this.y = y;
	}

	public float calculateHCost(Node2D endNode){
		return distance(endNode);
	}

	public float calculateGCost(Node2D startNode){
		return distance(startNode);
	}

	public void calculateFCost(Node2D startNode, Node2D endNode){
		this.fCost = calculateGCost(startNode) + calculateHCost(endNode);
	}

	public float distance(Node2D node){
		return Mathf.Abs(Mathf.Sqrt(Mathf.Pow(node.x-x,2) + Mathf.Pow(node.y-y,2)));
	}

	public void open(){
		closed = false;
	}

	public void close(){
		closed = true;
	}
}
