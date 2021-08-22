using System.Collections;
using UnityEngine;
using TMPro;
[CreateAssetMenu(menuName = "Extras/New Drop")]
public class Drop : ScriptableObject
{
    public string pickupText;
    public Material material;
    public ModInitializer pickupMods;
    public FXInitializer pickupFX;

    public IEnumerator DisplayPickupText(float duration, GameObject GO)
    {
        if (GO != null)
        {
            TMP_Text pT = GO.GetComponent<TMP_Text>();
            if (pT != null)
            {
                pT.text = pickupText;
                pT.gameObject.SetActive(true);
                yield return new WaitForSeconds(duration);
                pT.gameObject.SetActive(false);
            }
        }
    }
}
