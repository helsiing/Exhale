using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "Exhale/BoardConfig", order = 0)]
    public class BoardConfig : ScriptableObject
    {
        [SerializeField] private int width = 11;
        public int Width => width;
        
        [SerializeField] private int height = 11;
        public int Height => height;
        
        [SerializeField] private int numBuildings = 5;
        public int NumBuildings => numBuildings;
    }
}