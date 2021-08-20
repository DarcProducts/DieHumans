using System.Collections;
using UnityEngine;
using TMPro;

public abstract class Drop : MonoBehaviour
{
    public ObjectPooler infoLettersPool;
    public string pickupText;
    public Material material;
    public ModInitializer pickupMods;
    public FXInitializer pickupFX;
    
    public abstract void Activate();

    public abstract void SetData();

    public IEnumerator DisplayPickupText(float duration)
    {
        GameObject pGO = infoLettersPool.GetObject();
        if (pGO != null)
        {
            TMP_Text pT = pGO.GetComponent<TMP_Text>();
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
