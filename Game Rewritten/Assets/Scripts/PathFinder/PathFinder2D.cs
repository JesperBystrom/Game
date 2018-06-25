using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Isometric pathfinder, y = z.
*/
public class PathFinder2D {

	private int width;
	private int height;
	private Node2D[] nodes;

	public PathFinder2D(int width, int height) {
		this.width = width;
		this.height = height;
		this.nodes = new Node2D[width*height];

		for(int y=0;y<height;y++){
			for(int x=0;x<width;x++){
				nodes[x+y*width] = new Node2D(x,y);
			}
		}
	}

	public Node2D[] findPath(int x1, int y1, int x2, int y2, int range, bool includeBlockage){
		Node2D startNode = getNode(x1,y1);
		Node2D endNode = getNode(x2,y2);
		List<Node2D> path = new List<Node2D>();

		if(includeBlockage){
			foreach(Node2D n in nodes){
				Territory t = Map.instance.findTerritory(n.x, n.y);
				if(t == null) continue;
				if(t.hasEntity()){
					n.close();
				}
			}
			if(endNode.closed) return null;
		}

		startNode.close();
		Node2D current = startNode;
		int val = 0;
		while(current.distance(endNode) > 0) {
			if(val >= range) break;
			current.fCost = 9999;
			Node2D lowest = current;
			Node2D[] neighbours = getNeighbours(current);
			foreach(Node2D n in neighbours){
				if(n == null) continue;
				n.calculateFCost(startNode, endNode);
				if((n.fCost < lowest.fCost && !n.closed) || n.Equals(endNode))
					lowest = n;
			}
			current = lowest;
			current.close();
			val++;
			path.Add(lowest);
		}
		reset();
		return ArrayUtils<Node2D>.listToArray(path);
	}

	public Node2D[] getNeighbours(Node2D node){
		List<Node2D> nodes = new List<Node2D>();
		nodes.Add(getNode(node.x-1, node.y));
		nodes.Add(getNode(node.x+1, node.y));
		nodes.Add(getNode(node.x, node.y-1));
		nodes.Add(getNode(node.x, node.y+1));
		nodes.Add(getNode(node.x+1, node.y+1));
		nodes.Add(getNode(node.x-1, node.y+1));
		nodes.Add(getNode(node.x-1, node.y-1));
		nodes.Add(getNode(node.x+1, node.y-1));
		return ArrayUtils<Node2D>.listToArray(nodes);
	}

	public Node2D getNode(int x, int y){
		if(x >= 0 && x <= width && y >= 0 && y <= height)
			return nodes[x+y*width];
		return null;
	}

	public void reset(){
		foreach(Node2D n in nodes){
			n.open();
		}
	}
}

public static class ArrayUtils<T> {
	public static T[] listToArray(List<T> list){
		T[] result = new T[list.Count];
		for(int i=0;i<list.Count;i++) {
			result[i] = list[i];
		}
		return result;
	}
}