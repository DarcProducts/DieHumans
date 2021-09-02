using UnityEngine;

public static class Utilities
{
    public static bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    /// <param name="damage"> damage before rounded to int </param>
    public static void TryDamagingNearTargets(Vector3 pos, float rad, LayerMask hitLayer, float damage)
    {
        Collider[] closeObjects = Physics.OverlapSphere(pos, rad, hitLayer);
        for (int i = 0; i < closeObjects.Length; i++)
        {
            float eachDamage = Mathf.RoundToInt(damage / closeObjects.Length);
            IDamagable<float> d = closeObjects[i].gameObject.GetComponent<IDamagable<float>>();
            if (d != null)
                d.ApplyDamage(eachDamage);
        }
    }

    public static void TryDamagingTarget(GameObject target, LayerMask hitLayer, float damage)
    {
        if (IsInLayerMask(target, hitLayer))
        {
            IDamagable<float> t = target.GetComponent<IDamagable<float>>();
            if (t != null)
                t.ApplyDamage(Mathf.RoundToInt(damage));
        }
    }

    public static void PlayAtSourceWithVPRange(AudioSource source, AudioClip clip, float minVol = 0, float maxVol = 1, float minPitch = -3, float maxPitch = 3)
    {
        if (source != null && clip != null)
        {
            minVol = minVol < 0 ? 0 : minVol > 1 ? 1 : minVol;
            maxVol = maxVol < minVol ? minVol : maxVol > 1 ? 1 : maxVol;
            minPitch = minPitch < -3 ? -3 : minPitch > 3 ? 3 : minPitch;
            maxPitch = maxPitch < minPitch ? minPitch : maxPitch > 3 ? 3 : maxPitch;
            source.volume = Random.Range(minVol, maxVol);
            source.pitch = Random.Range(minPitch, maxPitch);
            source.PlayOneShot(clip);
        }
    }
}