using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecieverTest : MonoBehaviour
{

    private PhotonView RPCPhotonView;
    private Dictionary<int, MeshFilter> recievedMeshes = new Dictionary<int, MeshFilter>();
    //private List<int> RecievedMeshesID = new List<int>();
    public GameObject meshPrefab;

    // Start is called before the first frame update
    void Start()
    {
        RPCPhotonView = GetComponent<PhotonView>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    [PunRPC]
    public void RPCTest(string textMessage)
    {
        Debug.Log($" Recieved message: {textMessage}");
    }

    [PunRPC]
    public void RecieveMeshData(Vector3[] vertices, int[] triangles, Vector2[] uvs, int meshId)
    {
        if(!recievedMeshes.ContainsKey(meshId))
            {
            Debug.Log($"Building new mesh with ID: {meshId}");
            GameObject prefab = Instantiate(meshPrefab) as GameObject;
            prefab.name = meshId.ToString();

            Mesh recievedMesh = new Mesh();
            recievedMesh.vertices = vertices;
            recievedMesh.triangles = triangles;
            recievedMesh.uv = uvs;

            MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
            meshFilter.mesh = recievedMesh;
            recievedMeshes.Add(meshId, meshFilter);
        }
        else
        {
            Debug.Log($"Mesh with ID: {meshId} already recieved. Calling UpdateMeshData");
            UpdataMeshData(vertices, triangles, meshId);
        }

        //Debug.Log($"ReciveMeshData called. Getting {vertices.Length} vertices, {triangles.Length} triangles, {uvs.Length} uvs and ID: {meshId}");

    }

    private void UpdataMeshData(Vector3[] vertices, int[] triangles, int meshId)
    {
        recievedMeshes[meshId].mesh.Clear();
        recievedMeshes[meshId].mesh.vertices = vertices;
        recievedMeshes[meshId].mesh.triangles = triangles;
        Debug.Log($"Updated Mesh with ID: {meshId}");

    }


    [PunRPC]
    public void SendError(string message)
    {
        Debug.Log(message);
    }

    /*    [PunRPC]
        public void RecieveMeshData(Vector3[] vertices, Vector2[] uvs, int[] triangles, int meshId)
        {    
            if(!RecievedMeshesID.Contains(meshId))
            {
                RecievedMeshesID.Add(meshId);
                //create a prefab, which contains the nescessary Components MeshFilter and MeshRenderer and attach it to Reciever
                GameObject prefab = Instantiate(meshPrefab) as GameObject;
                //assign the id of the recieved mesh, so we can identify it later for updates
                prefab.name = meshId.ToString();

                //create a new Mesh, assign recieved Mesh data and assign the mesh to the prefab component
                Mesh recievedMesh = new Mesh();
                recievedMesh.vertices = vertices;
                recievedMesh.uv = uvs;
                recievedMesh.triangles = triangles;
                prefab.GetComponent<MeshFilter>().mesh = recievedMesh;
            }
            else
            {
                Debug.Log($"Already recieved mesh with ID {meshId}");
            }


        }

        [PunRPC]
        public void UpdateMesh(Vector3[] vertices, Vector2[] uvs, int[] triangles, int meshId)
        {
            Debug.Log($"Trying to Update mesh with ID: {meshId}");
            //get the MeshFilter Component and delete the mesh
            MeshFilter meshFilter = GameObject.Find("Reciever/" + meshId.ToString()).GetComponent<MeshFilter>();
            Destroy(meshFilter.mesh);

            //create new Mesh with updated data and assign it to the MeshFilterComponent
            Mesh meshUpdate = new Mesh();
            meshUpdate.vertices = vertices;
            meshUpdate.uv = uvs;
            meshUpdate.triangles = triangles;
            meshFilter.mesh = meshUpdate;
            Debug.Log($"Updated mesh with ID: {meshId}");

        } */

    [PunRPC]
    public void RemoveMesh(int meshId)
    {
        Destroy(GameObject.Find(meshId.ToString()));
        Debug.Log($"Destroyed GameObject with ID: {meshId}");
        recievedMeshes.Remove(meshId);

    }
}
