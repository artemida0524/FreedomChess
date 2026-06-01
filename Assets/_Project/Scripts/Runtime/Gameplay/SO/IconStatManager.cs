using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IconStatManager : MonoBehaviour
{
    public List<IconStat> icons;

    private Dictionary<int, IconStat> _iconStatsById;

    public void Init()
    {
        _iconStatsById = icons.ToDictionary(item => item.iconId, item => item);
    }

    public IconStat GetIconStatById(int id)
    {
        if (_iconStatsById.TryGetValue(id, out var stat))
        {
            return stat;
        }
        else
        {
            Debug.LogWarning($"IconStat with id {id} not found.");
            return null;
        }
    }

}
