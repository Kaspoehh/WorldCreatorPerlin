using UnityEngine;

public class PlayerWorldCreationManager : MonoBehaviour
{
    private Vector2 currentChunk;

    public WorldReactionRefractor worldCreator;

 
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        
        float posX = RoundDown((int)this.transform.position.x);

        float posY = RoundDown((int)this.transform.position.y);
        
        if (currentChunk.x != posX)
        {
            currentChunk.x = posX;
            for (int y = -2; y < 2; y++)
            {
                for (int x = -2; x < 2; x++)
                {
                    float yPos = posY + (y * 10);
                    float xPos = posX + (x * 10);
                    
                    if(worldCreator.CreatedChunks.TryGetValue(new Vector2(xPos, yPos), out var chunk))
                    {
                        worldCreator.SpawnChunk(chunk);
                    }
                }
            }
        }

        if (currentChunk.y != posY)
        {
            currentChunk.y = posY;
            
            for (int y = -2; y < 2; y++)
            {
                for (int x = -2; x < 2; x++)
                {
                    float yPos = posY + (y * 10);
                    float xPos = posX + (x * 10);
                    
                    if(worldCreator.CreatedChunks.TryGetValue(new Vector2(xPos, yPos), out var chunk))
                    {
                        worldCreator.SpawnChunk(chunk);
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < worldCreator.RenderedChunkData.Count; i++)
        {
            float distance = Vector2.Distance(this.transform.position, worldCreator.RenderedChunkData[i].Position);
            if (distance > 40)
            {
                Destroy(worldCreator.RenderedChunkData[i].ChunkGameObject);
                
                if(worldCreator.CreatedChunks.TryGetValue(worldCreator.RenderedChunkData[i].Position, out var chunk))
                {
                    chunk.ChunkSpawned = false;
                }
                
                worldCreator.RenderedChunkData.RemoveAt(i);    
            }
        }
    }
    int RoundDown(int toRound)
    {
        return toRound - toRound % 10;
    }
}
