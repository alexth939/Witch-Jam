using System.Collections.Generic;
using UnityEngine;

namespace PinBoard.Editor
{
    [CreateAssetMenu(fileName = "PinBoardPage", menuName = "PinBoard/Page")]
    public class PinBoardPage : ScriptableObject
    {
        [field: SerializeField] internal List<PinReference> ProjectPins { get; set; } = new();
        [field: SerializeField] internal List<PinReference> ScenePins = new();
    }
}