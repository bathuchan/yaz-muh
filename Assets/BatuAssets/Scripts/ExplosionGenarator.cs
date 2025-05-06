using System.Collections;
using UnityEngine;

public class ExplosionGenarator : MonoBehaviour
{
    public static void CreateExplosion(Vector3 spawnPosition, float expRadius, float expForce)
    {
        GameObject explosionObject = new GameObject("ExplosionEffect");
        ExplosionGenarator explosion = explosionObject.AddComponent<ExplosionGenarator>();
        explosion.StartCoroutine(explosion.Explode(explosionObject.transform.position, expRadius, expForce));
    }

    public IEnumerator Explode(Vector3 spawnPosition, float expRadius, float expForce)
    {
        Collider[] affectedPlayers = Physics.OverlapSphere(spawnPosition, expRadius);

        foreach (var hit in affectedPlayers)
        {
            PlayerState player = hit.GetComponentInParent<PlayerState>();
            if (player != null)
            {
                Vector3 direction = (player.transform.position - spawnPosition).normalized;
                float distance = Vector3.Distance(player.transform.position, spawnPosition);
                float forceMagnitude = Mathf.Lerp(expForce, 0, distance / expRadius);
                player.ApplyForce(direction * forceMagnitude);
            }

            yield return null;
        }

        // Clean up
        Destroy(gameObject);
    }
}
