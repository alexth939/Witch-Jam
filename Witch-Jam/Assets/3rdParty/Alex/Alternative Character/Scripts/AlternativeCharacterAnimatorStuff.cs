using System;
using UnityEngine;
using NaughtyAttributes;

namespace Runtime
{
    [Serializable]
    internal sealed class AlternativeCharacterAnimatorStuff
    {
        [SerializeField] private Animator _animator;
        [SerializeField, AnimatorParam(nameof(_animator))] private string _runBlendParamName;
        [SerializeField, AnimatorParam(nameof(_animator))] private string _plantBombParamName;
        [SerializeField, AnimatorParam(nameof(_animator))] private string _meleeAtackParamName;

        public Animator Animator => _animator;

        public string RunBlendParameter => _runBlendParamName;

        public string PlantBombParameter => _plantBombParamName;

        public string MeleeAttackParameter => _meleeAtackParamName;
    }
}
