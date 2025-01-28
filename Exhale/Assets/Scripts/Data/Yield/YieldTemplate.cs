using BrunoMikoski.ScriptableObjectCollections;

namespace Exhale.Scripts.Gameplay
{
    public class YieldTemplate : ScriptableObjectCollectionItem
    {
        public int GetId()
        {
            return GUID.GetHashCode();
        }
    }
}