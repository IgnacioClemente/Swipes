using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodoSwipe
{
	private Vector2 _initialPosition;
	private Vector2 _endPosition;
	private Gesture _gesture;

	//GETTERS
	public Vector2 InitialPosition { get { return _initialPosition; } } 
	public Vector2 EndPosition { get { return _endPosition; } }
	public Gesture Gesture { get { return _gesture; } } 

	//Constructor
	public NodoSwipe (Vector2 initial, Vector2 end, Gesture gest)
	{
		_initialPosition = initial;
		_endPosition = end;
		_gesture = gest;
	}
}
