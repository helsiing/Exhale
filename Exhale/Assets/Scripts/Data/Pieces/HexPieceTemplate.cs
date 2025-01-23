using System;
using BrunoMikoski.ScriptableObjectCollections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [Serializable] 
    public struct PieceTemplateData : IComponentData
    {
        public int PieceId;
    } 
    
    [Serializable] 
    public struct BoardPosition : IComponentData
    {
        public int2 PositionIndex;
    } 
    
    public class HexPieceTemplate : ScriptableObjectCollectionItem
    {
        [SerializeField] private GameObject piecePrefab;
        public GameObject PiecePrefab => piecePrefab;
        
        public int GetId()
        {
            return GUID.GetHashCode();
        }
        
    }
}