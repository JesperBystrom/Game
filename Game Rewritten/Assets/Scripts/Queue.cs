using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queue <T> {
	private List<T> queue = new List<T>();

	public void addToQueue(T item){
		queue.Add(item);
	}

	public void removeFromQueue(T item){
		queue.Remove(item);
	}
}
