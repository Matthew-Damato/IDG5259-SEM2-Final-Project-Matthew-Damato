using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject mapTile;
    [SerializeField] private List<GameObject> mapTiles = new List<GameObject>();

    [SerializeField] private int mapHeight;
    [SerializeField] private int mapWidth;


    // Start is called before the first frame update
    void Start()
    {
        generateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void generateMap()
    {
        for(int z = 0; z < mapHeight; z++) 
        {
            for(int x = 0; x < mapWidth; x++)
            {
                GameObject newTile = Instantiate(mapTile);
                mapTiles.Add(newTile);
                newTile.transform.position = new Vector3(x, 0, z);
            }
        }

        List<GameObject> TopTiles = TopEdgeTiles();
        List<GameObject> BottomTiles = BottomEdgeTiles();

        GameObject StartTile;
        GameObject EndTile;

        int R1 = Random.Range(0, mapWidth);
        int R2 = Random.Range(0, mapWidth);

        StartTile = TopTiles[R1];
        EndTile = BottomTiles[R2];
        Destroy(StartTile);
        Destroy(EndTile);
    }


    private List<GameObject> TopEdgeTiles()
    {
        List<GameObject> edgeTiles = new List<GameObject>(); ;

        for (int i = mapWidth * (mapHeight - 1); i < mapWidth * mapHeight; i++)
        {
            edgeTiles.Add(mapTiles[i]);
        }

        return edgeTiles;
    }



    private List<GameObject> BottomEdgeTiles()
    {
        List<GameObject> edgeTiles = new List<GameObject>();
        for (int i = 0; i < mapWidth; i++)
        {
            edgeTiles.Add(mapTiles[i]);
        }
        return edgeTiles;
    }
}
