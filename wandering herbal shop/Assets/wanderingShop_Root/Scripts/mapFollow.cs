using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapFollow : MonoBehaviour
{

    [SerializeField] Transform player;
    private void LateUpdate()
    {
        transform.position = new Vector3 (player.position.x, 29, player.position.z);
    }
}
