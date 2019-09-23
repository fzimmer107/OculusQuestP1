using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// This class writes all necessary position and rotation information
/// we need to display and update the local VR users head and hand position and rotation
/// on a remote client
/// </summary>
public class NetworkedPlayer : MonoBehaviourPun, IPunObservable

{

    //these are the Transforms which change when position or rotation of
    // VR users head or hands change
    public Transform playerGlobal;  //changes if VR user navigates with the Oculus Touch Controller
    public Transform playerLocal;   //changes if VR user moves/rotates his head
    public Transform leftHand;      //changes if VR user moves/rotates left controller  
    public Transform rightHand;     //changes if VR user moves/rotates right controller

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            Debug.Log("Player is mine");
        }
    }

    /// <summary>
    /// Sends position and rotation data, if changes occur
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //we only need to write data to update the position and rotation of the VR user and 
        //his hands. There is no need for reading of the stream

        if (stream.IsWriting)
        {
            //global position of the player
            stream.SendNext(playerGlobal.position);

            //local position/rotation of the player
            stream.SendNext(playerLocal.localPosition);
            stream.SendNext(playerLocal.localRotation);

            //left hand local position/rotation
            stream.SendNext(leftHand.localPosition);
            stream.SendNext(leftHand.rotation);

            //right hand local position/rotation
            stream.SendNext(rightHand.localPosition);
            stream.SendNext(rightHand.rotation);
        }
    }
}
