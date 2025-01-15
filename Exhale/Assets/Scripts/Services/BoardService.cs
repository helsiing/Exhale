using System;
using Exhale.Gameplay;
using Exhale.Scripts.External.ServiceLocators;
using UnityEngine;

namespace Exhale.Services
{
    public interface IBoardService : IService
    {
        public Action<IHexPiece> OnPiecePlaced { get; set; }
    }

    public class BoardService : IBoardService
    {
        public BoardService()
        {
            Debug.Log("Service created");    
        }
        
        public void Dispose()
        {
            // TODO release managed resources here
        }

        public Action<IHexPiece> OnPiecePlaced { get; set; }
    }
}