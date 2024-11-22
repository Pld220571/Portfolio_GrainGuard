using UnityEngine;

public class TowerHealth : Health
{
    private GridSystem _gridSystem;
    private Building _building;
    private Notifications _notifications;

    public override void Start()
    {
        _building = FindObjectOfType<Building>();
        _gridSystem = FindObjectOfType<GridSystem>();
        _notifications = FindObjectOfType<Notifications>();
        base.Start();
    }

    protected override void CheckHealth()
    {
        base.CheckHealth();
    }

    protected override void Kill()
    {
        Vector3Int positionInt = GridSystem.current.gridLayout.LocalToCell(transform.position); // Convert the tower's position to grid coordinates
        BoundsInt areaTemp = _building.area; // Get the area occupied by the building
        areaTemp.position = positionInt; // Set the area position to the tower's grid position
        _gridSystem.SetTilesBlock(areaTemp, GridSystem.TilesType.Destroyed, _gridSystem.mainTileMap); // Set the tiles in the grid to indicate that they are destroyed
        Destroyed(); // Call the Destroyed method to update stored tiles
        _notifications.BuyBackGrid(); // Notify the user about the buy-back option
        base.Kill();
    }
    
    private void Destroyed() // Method to handle the destruction of the tower and update the stored tiles
    {
        Vector3Int positionInt = GridSystem.current.gridLayout.LocalToCell(transform.position); // Convert the tower's position to grid coordinates
        BoundsInt areaTemp = _building.area; // Get the area occupied by the building
        areaTemp.position = positionInt; // Set the area position to the tower's grid position

        foreach (var pos in areaTemp.allPositionsWithin) // Iterate through all positions within the building's area
        {
            if (LandManager.current != null && LandManager.current.storedTilesArray != null) // Check if the LandManager and stored tiles are available
            {
                foreach (var storedTiles in LandManager.current.storedTilesArray) // Update the stored tiles to indicate they are destroyed
                {
                    if (storedTiles.ContainsKey(pos))
                    {
                        storedTiles[pos] = GridSystem.TilesType.Destroyed; // Set the tile type to Destroyed
                    }
                }
            }
        }
    }

    protected void KillSell() // Method to handle the selling of the tower and update the grid accordingly
    {
        Vector3Int positionInt = GridSystem.current.gridLayout.LocalToCell(transform.position); // Convert the tower's position to grid coordinates
        BoundsInt areaTemp = _building.area; // Get the area occupied by the building
        areaTemp.position = positionInt; // Set the area position to the tower's grid position
        _gridSystem.SetTilesBlock(areaTemp, GridSystem.TilesType.White, _gridSystem.mainTileMap); // Set the tiles in the grid to indicate they are available (White)

        foreach (var pos in areaTemp.allPositionsWithin) // Iterate through all positions within the building's area
        {
            if (LandManager.current != null && LandManager.current.storedTilesArray != null) // Check if the LandManager and stored tiles are available
            {
                foreach (var storedTiles in LandManager.current.storedTilesArray) // Update the stored tiles to indicate they are available
                {
                    if (storedTiles.ContainsKey(pos))
                    {
                        storedTiles[pos] = GridSystem.TilesType.White; // Set the tile type to White
                    }
                }
            }
        }

        base.Kill();
    }

    public void PublicKill() // Public method to allow external calls to kill and sell the tower
    {
        KillSell(); // Call the KillSell method to handle the selling process
    }
}