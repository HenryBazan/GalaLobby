using UnityEngine;
using Photon.Pun;
public class DiamondSpawnerOnline : MonoBehaviour
{
    public GameObject diamondPrefab;
    public int numberOfDiamonds = 4;
    public static Bounds combinedBounds;
    public float margin = 0.5f;

    void Start()
    {
      if (!PhotonNetwork.IsMasterClient){
        return;
      }
      Debug.LogError("Network diamond Spawner called on master client only");
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Walls");

        if (walls.Length == 0)
        {
            Debug.LogError("No objects with tag 'Walls' found.");
            return;
        }

        CalculateCombinedBounds(walls);
        PlaceDiamonds();
    }

    void CalculateCombinedBounds(GameObject[] walls)
    {
      Debug.Log("CalculateCombinedBounds for network diamond spwaner is working");
        Renderer firstWallRenderer = walls[0].GetComponent<Renderer>();
        combinedBounds = firstWallRenderer.bounds;

        foreach (GameObject wall in walls)
        {
            Renderer wallRenderer = wall.GetComponent<Renderer>();
            if (wallRenderer != null)
            {
                combinedBounds.Encapsulate(wallRenderer.bounds);
            }
        }

        combinedBounds.Expand(new Vector3(-margin * 2, -margin * 2, 0));
    }

    void PlaceDiamonds()
    {
      Debug.Log("PlaceDiamonds called");
        //GameObject[] diamonds = GameObject.FindGameObjectsWithTag("Diamond");

        // if (diamonds.Length < numberOfDiamonds)
        // {
        //     int toInstantiate = numberOfDiamonds - diamonds.Length;
            for (int i = 0; i < numberOfDiamonds; i++)
            {
              Debug.Log("Placing Diamonds");
                PhotonNetwork.Instantiate("DiamondPrefab/Diamond", GetRandomPositionWithinBounds(), Quaternion.identity);
            }

            //diamonds = GameObject.FindGameObjectsWithTag("Diamond");
        }

        // for (int i = 0; i < numberOfDiamonds; i++)
        // {
        //     if (i < diamonds.Length)
        //     {
        //         diamonds[i].transform.position = GetRandomPositionWithinBounds();
        //     }
        // }


    public void RespawnDiamond(GameObject diamondPrefab)
    {
        diamondPrefab.GetComponent<Collider2D>().enabled = true;
        diamondPrefab.GetComponent<Rigidbody2D>().isKinematic = true;
        diamondPrefab.transform.SetParent(null);
        Debug.Log($"Respawning diamond within the bounds :Min {combinedBounds.min}, Max {combinedBounds.max}");
        diamondPrefab.transform.position = GetRandomPositionWithinBounds();
    }

    Vector3 GetRandomPositionWithinBounds()
    {
        float x = Random.Range(combinedBounds.min.x, combinedBounds.max.x);
        float y = Random.Range(combinedBounds.min.y, combinedBounds.max.y);
        return new Vector3(x, y, 0f);
    }
}
