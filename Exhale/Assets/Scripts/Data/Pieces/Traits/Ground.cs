using System;

namespace Exhale.Scripts.Data
{
    [Serializable]
    public class Ground : PieceTrait
    {
        public override bool ValidateConfig()
        {
            return true;
        }
    }
}