using System.Collections;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroyOverTime(4f));
    }
    IEnumerator DestroyOverTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

}
