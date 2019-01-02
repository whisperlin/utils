using System;
using System.Collections.Generic;
using UnityEngine;

// A C# MinHeap implementation used by the mesh simplification algorithm.
// It's used to maintain a sorted list of changing edge costs during simplification.

#if false
class PriorityQueue : MonoBehaviour
{
	void Start() {
		// A test for the MinHeap
		
		Krabl.MinHeap<string> heap = new Krabl.MinHeap<string>();
		heap.insert(new Krabl.HeapNode<string>(3.0f, "drei"));
		heap.insert(new Krabl.HeapNode<string>(1.0f, "eins"));
		heap.insert(new Krabl.HeapNode<string>(4.0f, "vier"));
		heap.insert(new Krabl.HeapNode<string>(2.0f, "zwei"));
		Krabl.HeapNode<string> f = new Krabl.HeapNode<string>(0.5f, "fünf");
		heap.insert(f);
		heap.insert(new Krabl.HeapNode<string>(8.0f, "acht"));
		
		heap.update(f, 5.0f);
	
		while (heap.size() > 0) {
			Krabl.HeapNode<string> n = heap.extract();
			Debug.Log(n.obj + " <- " + n.heapValue);
		}
	}
}
#endif

namespace KrablMesh {	
	class HeapNode<TObj>
	{
		public HeapNode(float k, TObj o) {
			obj = o;
			heapValue = k;
			index = -42;
		}
		
		public bool IsInHeap() { return index != -42; }
		public void RemoveFromHeap() { index = -42; }
		
		public TObj obj;
		public float heapValue;
		public int index;
	}
	
	class MinHeap<TObj>
	{
		List<HeapNode<TObj>> _buffer;

		public MinHeap(int capacity = 8) {
			_buffer = new List<HeapNode<TObj>>(capacity);
		}
				
		static int _parent(int i) { return (i - 1) >> 1; }
   		static int _left(int i) { return i + i + 1; }
   		static int _right(int i) { return i + i + 2; }

		void _place(HeapNode<TObj> node, int i) {
    		_buffer[i] = node;
			node.index = i;
		}

		void _swap(int i, int j) {
    		HeapNode<TObj> tmp = _buffer[i];

    		_place(_buffer[j], i);
    		_place(tmp, j);
		}

		void _upheap(int i) {
			HeapNode<TObj> moving = _buffer[i];
		    int index = i;
		    int p = _parent(i);
		
		    while (index > 0 && moving.heapValue < _buffer[p].heapValue) {
				_place(_buffer[p], index);
				index = p;
				p = _parent(p);
		    }
		
		    if (index != i) _place(moving, index);
		}

		void _downheap(int i) {
			int length = _buffer.Count;
			if (!(i < length)) return;
			
		    HeapNode<TObj> moving = _buffer[i];
		    int index = i;
		    int l = _left(i);
		    int r = _right(i);
		    int largest;
				
		    while (l < length) {
				if (r < length && _buffer[l].heapValue > _buffer[r].heapValue) largest = r;
				else largest = l;
		
				if (moving.heapValue > _buffer[largest].heapValue) {
			    	_place(_buffer[largest], index);
			    	index = largest;
			    	l = _left(index);
			    	r = l + 1; //right(index);
				} else {
			  		break;
				}
		    }
		
		    if (index != i) _place(moving, index);
		}
				
		public int Size() {
			return _buffer.Count;
		}
		
		public HeapNode<TObj> Insert(HeapNode<TObj> node) {
			int i = _buffer.Count;
			node.index = i;
			_buffer.Add(node);
			_upheap(i);
			return node;
		}	
		
		public void Update(HeapNode<TObj> node, float v) {
			node.heapValue = v;					
		 //  	if(!node.IsInHeap()) {
				//Debug.LogError("Updateing node which is not in heap!");
		//		return;
		//	}
			
		    int i = node.index;
		
		    if (i > 0 && v < _buffer[_parent(i)].heapValue) _upheap(i);
			else _downheap(i);
		}
		
		public HeapNode<TObj> Extract() {
			int p = _buffer.Count - 1;
		    if (p < 0) return null;
		
		    _swap(0, p);
			
		    HeapNode<TObj> dead = _buffer[p];
			_buffer.RemoveAt(p);
			dead.RemoveFromHeap();
		
		    _downheap(0);
		    return dead;
		}
		
		public HeapNode<TObj> Remove(HeapNode<TObj> node) {
		   	if(!node.IsInHeap()) {
				//Debug.LogError("Removing node which is not in heap!");
				return null;
			}
		
		    int i = node.index;
			int p = _buffer.Count - 1;
		    _swap(i, p);
			_buffer.RemoveAt(p);
		    node.RemoveFromHeap();
		
		    if (_buffer[i].heapValue > node.heapValue) _downheap(i);
		    else _upheap(i);
		
		    return node;
		}	
	}

};

