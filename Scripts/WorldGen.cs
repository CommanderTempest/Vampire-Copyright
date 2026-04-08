using Godot;
using System;
using System.Linq;

public partial class WorldGen : Node2D
{
    [Export]
    public NoiseTexture2D noise_texture;

    public TileMap tile_map;

    public int source_id = 0;
    public Vector2I water_atlas;
    public Vector2I land_atlas;

    public int width = 100;
    public int height = 100;

    private float[] noise_val_array;
    private Noise noise;

    public void _ready()
    {
        this.tile_map = GetNode<TileMap>("TileMap");
        this.noise = this.noise_texture.Noise;
    }

    public void generate_map()
    {
        for (int i=-width/2; i<width/2;i++)
        {
            for(int x=-height/2; x<height/2; x++)
            {
                float noise_value = this.noise.GetNoise2D(i,x);
                if (noise_value >= 0.0)
                {
                    tile_map.SetCell(0, new Vector2I(i,x), source_id, land_atlas);
                }
                else if (noise_value < 0.0)
                {
                    tile_map.SetCell(0, new Vector2I(i,x), source_id, water_atlas);
                }
                noise_val_array.Append(noise_value);
            }
            
        }
    }
}
