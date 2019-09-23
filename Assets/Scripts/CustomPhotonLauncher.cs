using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CustomPhotonLauncher : MonoBehaviourPunCallbacks, IOnEventCallback
{
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
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");

        GameObject localAvatar = Instantiate(Resources.Load("OVRPlayerControllerLocal"), new Vector3(0,0,0), Quaternion.identity) as GameObject;
        PhotonNetwork.Instantiate("GrabbableCube", new Vector3(0,0,3), Quaternion.identity);

        PhotonView photonView = localAvatar.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(photonView))
        {
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
            Debug.Log("Failed to allocate a ViewId");

            Destroy(localAvatar);
        }

    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == InstantiateVrAvatarEventCode)
        {
           GameObject remotAvatar = Instantiate(Resources.Load("VRAvatar"),new Vector3(1f,0f,1f),Quaternion.identity) as GameObject;
           PhotonView photonView = remotAvatar.GetComponent<PhotonView>();
           photonView.ViewID = (int)photonEvent.CustomData;
        }
    }

}
