using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public enum LineElementType
    {
        None,
        Lamp,
        Wifi,
        Energy,
        Line,
        Corner,
        ThirdWay,
        Block
    }
    
    public class LevelGenerationControl : MonoBehaviour
    {
        public Transform powerLineElementsContainer;
        
        [SerializeField] private LevelData activeLevelData;
        [SerializeField] private GameObject energy;
        [SerializeField] private GameObject wifi;
        [SerializeField] private GameObject lamp;
        [SerializeField] private GameObject corner;
        [SerializeField] private GameObject line;
        [SerializeField] private GameObject thirdWay;
        [SerializeField] private TMP_Text log;

        public List<List<Vector2>> WifiPositions { get; } = new();

        public bool isGenerateLvl { get; private set; }

        public static LevelGenerationControl Instance;
        public static readonly List<Cell> Cells = new();
        public static readonly System.Random SystemRandom = new System.Random();

        private void Awake()
        {
            Instance = this;
        }

        public void DeleteAllPowerLineElements()
        {
            while (LevelControl.Instance.PowerLineElements.Count > 0)
            {
                var element = LevelControl.Instance.PowerLineElements[0];
                LevelControl.Instance.PowerLineElements.Remove(element);
                Destroy(element.gameObject);
            }
        }

        public static void UpdateRandomState()
        {
            Random.InitState(SystemRandom.Next());
        }

        private void CreateLvl()
        {
            var wifiElements = new List<WifiElement>();
            var vector = -Vector3.one * (activeLevelData.map.Length / 2);
            vector.z = 0;
            
            for (int i = 0; i < activeLevelData.map.Length; i++)
            {
                for (int j = 0; j < activeLevelData.map[0].data.Length; j++)
                {
                    GameObject createElement;
                    switch (activeLevelData.map[i].data[j])
                    {
                        case LineElementType.Line:
                            createElement = line;
                            break;
                        case LineElementType.Corner:
                            createElement = corner;
                            break;
                        case LineElementType.ThirdWay:
                            createElement = thirdWay;
                            break;
                        case LineElementType.Energy:
                            createElement = energy;
                            break;
                        case LineElementType.Wifi:
                            createElement = wifi;
                            break;
                        case LineElementType.Lamp:
                            createElement = lamp;
                            break;
                        default:
                            continue;
                    }
                    
                    var newElement = Instantiate(createElement, powerLineElementsContainer).GetComponent<PowerLineElement>();
                    newElement.Position = new Vector2(j, i);
                    newElement.transform.localPosition = new Vector3(j, i, 0) + vector;
                    if (activeLevelData.map[i].data[j] == LineElementType.Wifi)
                    {
                        wifiElements.Add((WifiElement)newElement);
                    }
                    LevelControl.Instance.PowerLineElements.Add(newElement);
                }
            }
            powerLineElementsContainer.localScale = Vector3.one * (5f / activeLevelData.map.Length);

            if(wifiElements.Count == 0 || wifiElements.Count % 2 != 0) return;
            if (wifiElements.Count == 4)
            {
                var firstWifi = wifiElements.Where(element => element.Position == WifiPositions[0][0]).ToArray()[0];
                wifiElements.Remove(firstWifi);
                var secondWifi = wifiElements.Where(element => element.Position == WifiPositions[1][0]).ToArray()[0];
                wifiElements.Remove(secondWifi);
                firstWifi.InitTwoWifi(secondWifi);
            }
            wifiElements[0].InitTwoWifi(wifiElements[1]);
        }

        private void GenerateLvlField(int x, int y)
        {
            activeLevelData.map = new LevelLineData[y];
            for (int i = 0; i < y; i++)
            {
                activeLevelData.map[i] = new LevelLineData();
                activeLevelData.map[i].data = new LineElementType[x];
                for (int j = 0; j < x; j++)
                {
                    activeLevelData.map[i].data[j] = LineElementType.None;
                }
            }
        }

        public void GenerateLvl(Vector2Int mapSize)
        {
            StartCoroutine(GenerateLvlCoroutine(mapSize));
        }

        private IEnumerator GenerateLvlCoroutine(Vector2Int mapSize)
        {
            isGenerateLvl = true;
            WifiPositions.Clear();
            DeleteAllPowerLineElements();
            GenerateLvlField(mapSize.x, mapSize.y);
            var lineCount = Random.Range(1, 4);
            for (int i = 0; i < lineCount; i++)
            {
                if (i == 0) 
                    AddCellOnRandomPosition(LineElementType.Energy, lineCount != 1, i);
                else if (i + 1 == lineCount) 
                    AddCellOnRandomPosition(LineElementType.Wifi, false, i);
                else
                    AddCellOnRandomPosition(LineElementType.Wifi, true, i);
                UpdateRandomState();
            }
            var generationPartCount = 20;
            while (Cells.Count > 0 && generationPartCount > 0)
            {
                generationPartCount--;
                var cells = Cells.ToArray();
                foreach (var cell in cells)
                {
                    cell.GenerateCell(); // При вызове этой функции через некоторое время выдает ошибку, генерация вроде сначала идет но всегда выдает здесь ошибку через некоторое время
                    cell.DeleteCellIfEnd();
                }
            }
            CreateLvl();
            SetElementsRandomRotation();
			yield return null;
            LevelControl.Instance.UpdatePowerLineState();
            HatchControl.Instance.GenerateNewHatches();
            
            isGenerateLvl = false;
        }

        public void SetElementsRandomRotation()
        {
            foreach (var powerLineElement in LevelControl.Instance.PowerLineElements)
            {
                var randomRotateCount = Random.Range(0, 3);
                for (int i = 0; i <= randomRotateCount; i++)
                {
                    powerLineElement.RotateWithoutSetRotateAndAnimation();
                }
                powerLineElement.SetActiveRotate();
            }
        }

        private void AddCellOnRandomPosition(LineElementType firstElementType, bool needGenerateWifi = false, int wifiIndex = 0)
        {
            while (true)
            {
                var pos = new Vector2(Random.Range(0, activeLevelData.map[0].data.Length), Random.Range(0, activeLevelData.map.Length));
                if (CheckMapElement(pos, LineElementType.None))
                {
                    List<Direction> voidDirection = new List<Direction>();

                    if (CheckVoidCell(pos, Direction.Bottom))
                    {
                        voidDirection.Add(Direction.Bottom);
                    }

                    if (CheckVoidCell(pos, Direction.Left))
                    {
                        voidDirection.Add(Direction.Left);
                    }

                    if (CheckVoidCell(pos, Direction.Right))
                    {
                        voidDirection.Add(Direction.Right);
                    }

                    if (CheckVoidCell(pos, Direction.Top))
                    {
                        voidDirection.Add(Direction.Top);
                    }

                    if (voidDirection.Count == 0)
                    {
                        continue;
                    }

                    SetMapElement(pos, firstElementType);
                    WifiPositions.Add(new List<Vector2>());
                    if (firstElementType == LineElementType.Wifi)
                    {
                        WifiPositions[wifiIndex].Add(pos);
                    }

                    var nextPositionDirection = voidDirection.Count == 1
                        ? voidDirection[0]
                        : voidDirection[Random.Range(0, voidDirection.Count)];
                    var nextPosition = PowerLineElement.GetVectorByDirection(nextPositionDirection) + pos;
                    SetMapElement(nextPosition, LineElementType.Block);
                    Cells.Add(new Cell(nextPosition, PowerLineElement.GetOppositeDirection(nextPositionDirection), wifiIndex, needGenerateWifi));
                }
                else
                {
                    continue;
                }

                break;
            }
        }

        public static void SetMapElement(Vector2 pos, LineElementType elementType)
        {
            Instance.activeLevelData.map[(int)pos.y].data[(int)pos.x] = elementType;
        }
        
        public static bool CheckMapElement(Vector2 pos, LineElementType elementType)
        {
            return Instance.activeLevelData.map[(int)pos.y].data[(int)pos.x] == elementType;
        }

        public static bool CheckVoidCell(Vector2 pos, Direction direction)
        {
            var checkPosition = PowerLineElement.GetVectorByDirection(direction) + pos;
			if((int)checkPosition.y < 0 || (int)checkPosition.y >= Instance.activeLevelData.map.Length ||
               (int)checkPosition.x < 0 || (int)checkPosition.x >= Instance.activeLevelData.map[(int)pos.y].data.Length) return false;
            return CheckMapElement(checkPosition, LineElementType.None);
        }
    }

    public class Cell
    {
        private bool _isEnd;
        private readonly int _generationCellIndex;
        private readonly bool _needGenerateWifi;
        private Vector2 _startPosition;
        private Direction _prevPositionDirection;

        public Cell(Vector2 startPosition, Direction prevPositionDirection, int generationCellIndex = 0, bool needGenerateWifi = false)
        {
            _startPosition = startPosition;
            _prevPositionDirection = prevPositionDirection;
            _generationCellIndex = generationCellIndex;
            _needGenerateWifi = needGenerateWifi;
        }

        public void DeleteCellIfEnd()
        {
            if(_isEnd) LevelGenerationControl.Cells.Remove(this);
        }
        
        public void GenerateCell()
        {
            List<Direction> voidDirection = new List<Direction>();

            if (LevelGenerationControl.CheckVoidCell(_startPosition, Direction.Bottom))
            {
                voidDirection.Add(Direction.Bottom);
            }

            if (LevelGenerationControl.CheckVoidCell(_startPosition, Direction.Left))
            {
                voidDirection.Add(Direction.Left);
            }

            if (LevelGenerationControl.CheckVoidCell(_startPosition, Direction.Right))
            {
                voidDirection.Add(Direction.Right);
            }

            if (LevelGenerationControl.CheckVoidCell(_startPosition, Direction.Top))
            {
                voidDirection.Add(Direction.Top);
            }

            LineElementType nextElementType;
            if (voidDirection.Count > 0)
            {
                var elements = new List<LineElementType> { };
                if (voidDirection.Contains(PowerLineElement.GetOppositeDirection(_prevPositionDirection)))
                {
                    elements.Add(LineElementType.Line);
                }

                if (voidDirection.Contains(PowerLineElement.GetDirectionAfterRotate90(_prevPositionDirection))
                    || voidDirection.Contains(PowerLineElement.GetDirectionAfterRotate90(
                        PowerLineElement.GetOppositeDirection(_prevPositionDirection))))
                {
                    elements.Add(LineElementType.Corner);
                }

                if (voidDirection.Count >= 2)
                {
                    elements.Add(LineElementType.ThirdWay);
                    elements.Add(LineElementType.ThirdWay);
                }

                nextElementType = elements.Count == 1 ? elements[0] : elements[Random.Range(0, elements.Count)];
            }
            else
            {
                nextElementType = LineElementType.Lamp;
            }

            Direction vector;
            LevelGenerationControl.SetMapElement(_startPosition, nextElementType);
            switch (nextElementType)
            {
                case LineElementType.Lamp:
                    if (_needGenerateWifi)
                    {
                        LevelGenerationControl.Instance.WifiPositions[_generationCellIndex].Add(_startPosition);
                        LevelGenerationControl.SetMapElement(_startPosition, LineElementType.Wifi);
                    }
                    _isEnd = true;
                    return;
                case LineElementType.Line:
                    vector = PowerLineElement.GetOppositeDirection(_prevPositionDirection);
                    _startPosition = PowerLineElement.GetVectorByDirection(vector) + _startPosition;
                    LevelGenerationControl.SetMapElement(_startPosition, LineElementType.Block);
                    return;
                case LineElementType.Corner:
                    vector = PowerLineElement.GetDirectionAfterRotate90(_prevPositionDirection);
                    if (!voidDirection.Contains(vector)) 
                    {
                        vector = PowerLineElement.GetDirectionAfterRotate90(
                            PowerLineElement.GetOppositeDirection(_prevPositionDirection));
                    }
                    _prevPositionDirection = PowerLineElement.GetOppositeDirection(vector);
                    _startPosition = PowerLineElement.GetVectorByDirection(vector) + _startPosition;
                    LevelGenerationControl.SetMapElement(_startPosition, LineElementType.Block);
                    return;
                case LineElementType.ThirdWay:
                    var nextDirections = new Direction[2];
                    var randomNumber = Random.Range(0, voidDirection.Count);
                    nextDirections[0] = voidDirection[randomNumber];
                    voidDirection.RemoveAt(randomNumber);
                    nextDirections[1] = voidDirection.Count == 1
                        ? voidDirection[0]
                        : voidDirection[Random.Range(0, voidDirection.Count)];
                    
                    var secondCellPosition = PowerLineElement.GetVectorByDirection(nextDirections[1]) 
                                             + _startPosition;
                    LevelGenerationControl.Cells.Add(new Cell(secondCellPosition, 
                        PowerLineElement.GetOppositeDirection(nextDirections[1])));
                    
                    _prevPositionDirection = PowerLineElement.GetOppositeDirection(nextDirections[0]);
                    _startPosition = PowerLineElement.GetVectorByDirection(nextDirections[0]) + _startPosition;
                    
                    LevelGenerationControl.SetMapElement(_startPosition, LineElementType.Block);
                    LevelGenerationControl.SetMapElement(secondCellPosition, LineElementType.Block);
                    return;
                default:
                    return;
            }
        }
    }
}