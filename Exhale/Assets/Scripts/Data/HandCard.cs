namespace Exhale.Scripts.Data
{
    public class HandCard
    {
        private static int _nextId;

        public HexPieceTemplate Template { get; }
        public int InstanceId { get; } = _nextId++;

        public HandCard(HexPieceTemplate template)
        {
            Template = template;
        }
    }
}
