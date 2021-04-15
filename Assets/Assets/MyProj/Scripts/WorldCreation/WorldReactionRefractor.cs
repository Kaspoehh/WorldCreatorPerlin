using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldReactionRefractor : MonoBehaviour
{
    [Header("Origin of the chunk in the perlin map")] [SerializeField]
    private float scale = 1.0F;

    [Header("World Logic")] [SerializeField]
    private Vector2 worldSize;


    [Header("Chunk Logic")] [SerializeField]
    private Vector2 chunkSize;

    private Dictionary<Vector2, Chunk> createdChunks;

    [Header("Ground")]
    [SerializeField] private GameObject normalGroundBlock;
    [SerializeField] private GameObject normalGroundBlockTundra;
    [SerializeField] private GameObject normalGroundBlockSaharah;
    [SerializeField] private GameObject normalGroundSandBlock;

    [Header("Water")] [SerializeField] private GameObject normalWaterBlock;

    [Header("Enviroment")] 
    [SerializeField] private GameObject normalTreeGameObject;
    [SerializeField] private GameObject normalFlowerGameObject;
    [SerializeField] private GameObject normalBushGameObject;
    [SerializeField] private List<GameObject> grassBlocks = new List<GameObject>();

    
    private bool _createdFirstChunk;
    
    public Dictionary<Vector2, Chunk> CreatedChunks => createdChunks;
    
    [Header("GLOBAL")]
    public List<RenderedChunkData> RenderedChunkData = new List<RenderedChunkData>();

    [Header("Player")] 
    [SerializeField] private GameObject playerPrefab;
    private GameObject _player;
    
    private void Awake()
    {
        createdChunks = new Dictionary<Vector2, Chunk>();

        for (int y = 0; y < worldSize.y; y++)
        {
            for (int x = 0; x < worldSize.x; x++)
            {
                float posX = x * chunkSize.x;
                float posY = y * chunkSize.y;
                
                Vector2 startPos = new Vector2(posX, posY);
                
                //Debug.Log(startPos);
                
                CreateChunk(startPos);

                // if (createdChunks.TryGetValue(new Vector2(posX, posY), out var chunk))
                // {
                //     CreateFirstChunk(chunk);
                // }
                // else
                // {
                //     Debug.Log("errorrrr");
                // }


            }
        }

        _player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        _player.GetComponent<PlayerWorldCreationManager>().worldCreator = this;
    }

    private void Start()
    {
        _player.transform.position = new Vector3(100, 100, 0);   
    }
    
    private void CreateChunk(Vector2 startPos)
    {
        Chunk newChunk = new Chunk();
        newChunk.VoxelsInChunkData = new Dictionary<Vector2, VoxelData>();

        for (int y = 0; y < chunkSize.y; y++)
        {
            for (int x = 0; x < chunkSize.x; x++)
            {

                Vector2 voxelPos = new Vector2(startPos.x + x, startPos.y + y);

                float yCoord = (float) voxelPos.x/  worldSize.x * scale;
                float xCoord = (float) voxelPos.y / worldSize.y * scale;

                float value = Mathf.PerlinNoise(xCoord, yCoord);

                VoxelData voxelData = new VoxelData();

                if (value > 0.5)
                {
                    int makeTree = Random.Range(0, 100);
                    int makeGrass = Random.Range(0, 50);
                    int makeFlower = Random.Range(0, 100);
                    int makeBush = Random.Range(0, 100);
                    
                    float valueBiome = Mathf.PerlinNoise(xCoord + 521F, yCoord + 2314F);

                    if (valueBiome > 0.6F)
                    {
                        voxelData.BiomeType = BiomeTypes.Green;
                    }
                    else if (valueBiome > 0.3)
                    {
                        voxelData.BiomeType = BiomeTypes.Saharah;
                    }
                    else
                    {
                        voxelData.BiomeType = BiomeTypes.Tundra;
                    }

                    #region Make Props

                    if (makeTree == 1 && voxelData.BiomeType != BiomeTypes.Saharah)
                    {
                        voxelData.Tree = true;
                    }
                    else if (makeGrass == 1)
                    {
                        voxelData.Grass = true;
                    }
                    else if (makeFlower == 1)
                    {
                        voxelData.Flower = true;
                    }
                    
                    else if (makeBush == 1)
                    {
                        voxelData.Bush = true;
                    }
                    
                    #endregion 
                
                    voxelData.Land = true;
                }
                else if (value > 0.49)
                {
                    voxelData.Sand = true;
                    voxelData.Tree = false;
                    voxelData.Grass = false;
                    voxelData.Flower = false;
                    voxelData.Bush = false;
                }
                else
                {
                    voxelData.Water = true;
                }

                newChunk.VoxelsInChunkData.Add(voxelPos, voxelData);
            }
        }

        createdChunks.Add(startPos, newChunk);

    }

    #region OldCoroutine
    IEnumerator CreateChunkNew(Chunk chunk, Vector2 startPos)
    {

        GameObject chunkParentBlock = new GameObject();

        chunkParentBlock.name = "Chunk: " + startPos.x + " / " + startPos.y;
        
        RenderedChunkData renderedChunkData = new RenderedChunkData();
        renderedChunkData.ChunkGameObject = chunkParentBlock;
        renderedChunkData.Position = startPos;
        
        RenderedChunkData.Add(renderedChunkData);
        
        for (int y = 0; y < chunkSize.y; y++)
        {
            for (int x = 0; x < chunkSize.x; x++)
            {
                Vector2 checkPos = new Vector2(startPos.x + x, startPos.y + y);

                if (chunk.VoxelsInChunkData.TryGetValue(checkPos, out var voxelData))
                {
                    GameObject temp;

                    if (voxelData.Water)
                    {
                        //Spawn Water
                        temp = normalWaterBlock;
                    }
                    else if (voxelData.Sand)
                    {
                        temp = normalGroundSandBlock;
                    }
                    else
                    {
                        temp = normalGroundBlock;
                    }

                    GameObject go = Instantiate(temp, checkPos, Quaternion.identity);
                    
                    
                    go.transform.SetParent(chunkParentBlock.transform);

                    GameObject Tree;
                    
                    if (voxelData.Tree && voxelData.Land)
                    {
                        Tree = normalTreeGameObject;
                        GameObject goTree = Instantiate(Tree, checkPos, Quaternion.identity);
                        goTree.transform.SetParent(go.transform);
                    }
                    
                    GameObject Grass;
                    
                    if (voxelData.Grass && voxelData.Land)
                    {
                        var index = Random.Range(0, grassBlocks.Count);
                        Grass = grassBlocks[index];
                        GameObject goGrass = Instantiate(Grass, checkPos, Quaternion.identity);
                        goGrass.transform.SetParent(go.transform);
                    }
                    
                    GameObject Flower;
                    
                    if (voxelData.Flower && voxelData.Land)
                    {
                        Flower = normalFlowerGameObject;
                        GameObject goFlower = Instantiate(Flower, checkPos, Quaternion.identity);
                        goFlower.transform.SetParent(go.transform);
                    }
                    
                    GameObject Bush;
                    
                    if (voxelData.Bush && voxelData.Land)
                    {
                        Bush = normalBushGameObject ;
                        GameObject goBush = Instantiate(Bush, checkPos, Quaternion.identity);
                        goBush.transform.SetParent(go.transform);
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }


    }
#endregion
    
    private void CreateChunkSpawn(Chunk chunk, Vector2 startPos)
    {
        GameObject chunkParentBlock = new GameObject();

        chunkParentBlock.name = "Chunk: " + startPos.x + " / " + startPos.y;
        
        RenderedChunkData renderedChunkData = new RenderedChunkData();
        renderedChunkData.ChunkGameObject = chunkParentBlock;
        renderedChunkData.Position = startPos;
        
        RenderedChunkData.Add(renderedChunkData);
        
        for (int y = 0; y < chunkSize.y; y++)
        {
            for (int x = 0; x < chunkSize.x; x++)
            {
                Vector2 checkPos = new Vector2(startPos.x + x, startPos.y + y);

                if (chunk.VoxelsInChunkData.TryGetValue(checkPos, out var voxelData))
                {
                    GameObject temp;

                    if (voxelData.Water)
                    {
                        //Spawn Water
                        temp = normalWaterBlock;
                    }
                    else if (voxelData.Sand)
                    {
                        temp = normalGroundSandBlock;
                    }
                    else
                    {
                        if(voxelData.BiomeType == BiomeTypes.Green)
                            temp = normalGroundBlock;
                        else if (voxelData.BiomeType == BiomeTypes.Saharah)
                            temp = normalGroundBlockSaharah;
                        else if (voxelData.BiomeType == BiomeTypes.Tundra)
                            temp = normalGroundBlockTundra;
                        else
                        {
                            Debug.LogError("Biome not set on voxel!!!!");
                            temp = null;
                        }
                    }

                    GameObject go = Instantiate(temp, checkPos, Quaternion.identity);
                    
                    
                    go.transform.SetParent(chunkParentBlock.transform);

                    GameObject Tree;
                    
                    if (voxelData.Tree && voxelData.Land)
                    {
                        Tree = normalTreeGameObject;
                        GameObject goTree = Instantiate(Tree, checkPos, Quaternion.identity);
                        goTree.transform.SetParent(go.transform);
                    }
                    
                    GameObject Grass;
                    
                    if (voxelData.Grass && voxelData.Land)
                    {
                        var index = Random.Range(0, grassBlocks.Count);
                        Grass = grassBlocks[index];
                        GameObject goGrass = Instantiate(Grass, checkPos, Quaternion.identity);
                        goGrass.transform.SetParent(go.transform);
                    }
                    
                    GameObject Flower;
                    
                    if (voxelData.Flower && voxelData.Land)
                    {
                        Flower = normalFlowerGameObject;
                        GameObject goFlower = Instantiate(Flower, checkPos, Quaternion.identity);
                        goFlower.transform.SetParent(go.transform);
                    }
                    
                    GameObject Bush;
                    
                    if (voxelData.Bush && voxelData.Land)
                    {
                        Bush = normalBushGameObject ;
                        GameObject goBush = Instantiate(Bush, checkPos, Quaternion.identity);
                        goBush.transform.SetParent(go.transform);
                    }
                }
            }
        }


    }
    
    public void SpawnChunk(Chunk chunk)
    {
        if (!chunk.ChunkSpawned)
        {
            chunk.ChunkSpawned = true;
            CreateChunkSpawn(chunk, chunk.VoxelsInChunkData.Keys.First());
        }
        else if (chunk.ChunkSpawned)
        {
           // if(RenderedChunks.TryGetValue(chunk.VoxelsInChunkData.Keys.First(), out var chunkToActivate))
           // {
           //     if (chunkToActivate.active)
           //     {
           //         return;
           //     }
           //     chunkToActivate.SetActive(true);
           //     RenderdChunksVector2s.Add(chunk.VoxelsInChunkData.Keys.First());
           // }
        }
    }
}

[Serializable]
public class VoxelData
{
    public bool Water;
    public bool Land;
    public bool Sand;


    public bool Grass;
    public bool Tree;
    public bool Flower;
    public bool Bush;
    
    public BiomeTypes BiomeType;    
    
    //public Vector2 SpawnPos;
}

[Serializable]
public class Chunk
{
    public bool ChunkSpawned;
    public Dictionary<Vector2, VoxelData> VoxelsInChunkData;
}

[Serializable]
public class RenderedChunkData
{
    public GameObject ChunkGameObject;
    public Vector2 Position;
}

public enum BiomeTypes
{
    Green,
    Saharah,
    Tundra
}