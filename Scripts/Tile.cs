using Godot;
using System;
using System.Dynamic;

public partial class Tile : Godot.Panel
{
    private byte TileIndex { get; set; }
    private bool IsCaptured { get; set; } = false;
    
    [Export]
    private static float TileSize { get; set; } = 83F;
    
    [Export]
    private static Vector2 Gap { get; set; } = new Vector2(8, 8);


    public override void _Ready()
    {
        this.Size = new Vector2(TileSize, TileSize);
    }

    public void SetTileIndex(byte n)
    {
        this.TileIndex = n;
    }
    public byte GetTileIndex()
    {
        return this.TileIndex;
    }

    public static Vector2 GetGap()
    {
        return Gap;
    }
    public static float GetSize()
    {
        return TileSize;
    }

    public bool GetIsCaptured()
    {
        return IsCaptured;

    }

    public void SetIsCaptured(bool Value)
    {
        IsCaptured = Value;
        return;
    }

    public bool IsClicked(Vector2 mousePos)
    {
        return this.GetGlobalRect().HasPoint(mousePos);
    }



}
