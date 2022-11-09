
using System.Collections.Generic;
using UnityEngine;

public class Pixel {
    public float A = 1.0f; 
    public float B = 0.0f;
}

public class ReactionDiffusion : MonoBehaviour {
    private int width = 200;
    private int height = 200;
    
    private float diffuseA = 1f;
    private float diffuseB = 0.3f;
    
    private float feed = 0.055f;
    private float kill = 0.062f;
    
    List<List<Pixel>> grid;
    List<List<Pixel>> gridBack;
    private Texture2D texture;
    

    
    void Start()
    {
        grid = new List<List<Pixel>>();
        gridBack = new List<List<Pixel>>();
        texture = new Texture2D(width, height);
        GetComponent<Renderer>().material.mainTexture = texture;

        for (int i = 0; i < width; i++)
        {
            var list = new List<Pixel>();
            var listBack = new List<Pixel>();
            for (int j = 0; j < height; j++)
            {
                list.Add(new Pixel());
                listBack.Add(new Pixel());
            }
            grid.Add(list);
            gridBack.Add(listBack);
        }

        SetSomeRandomPixBlack(50);
    }

    
    void Update()
    {
        Iterate();
        UpdateTexture();
    }

    void Iterate()
    {
        for (int x = 1; x < width -1; x++)
        {
            for (int y = 1; y < height -1; y++)
            {
                var a = grid[x][y].A;
                var b = grid[x][y].B;

                gridBack[x][y].A = Mathf.Clamp01(
                    a +
                    (diffuseA * Laplace(x, y, false)) -
                    (a * b * b) +
                    (feed * (1 - a)));
                gridBack[x][y].B = Mathf.Clamp01(
                    b +
                    (diffuseB * Laplace(x, y, true)) +
                    (a * b * b) -
                    ((kill + feed) * b));
            }
        }

        var temp = grid;
        grid = gridBack;
        gridBack = temp;
    }
    
    void UpdateTexture()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float value = grid[x][y].A - grid[x][y].B;
                Color color = new Color(grid[x][y].A, value, grid[x][y].B, 1);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    float Laplace(int x, int y, bool isBComponent)
    {
        float sum = 0;
        if (isBComponent)
        {
            sum += grid[x][y].B * -1f;
            sum += grid[x - 1][y].B * 0.2f;
            sum += grid[x + 1][y].B * 0.2f;
            sum += grid[x][y + 1].B * 0.2f;
            sum += grid[x][y - 1].B * 0.2f;
            sum += grid[x - 1][y - 1].B * 0.05f;
            sum += grid[x + 1][y - 1].B * 0.05f;
            sum += grid[x + 1][y + 1].B * 0.05f;
            sum += grid[x - 1][y + 1].B * 0.05f;
            return sum * Time.deltaTime * 60;
        }

        sum += grid[x][y].A * -1f;
        sum += grid[x][y + 1].A * 0.2f;
        sum += grid[x - 1][y].A * 0.2f;
        sum += grid[x][y - 1].A * 0.2f;
        sum += grid[x + 1][y].A * 0.2f;
        sum += grid[x - 1][y - 1].A * 0.05f;
        sum += grid[x + 1][y - 1].A * 0.05f;
        sum += grid[x + 1][y + 1].A * 0.05f;
        sum += grid[x - 1][y + 1].A * 0.05f;
        return sum * Time.deltaTime * 60;
    }

    void SetSomeRandomPixBlack(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int x = Random.Range(1, width -1);
            int y = Random.Range(1, height -1);
            grid[x][y].B = 1.0f;
            grid[x-1][y].B = 1.0f;
            grid[x][y-1].B = 1.0f;
            grid[x+1][y].B = 1.0f;
            grid[x][y+1].B = 1.0f;
            
        }
    }
}
