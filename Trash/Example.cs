using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Example : MonoBehaviour {

	class Player
	{
		public int playerId;

		public Player(int id)
		{
			this.playerId = id;
		}

		public void Die(Player enemy)
		{
			Debug.Log("Player " + this.playerId + " died by " + enemy.playerId);
		}

		public void Revive(Player enemy)
		{
			Debug.Log("Player " + this.playerId + " has been revive by " + enemy.playerId);
		}
	}

	delegate void ActionDelegate(Player enemy);
	ActionDelegate myDelegate;
	delegate void DelegateExample(int a);

	void Start () 
	{
		FooBar(Bar);
	}

	void FooBar(DelegateExample dele)
	{
		dele(50);
	}

	void Foo(int a)
	{
		Debug.Log("Foo " + a.ToString());
	}

	void Bar(int b)
	{
		Debug.Log("Bar " + b.ToString());
	}
}
