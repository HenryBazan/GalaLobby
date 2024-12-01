using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class Movement : MonoBehaviourPunCallbacks
{
  private Rigidbody2D rb2D;
  public float speed = 5f;
  private PhotonView PV;
    // Start is called before the first frame update
    private void Start()
    {
      rb2D = GetComponent<Rigidbody2D>();
      PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
      if (PV.IsMine){
        TakeInput();
      }
    }

    private void TakeInput()
    {
      transform.rotation = Quaternion.Euler(0, 0, 0);
      float moveX = Input.GetAxis("Horizontal");
      float moveY = Input.GetAxis("Vertical");

      Vector2 movement = new Vector2(moveX, moveY).normalized;

      rb2D.velocity = movement * speed;
    }
}
