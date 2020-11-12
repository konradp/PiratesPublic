using UnityEngine;

public class funny : MonoBehaviour
{
    Light light;
    void Start()
    {
        light = GetComponent<Light>();
    }
    private void Update()
    {
        light.color = Random.ColorHSV(0.5f, 0.7f, 0.5f, 0.7f);
    }

}
