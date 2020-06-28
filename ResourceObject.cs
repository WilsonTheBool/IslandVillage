using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.TownTiles;

namespace Assets.Scripts
{
	public class ResourceObject : BaseTownTile
	{

		public string tileName;


		public GameObject[] resources;

		public List<Resource> real;

		public GameController game;

		public override string GetInfo()
		{
			string str = tileName + "\n";

			

			foreach (Resource res in real)
			{
				str += res.GetInfo() + "\n";
			}
			

			return str;
		}

		//меняется спрайт ресурсов в зависимости от их кол-ва на клетке
		public bool HasResource(Resource res)
		{
			
			foreach (Resource r in real)
			{
				
				if (r.resourceName == res.resourceName)
				{
					return true;
				}
			}
			return false;
		}

		public event EventHandler resourceStateChanged;

		public void RemoveResources(float ammount, Resource resource)
		{
			for(int i = 0; i < real.Count; i++)
			{
				Resource res = real[i];
				if (res.resourceName == resource.resourceName)
				{
					res.RemoveResources(ammount);
				}
			}
		}

		public void AddResources(float ammount, Resource resource)
		{
			for (int i = 0; i < real.Count; i++)
			{
				Resource res = real[i];
				if (res.resourceName == resource.resourceName)
				{
					res.AddResources(ammount);
				}
			}
		}

		public void RemoveResource(Resource res)
		{
			Resource r = null;
			foreach(Resource temp in real)
			{
				if(temp.resourceName == res.resourceName)
				{
					r = temp;
				}
			}

			if(r != null)
			{
				real.Remove(r);
			}
		}

		public void OnResourceChange(object res, EventArgs e)
		{
			Resource r = res as Resource;
			if((r).resourceAmmount == 0)
			{
				RemoveResource(r);
				resourceRemoved?.Invoke(this, null);
			}

			if(real.Count == 0)
			{
				Delete();
			}
		}

		public void Delete()
		{
			deleted?.Invoke(this, null);
			game.structureMap.SetTile(game.structureMap.WorldToCell(transform.position), null);
			game.SpawnSmoke(this.transform.position);
			gameObject.SetActive(false);
			
		}
		public event EventHandler deleted;
		public event EventHandler resourceRemoved;

		public void Start()
		{
			SetUp();
		}
		public void SetUp()
		{
			//resources - префабы! real - экземпляры
			real = new List<Resource>();
			game = FindObjectOfType<GameController>();

			for (int i = 0; i < resources.Length; i++)
			{
				real.Add(Instantiate(resources[i], this.gameObject.transform).GetComponent<Resource>());

				real[i].StateChanged += resourceStateChanged;
				real[i].ammountChanged += OnResourceChange;

				real[i].owner = this;
			}

			
		}

		
	}
}
