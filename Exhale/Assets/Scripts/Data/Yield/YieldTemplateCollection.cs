using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    [CreateAssetMenu(menuName = "Exhale/Data/YieldTemplateCollection", fileName = "YieldTemplateCollection", order = 0)]
    public class YieldTemplateCollection : ScriptableObjectCollection<YieldTemplate>
    {
    }
}
