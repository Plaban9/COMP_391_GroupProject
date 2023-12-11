using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class Teleport : MonoBehaviour
{
    [SerializeField]
    private Transform _teleportTo;

    [SerializeField]
    private AudioClip _teleportSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_teleportTo != null)
            {
                Controller.GetPlayer().OnTeleport(_teleportTo.position);
            }

            if (_teleportSound != null)
            {
                AudioSource.PlayClipAtPoint(_teleportSound, other.transform.position, 1f);
            }
        }
    }
}
