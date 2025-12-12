using System;
using System.Linq;
using Exhale.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exhale.Scripts.Data
{
	[Serializable]
	public sealed class Buyable : PieceTrait
	{
		[Title("Buyable")]
		[SerializeReference]
		private DictionaryWrapper<Yield, int> basePrice;
		public DictionaryWrapper<Yield, int> BasePrice => basePrice;
		
		public override bool ValidateConfig(HexPieceTemplate pieceTemplate)
		{
			return basePrice.PairsInOrder.All(entry => entry.Key != null);
		}
	}
}
