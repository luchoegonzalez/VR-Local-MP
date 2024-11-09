using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
///  Clase que desactiva la camara del objeto spawneado, si el cliente no esta jugando con el.
///  Si el cliente esta jugando con el objeto, se elimina la camara principal de la escena.
/// </summary>
public class MainCameraCheck : NetworkBehaviour {

    [SerializeField] private Camera InnerCamera;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        Debug.Log("Cliente: " + IsLocalPlayer);

        // !IsLocalPlayer, esto quiere decir que en ese cliente no se esta jugando con este objeto
        if (!IsLocalPlayer) {
            if (InnerCamera == null) {
                Debug.LogError("No se ha asignado la camara interna");
                return;
            } else {
                Debug.Log("Eliminando camara interna");
                DeleteInnerCamera();
            }
        } else {
            Debug.Log("Eliminando camara externa");
            DeleteOuterCamera();
        }

    }

    private void DeleteInnerCamera() {
        InnerCamera.gameObject.SetActive(false);
    }

    // Eliminamos la camara principal de la escena, porque el VR y el Host ya tienen su propia camara
    private void DeleteOuterCamera() {
        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject rootObj in activeScene.GetRootGameObjects()) {
            Camera camera = rootObj.GetComponent<Camera>();
            if (camera != null) {
                Destroy(rootObj);
                break;
            }
        }
    }

}
