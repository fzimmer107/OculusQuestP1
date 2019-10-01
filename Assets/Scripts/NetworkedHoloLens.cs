using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedHoloLens : MonoBehaviourPun, IPunObservable
{

    public Transform playerGlobal;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Player is mine");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {


        //read stream from remote hololens avatar
        if(stream.IsReading)
        {
            Vector3 playerGlobalPos = (Vector3)stream.ReceiveNext();
            Quaternion playerGlobalRot = (Quaternion)stream.ReceiveNext();

            calculatePosition(playerGlobalPos, playerGlobalRot);
        }

    }


    private void calculatePosition(Vector3 globalPos, Quaternion globalRot)
    {
        playerGlobal.position = new Vector3(globalPos.x, globalPos.y, globalPos.z);
        playerGlobal.eulerAngles = new Vector3(globalRot.eulerAngles.x, globalRot.eulerAngles.y, globalRot.eulerAngles.z);

    }
}
