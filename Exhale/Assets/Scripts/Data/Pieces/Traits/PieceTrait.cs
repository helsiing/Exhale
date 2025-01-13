using System;

namespace Exhale.Scripts.Data
{
    /// <summary>
    /// Base class that defines a trait ("characteristic") of a piece.
    /// </summary>
    [Serializable]
    public abstract class PieceTrait
    {
        public abstract bool ValidateConfig();
    }
}