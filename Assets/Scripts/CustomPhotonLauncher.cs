using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;



public class CustomPhotonLauncher : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public Transform SpawnPoint;
    private string gameVersion = "1";
    public const byte InstantiateVrAvatarEventCode = 1;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.LocalPlayer.NickName = "VR";
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }


    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(":OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions());
    }



    public override void OnJoinedRoom()
    {

        /// <summary>
        /// Content of this class was created with help of a tutorial from: https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/oculusavatarsdk
        /// </summary>

        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
        //Instantiate own Avatar
        GameObject localAvatar = Instantiate(Resources.Load("VREmpty"), new Vector3(SpawnPoint.position.x,SpawnPoint.position.y, SpawnPoint.position.z), Quaternion.identity) as GameObject;
        PhotonNetwork.Instantiate("GrabbableCube", new Vector3(0,0,3), Quaternion.identity);

        PhotonView photonView = localAvatar.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(photonView))
        {
            //if we allocated an ID for the view, send an event, so an avatar gets intantiated on other clients
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(InstantiateVrAvatarEventCode, photonView.ViewID, raiseEventOptions, sendOptions);
        }

        else
        {
            //we failed to allocate a view, destroy local avatar
            Debug.Log("Failed to allocate a ViewId");

            Destroy(localAvatar);
        }

    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == InstantiateVrAvatarEventCode)
        {
            //this client only instantiates holoLens user avatars
           GameObject remotAvatar = Instantiate(Resources.Load("HoloLensAvatar2"),new Vector3(0f,0f,0f),Quaternion.identity) as GameObject;
           PhotonView photonView = remotAvatar.GetComponent<PhotonView>();
           photonView.ViewID = (int)photonEvent.CustomData;
        }
    }

}
