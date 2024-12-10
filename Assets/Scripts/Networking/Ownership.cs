using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class DiamondLogic : MonoBehaviour
{
    public bool IsClaimed { get; set; }

    [PunRPC]
    public void SyncDiamondPosition(Vector3 newPosition){
      transform.position = newPosition;
    }
}
