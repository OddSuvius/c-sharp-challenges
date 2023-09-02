using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Godot.Node
{
    private byte TieCounter = 0;
    Player PlayerX; Player PlayerO;
    private bool PlayerXTurn = true; // true is Player 'X' false is Player 'O'
    public Vector2 WindowCenterOffset;
    private Node GameInstance;
    private bool GameIsFinished = false;
    private PackedScene TileScene;
    private List<Tile> Tiles = new List<Tile> { };
    private struct Player
    {
        public Player() { }
        public bool[] ClaimedTiles = new bool[]{
                false, false, false,
                false, false, false,
                false, false, false
            };
    }

    public override void _Ready()
    {
        GetWindow().Set("unresizable", true);
        WindowCenterOffset = (GetWindow().Size - new Vector2(Tile.GetSize(), Tile.GetSize()) * 3 - Tile.GetGap() * 3) / 2;
        TileScene = ResourceLoader.Load<PackedScene>("res://Tile.tscn");
        LoadGame();
    }

    public override void _Process(double delta)
    {
        PlayerInput();
    }

    public void LoadGame()
    {
        PlayerX = new Player();
        PlayerO = new Player();
        GameInstance = new Node();
        GameIsFinished = false;
        // generating 3x3 tiles 
        byte r = 0;
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                Tile TileInstance = (Tile)TileScene.Instantiate();
                TileInstance.SetTileIndex(r);
                TileInstance.Name = $"Tile{TileInstance.GetTileIndex()}";
                TileInstance.Position = (new Vector2(x, y) * Tile.GetSize()) + (Tile.GetGap() * new Vector2(x, y)) + WindowCenterOffset;
                GameInstance.AddChild(TileInstance);
                this.Tiles.Add(TileInstance);
                r++;
            }
        }
        this.AddChild(GameInstance);
    }

    private void UnloadGame()
    {
        this.TieCounter = 0;
        this.GetNode<Label>("%Winner").Visible = false;
        this.PlayerXTurn = true;
        this.GetNode<Label>("%CurrentPlayer").Text = "Current Player: X";
        this.Tiles.Clear();
        this.RemoveChild(GameInstance);
    }
    public void PlayerInput()
    {
        if (Input.IsActionJustPressed("MouseClick"))
        {
            if (GameIsFinished)
            {
                UnloadGame();
                LoadGame();
                return;
            }
            foreach (var item in Tiles)
            {   // it's nested :d
                if (item.IsClicked(GetViewport().GetMousePosition()))
                {
                    GD.Print(item.Name);
                    if (item.GetIsCaptured())
                    {
                        return;
                    }
                    item.SetIsCaptured(true);
                    if (PlayerXTurn) // hardcoding :D
                    {
                        TieCounter++;
                        PlayerX.ClaimedTiles.SetValue(true, (int)item.GetTileIndex());
                        ((Label)item.GetNode("Label")).Text = "X";
                        this.GetNode<Label>("%CurrentPlayer").Text = "Current Player: O";
                        if (WinCalculator.checkWin(PlayerX.ClaimedTiles))
                        {
                            GameIsFinished = true;
                            this.GetNode<Label>("%Winner").Visible = true;
                            this.GetNode<Label>("%Winner").Text = "Player X WON!";
                        }
                        // if tie happends
                        if (TieCounter == 5 && !GameIsFinished)
                        {
                            this.GetNode<Label>("%Winner").Visible = true;
                            this.GetNode<Label>("%Winner").Text = "TIE!";
                            GameIsFinished = true;
                        }
                    }
                    else
                    {
                        PlayerO.ClaimedTiles.SetValue(true, (int)item.GetTileIndex());
                        ((Label)item.GetNode("Label")).Text = "O";
                        this.GetNode<Label>("%CurrentPlayer").Text = "Current Player: X";
                        if (WinCalculator.checkWin(PlayerO.ClaimedTiles))
                        {
                            GameIsFinished = true;
                            this.GetNode<Label>("%Winner").Visible = true;
                            this.GetNode<Label>("%Winner").Text = "Player O WON!";
                        }
                    }
                    PlayerXTurn = !PlayerXTurn;
                    return;
                }

            }
        }
        return;
    }
}









public static partial class WinCalculator
{

    public static bool checkWin(bool[] currentTable)
    {
        for (int x = 0; x < winTable.Length; x++)
        {
            byte d = 0;
            for (int i = 0; i < 3; i++)
            {
                if (currentTable[winTable[x][i]] == false)
                {
                    break;
                }
                d++;
                if (d == 3)
                {
                    return true;
                }
            };
        }
        return false;
    }

    private static byte[][] winTable = {
        // all vertical wins
        new byte[]{0, 1, 2},
        new byte[]{3, 4, 5},
        new byte[]{6, 7, 8},
        // all horizontal wins
        new byte[]{0, 3, 6},
        new byte[]{1, 4, 7},
        new byte[]{2, 5, 8},
        // all diagonal wins
        new byte[]{0, 4, 8},
        new byte[]{2, 4, 6}
        };

}