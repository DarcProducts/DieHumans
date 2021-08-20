using UnityEngine;
[CreateAssetMenu(menuName = "Mods/New Mod Initializer")]
public class ModInitializer : ScriptableObject { public Mod[] mods = new Mod[0]; public void ActivateMods() { for (int i = 0; i < mods.Length; i++) if (mods[i] != null) mods[i].ChangeValue(); } }
