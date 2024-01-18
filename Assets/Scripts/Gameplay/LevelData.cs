using System;
using UnityEngine;

namespace Gameplay
{
    [Serializable]
    public struct LevelLineData
    {
        public LineElementType[] data;
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "LevelData", menuName = "Create level data")]
    public class LevelData : ScriptableObject
    {
        public LevelLineData[] map;
    }
}