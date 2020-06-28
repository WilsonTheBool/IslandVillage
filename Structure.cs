using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.TownTiles;


namespace Assets.Scripts
{
    public class Structure: BaseTownTile
    {
		public string tileName;
		//обрабатываемые клетки ресурсов
		public List<ResourceObject> resourcseObj;

		public int workers;

		public int maxWorkers;

		public float resoursesPerWorker;

		public Resource resourse;

		public GameController game;

		public TownGameTile tile;

		public Sprite structureSprite;

		public float costStone;

		public float costWood;

		public GameObject axeIcon;
		public void Start()
		{
			SetUp();
		}
		public virtual void SetUp()
		{
			game = FindObjectOfType<GameController>();

			resourcseObj = new List<ResourceObject>(); 



			UpdateOwnedTiles();
			UpdateMaxWorkers();

		}
		public virtual void UpdateMaxWorkers()
		{
			maxWorkers = resourcseObj.Count * 2;

			if(workers > maxWorkers)
			{
				RemoveWorker(workers - maxWorkers);
			}
		}

		public virtual void RemoveWorker(int count)
		{
			workers -= count;
			if(workers < 0)
			{
				workers = 0;
			}
		}

		public virtual void AddWorker(int count)
		{
			if (game.population - game.workingPeople > 0)
			{
				workers += count;
				if (workers > maxWorkers)
				{
					workers = maxWorkers;
				}
			}
		}

		public event EventHandler workersChanged;

		public void OnResourseStateChanged(object res, EventArgs e)
		{

		}

		public bool isDeleted;
		public override void OnNewCycle()
		{
			//Добавить ресурсы;
			if (!isDeleted) 
			ChangeResources();
			//Изменить клетку ресурсов;
		}

		public virtual void AddResourcesToPlayer(float ammount, Resource res)
		{
			
			game.AddResources(ammount, res, this);
			ShowResourceAnimation();
		}
		public GameObject fadeResource;
		private  void ShowResourceAnimation()
		{
			Instantiate(fadeResource, gameObject.transform);
		}
		public virtual void OnResourceRemoved(object resObj, EventArgs e)
		{
			UpdateOwnedTiles(resObj as ResourceObject);
		}
		public virtual void UpdateOwnedTiles()
		{
			//unsub
			foreach (ResourceObject tile in resourcseObj)
			{
				tile.resourceStateChanged -= OnResourseStateChanged;
				tile.resourceRemoved -= OnResourceRemoved;
			}

			var tiles = game.GetAllResourceTilesOfType(resourse, this);
			resourcseObj.Clear();
			resourcseObj = new List<ResourceObject>(tiles);

			//sub to new
			foreach (ResourceObject tile in resourcseObj)
			{
				tile.resourceStateChanged += OnResourseStateChanged;
				tile.resourceRemoved += OnResourceRemoved;
			}

			UpdateMaxWorkers();
		}

		public virtual void UpdateOwnedTiles(ResourceObject tile)
		{
			//unsub
			if (!tile.HasResource(resourse))
			{
				tile.resourceStateChanged -= OnResourseStateChanged;
				tile.resourceRemoved -= OnResourceRemoved;

				resourcseObj.Remove(tile);
			}

			UpdateMaxWorkers();
		}

		private void ChangeResources()
		{
			if (workers != 0)
			{
				int wk = workers;
				int t;
				for (int i = 0; i < resourcseObj.Count; i++)
				{
					ResourceObject res = resourcseObj[i];
					if (wk >= 2)
					{
						wk -= 2;
						t = 2;

					}
					else
					{
						t = wk;
					}

					res.RemoveResources(t * resoursesPerWorker, resourse);

					if (t < 2)
					{
						break;
					}
				}

				AddResourcesToPlayer(workers * resoursesPerWorker, resourse);
			}
		}

		public event EventHandler onDelete;
		public virtual void Delete()
		{
			foreach(ResourceObject obj in resourcseObj)
			{
				obj.resourceStateChanged -= OnResourseStateChanged;
				obj.resourceRemoved -= OnResourceRemoved;

				
			}
			onDelete?.Invoke(this,null);
			isDeleted = true;
			game.SpawnSmoke(transform.position);
			game.structureMap.SetTile(game.structureMap.WorldToCell(transform.position), null);
			game.inputController.ClearSelectedObj();
			Destroy(this.gameObject, 0.1f);
		}

		public TownGameTile[] canBuildTiles;

		public virtual bool CanBuild(TownGameTile tile)
		{
			foreach(TownGameTile t in canBuildTiles)
			{
				if(tile == t)
				{
					return true;
				}
			}
			return false;
		}

		public override string GetInfo()
		{
			string str = tileName + "\n";

			str += $"Workers: {workers}/{maxWorkers}\n";

			if(resourse != null)
			str += $"Income: {resourse.resourceName} ({workers * resoursesPerWorker})\n";


			return str;
		}
	}
}
