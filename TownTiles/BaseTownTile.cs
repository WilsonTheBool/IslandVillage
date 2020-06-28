using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts;
public class BaseTownTile : MonoBehaviour
{

	//mouse hover	
	public virtual string GetInfo()
	{
		return "";
	}

	public virtual void OnClick()
	{

	}

	public virtual void OnNewCycle()
	{
		newCycle?.Invoke(this, null);
	}

	public event EventHandler newCycle;

	public float CycleTimeSec;

	public LandType type;

	public enum LandType
	{
		grass, sand, mountain, forest, structure, water
	}

	
}

	//public interface IAnimatedTile
	//{
	//	public void UpdateAnimationState();

	//}

