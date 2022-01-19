using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform playerHead;
    public GameObject Player;
    public Camera Cam;
    void Update() {
        transform.position = playerHead.transform.position;
    }
}
