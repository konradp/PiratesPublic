using System.Collections;
using UnityEngine;

public class BonusAreaChecker : MonoBehaviour
{
    [SerializeField] GameObject bonusCanvas;
    bool alreadyChecked;
    private void Start()
    {
        alreadyChecked = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player0") && !alreadyChecked)
        {
            alreadyChecked = true;
            StartCoroutine(ShowBonusCanvas(5f));
           
        }
    }
    IEnumerator ShowBonusCanvas(float _time)
    {
        bonusCanvas.SetActive(true);
        yield return new WaitForSeconds(_time);
        bonusCanvas.SetActive(false);
    }

}
