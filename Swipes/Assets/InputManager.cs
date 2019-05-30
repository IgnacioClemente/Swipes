using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Gesture {swipeRight,swipeLeft,swipeUp,swipeDown, swipeUpRight, swipeUpLeft, swipeDownRight, swipeDownLeft, swipePlus,swipeX, None};

public class InputManager : MonoBehaviour 
{
    Vector2 initialPosition;
	Vector2 endPosition;
	Vector2 swipeDirection;

	[SerializeField] private float minSwipeValue = 10;
    [SerializeField]
    private float marginPercentage = 30;
    [SerializeField]
    private Text DebugText;
    float timer = 0;

    Gesture firstSwipe = Gesture.None;
    Gesture previous;

	Gesture gestureDone;

    Vector2 ScreenSize;

	NodoSwipe firstSwipeNodo;
	NodoSwipe secondSwipeNodo;

    void Start () {
        ScreenSize = new Vector2(Screen.width, Screen.height);
	}
	

	void Update ()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                switch (Input.GetTouch(0).phase)
                {
                    case TouchPhase.Began:
                        initialPosition = Input.GetTouch(0).position;
                        break;
				case TouchPhase.Ended:
				case TouchPhase.Canceled:

					endPosition = Input.GetTouch (0).position;
                //SwipeCombination(initialPosition, endPosition);
                break;
                }
            }
        }
        else if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                initialPosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                endPosition = Input.mousePosition;
            }
        }
		//Calculo direccion
		if(initialPosition != Vector2.zero && endPosition != Vector2.zero) swipeDirection = endPosition - initialPosition;

		//Debug.Log ("Init: " + initialPosition);
		//Debug.Log ("End: " + endPosition);
		//Debug.Log ("Dir: " + swipeDirection);

		if (Mathf.Abs(swipeDirection.x) >= minSwipeValue || Mathf.Abs(swipeDirection.y) >= minSwipeValue)
			gestureDone = DetectGestureBasedOnSwipeDirection (swipeDirection);
		else
			gestureDone = Gesture.None;

		if (gestureDone != Gesture.None) 
		{
			CancelInvoke ("CleanNodo");
			PrintText ();

			//Debug.Log ("Se hizo un " + gestureDone.ToString () + " con direccion " + swipeDirection.ToString ());

			//Si el first es null, es el primer swipe del usuario
			if(firstSwipeNodo == null) firstSwipeNodo = new NodoSwipe (initialPosition, endPosition, gestureDone);
			else if(secondSwipeNodo == null) secondSwipeNodo = new NodoSwipe (initialPosition, endPosition, gestureDone);

			CleanDirections ();

			Invoke ("CleanNodo", 2f);
		}

		if (firstSwipeNodo != null && secondSwipeNodo != null)
			SwipeCombination (firstSwipeNodo, secondSwipeNodo);

    }

	Gesture SwipeCombination(NodoSwipe firstSwipe, NodoSwipe secondSwipe)
    {
		Debug.LogWarning ("A COMBINAR");
  
		if(IsPlus(firstSwipe.Gesture, secondSwipe.Gesture))
        {
            gestureDone = Gesture.swipePlus;
            Debug.Log("Isplus");
        }
	    else
		{
			if (IsX(firstSwipe.Gesture, secondSwipe.Gesture))
            {
                gestureDone = Gesture.swipeX;
                Debug.Log("IsXXXX");
            }
        }

        previous = gestureDone;
        //timer = timer - Time.deltaTime();
		PrintText ();

		CleanNodo ();

		return gestureDone;

    }

    private Gesture DetectGestureBasedOnSwipeDirection(Vector2 swipeDirection)
    {
        //Debug.Log("Margen en X: " + Mathf.Abs(swipeDirection.y) * marginPercentage / 100 + ". Margen en Y: " + Mathf.Abs(swipeDirection.x) * marginPercentage / 100);
        if (swipeDirection.x < 0 && Mathf.Abs(swipeDirection.y) < Mathf.Abs(swipeDirection.x) * marginPercentage / 100)
        {
            Vector3 playerPosition = transform.position;
            playerPosition.x = 3f;
            transform.position -= playerPosition;
            return Gesture.swipeLeft;
        }
        if (swipeDirection.x > 0 && Mathf.Abs(swipeDirection.y) < Mathf.Abs(swipeDirection.x) * marginPercentage / 100)
        {
            Vector3 playerPosition = transform.position;
            playerPosition.x = 3f;
            transform.position += playerPosition;
            return Gesture.swipeRight;
        }
        if (swipeDirection.y > 0 && Mathf.Abs(swipeDirection.x) < Mathf.Abs(swipeDirection.y) * marginPercentage / 100)
            return Gesture.swipeUp;
        if (swipeDirection.y < 0 && Mathf.Abs(swipeDirection.x) < Mathf.Abs(swipeDirection.y) * marginPercentage / 100)
            return Gesture.swipeDown;

        if (swipeDirection.x > 0 && swipeDirection.y > 0)
            return Gesture.swipeUpRight;
        if (swipeDirection.x < 0 && swipeDirection.y > 0)
            return Gesture.swipeUpLeft;
        if (swipeDirection.x > 0 && swipeDirection.y < 0)
            return Gesture.swipeDownRight;
        if (swipeDirection.x < 0 && swipeDirection.y < 0)
            return Gesture.swipeDownLeft;
        return Gesture.None;

    }
    void PrintText()
	{
		CancelInvoke("EraseText");
       
       // Debug.Log("Se hizo un " + gestureDone.ToString());
        DebugText.text = gestureDone.ToString();
        Invoke("EraseText", 1);

    }

	void EraseText()
	{
        DebugText.text = "";
	}

	bool IsPlus(Gesture first, Gesture second)
    {
		Debug.LogWarning ("A CHEQUEAR SI ES PLUS 1: " + first.ToString() + " 2: " + second.ToString());
		if((first == Gesture.swipeRight && second == Gesture.swipeUp)
			|| (first == Gesture.swipeLeft && second == Gesture.swipeDown))
        {
			return true;
        }

		if ((first == Gesture.swipeRight && second == Gesture.swipeDown)
		   || (first == Gesture.swipeLeft && second == Gesture.swipeUp)) 
		{
			return true;
		}

		if ((first == Gesture.swipeUp && second == Gesture.swipeRight)
		   || (first == Gesture.swipeUp && second == Gesture.swipeLeft)) 
		{
			return true;
		}

		if ((first == Gesture.swipeDown && second == Gesture.swipeRight)
		   || (first == Gesture.swipeDown && second == Gesture.swipeLeft)) 
		{
			return true;
		}
		return false;
    }

    bool IsX(Gesture first, Gesture second)
    {
        Debug.LogWarning("A CHEQUEAR SI ES XXXXXXXXXXX 1: " + first.ToString() + " 2: " + second.ToString());

        if ((first == Gesture.swipeUpLeft && second == Gesture.swipeDownLeft)
			|| (first == Gesture.swipeUpRight && second == Gesture.swipeDownRight))
        {
            return true;
        }

		if((first == Gesture.swipeUpLeft && second == Gesture.swipeUpRight)
			|| (first == Gesture.swipeDownLeft && second == Gesture.swipeDownRight))
		{
			return true;
		}	

		if((first == Gesture.swipeDownRight && second == Gesture.swipeDownLeft)
            || (first == Gesture.swipeDownLeft && second == Gesture.swipeUpRight))
        {
			return true;
		}

        if ((first == Gesture.swipeDownRight && second == Gesture.swipeUpLeft)
            || (first == Gesture.swipeUpRight && second == Gesture.swipeUpLeft))
        {
            return true;
        }

        if ((first == Gesture.swipeDownRight && second == Gesture.swipeUpRight)
            || (first == Gesture.swipeDownLeft && second == Gesture.swipeUpLeft))
        {
            return true;
        }

        return false;
    }

	void CleanNodo()
	{
		Debug.LogWarning ("A BORRAR NODITOS");
		gestureDone = Gesture.None;
		firstSwipeNodo = null;
		secondSwipeNodo = null;
	}

	void CleanDirections()
	{
		initialPosition = Vector2.zero;
		endPosition = Vector2.zero;
		swipeDirection = Vector2.zero;
	}
}
