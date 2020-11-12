using UnityEngine;
using TMPro;

public class CreditsLoader : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creditsText;
    [SerializeField] TextAsset credits;

    private void Start() 
    {
        creditsText.SetText(credits.text);
    }

}
