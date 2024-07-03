using UnityEngine;

public class TowerHealth : Health
{
    GridSystem gridSystem;
    Building building;
    Notifications notifications;

    public override void Start()
    {
        building = FindObjectOfType<Building>();
        gridSystem = FindObjectOfType<GridSystem>();
        notifications = FindObjectOfType<Notifications>();
        base.Start();
    }

    protected override void CheckHealth()
    {
        base.CheckHealth();
    }

    protected override void Kill()
    {
        Vector3Int positionInt = GridSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = building.area;
        areaTemp.position = positionInt;

        gridSystem.SetTilesBlock(areaTemp, GridSystem.TilesType.Destroyed, gridSystem.mainTileMap);
        Destroyed();
        notifications.BuyBackGrid();
        base.Kill();
    }

    private void Destroyed()
    {
        Vector3Int positionInt = GridSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = building.area;
        areaTemp.position = positionInt;
        foreach (var pos in areaTemp.allPositionsWithin)
        {
            if (LandManager.current != null && LandManager.current.storedTilesArray != null)
            {
                foreach (var storedTiles in LandManager.current.storedTilesArray)
                {
                    if (storedTiles.ContainsKey(pos))
                    {
                        storedTiles[pos] = GridSystem.TilesType.Destroyed;
                    }
                }
            }
        }
    }

    protected void KillSell()
    {
        Vector3Int positionInt = GridSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = building.area;
        areaTemp.position = positionInt;

        gridSystem.SetTilesBlock(areaTemp, GridSystem.TilesType.White, gridSystem.mainTileMap);

        foreach (var pos in areaTemp.allPositionsWithin)
        {
            if (LandManager.current != null && LandManager.current.storedTilesArray != null)
            {
                foreach (var storedTiles in LandManager.current.storedTilesArray)
                {
                    if (storedTiles.ContainsKey(pos))
                    {
                        storedTiles[pos] = GridSystem.TilesType.White;
                    }
                }
            }
        }
        base.Kill();
    }

    public void PublicKill()
    {
        KillSell();
    }
}
