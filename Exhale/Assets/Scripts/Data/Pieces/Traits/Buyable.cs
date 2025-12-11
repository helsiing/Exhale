using System;
using System.Linq;
using Exhale.Collections;

namespace Exhale.Scripts.Data
{
	[Serializable]
	public sealed class Buyable : PieceTrait
	{
		public DictionaryWrapper<Yield, int> BasePrice;
		
		public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
		{
			return BasePrice.PairsInOrder.All(entry => entry.Key != null);
		}
	}
}
