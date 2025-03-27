using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpoldeSIm : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 10.0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Explode();
        }
    }
    private void Explode()
    {
        Collider[] affectedPlayers = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var hit in affectedPlayers)
        {
            PlayerState player = hit.GetComponentInParent<PlayerState>();
            if (player!=null)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                float distance = Vector3.Distance(player.transform.position, transform.position);
                float forceMagnitude = Mathf.Lerp(explosionForce, 0, distance / explosionRadius);
                player.ApplyForce(direction * forceMagnitude);
            }
        }
    }
}
