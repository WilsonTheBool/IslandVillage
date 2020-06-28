using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using Assets.Scripts.TownTiles;
using Assets.Scripts;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnTileHoverScript : MonoBehaviour
{

    // Use this for initialization

    public Tilemap structures;
    public Tilemap land;

    public Text infoText;

    
    public Camera cam;

    public GameObject crown;

    public GameObject range_2;

    public List<GameObject> iconsInPlay;

    public BaseTownTile selectedTile;

    public StructureInfoWindow window;

    public void UpdateSelectedTile(object self, EventArgs e)
    {
        DeleteAllIcons();
        if (selectedTile != null)
        {
            if (selectedTile is Structure && !(selectedTile is House))
            {
                range_2.transform.position = selectedTile.transform.position;
                foreach (ResourceObject res in (selectedTile as Structure).resourcseObj)
                {
                    iconsInPlay.Add(Instantiate((selectedTile as Structure).axeIcon, res.transform.position, new Quaternion(), selectedTile.transform));
                }
            }
            else
            {
                HideRangeTile();
            }

        }
        else
        {
            HideRangeTile();
        }
        
    }

    public void UpdateInfoWindow()
    {
        if(selectedTile != null && selectedTile is Structure)
        {
            if (window.gameObject.activeSelf == false)
            {
                window.gameObject.SetActive(true);
            }

            window.UpdateWindow(selectedTile as Structure);
        }
        else
        {
            if(window.gameObject.activeSelf == true)
            {
                window.gameObject.SetActive(false);
            }
        }
    }
    public void HideRangeTile()
    {
        range_2.transform.Translate(new Vector3(0, 0, -20));
    }
    private void DeleteAllIcons()
    {
        for(int i = iconsInPlay.Count - 1; i >= 0; i--)
        {
            Destroy(iconsInPlay[i], 0.01f);
            iconsInPlay.RemoveAt(i);
        }
    }
    public event EventHandler selectedObjChanged;

    public void ClearSelectedObj()
    {
        if (selectedTile != null)
        {
            selectedTile = null;
            selectedObjChanged?.Invoke(this, null);
        }
    }
    private void Start()
    {
        selectedObjChanged += UpdateSelectedTile;
    }
    // Update is called once per frame
    public void UpdateInput()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
           
        }
        else
        {
            
            UpdateImputGame();
        }
        
    }

    void UpdateImputGame()
    {
        GridInformation info = structures.GetComponent<GridInformation>();
        Vector3 mouseToWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseToWorld.z = 0;
        Vector3Int tilePos = structures.WorldToCell(mouseToWorld);

        if (Input.GetMouseButtonDown(0))
        {
            if (structures.HasTile(tilePos))
            {
                TownGameTile tileStruct = structures.GetTile<TownGameTile>(tilePos);

                if (tileStruct != null)
                {
                    UnityEngine.Object obj = info.GetPositionProperty<UnityEngine.Object>(tilePos, "tileObj", null);
                    BaseTownTile res = (obj as GameObject).GetComponent<BaseTownTile>();
                    if (res != null)
                    {
                        if (selectedTile != res)
                        {
                            selectedTile = res;
                            selectedObjChanged?.Invoke(this, null);
                        }

                    }

                }


            }
            else
            {
                ClearSelectedObj();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelectedObj();
        }

        if (structures.HasTile(tilePos) || land.HasTile(tilePos))
        {
            if (!crown.activeSelf)
            {
                crown.SetActive(true);
            }

            crown.transform.position = land.CellToWorld(tilePos);
        }
        else
        {
            if (crown.activeSelf)
            {
                crown.SetActive(false);
            }
        }


        if (infoText.text != "")
            infoText.text = "";

        if (structures.HasTile(tilePos))
        {
            TownGameTile tileStruct = structures.GetTile<TownGameTile>(tilePos);
            
            if (tileStruct != null)
            {
                UnityEngine.Object obj = info.GetPositionProperty<UnityEngine.Object>(tilePos, "tileObj", null);
                if (obj != null)
                    infoText.text += (obj as GameObject).GetComponent<BaseTownTile>().GetInfo();
            }

        }

        if (land.HasTile(tilePos))
        {
            TownGameTile tileLand = land.GetTile<TownGameTile>(tilePos);
            if (tileLand != null && tileLand.gameObject != null)
            {
                BaseTownTile tile = tileLand.gameObject.GetComponent<BaseTownTile>();
                if (tile != null)
                    infoText.text += tile.GetInfo();

            }


        }

        UpdateInfoWindow();
    }
}
