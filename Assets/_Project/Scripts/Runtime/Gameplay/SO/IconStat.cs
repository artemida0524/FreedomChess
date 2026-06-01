using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "New Icon Stat", menuName = "Stats/Icon Stat")]
public class IconStat : ScriptableObject
{
    public int iconId;
    public Sprite icon;
}
