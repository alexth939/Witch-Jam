using UnityEngine;

namespace Prototypes.Interactables.Dynamics
{
    public class TinyAnimator : MonoBehaviour
    {
        [SerializeField] private AnimationClip _clip;

        [SerializeField]
        private WrapMode _wrapMode = WrapMode.Once;

        [SerializeField]
        private bool _playOnStart = true;

        private float _time;
        private bool _playing;
        private GameObject _dummyTarget;

        //maybe later: make it internal + assembly info: friendly asm
        public static string ClipFieldName => nameof(_clip);

        private void Start()
        {
            if(_clip == null)
            {
                Debug.LogWarning("No AnimationClip assigned to TinyAnimator.");
                return;
            }

            _clip.wrapMode = _wrapMode;

            // Needed for Evaluate()
            _dummyTarget = new GameObject("TinyAnimatorDummy");
            _dummyTarget.hideFlags = HideFlags.HideAndDontSave;

            if(_playOnStart)
            {
                Play();
            }
        }

        private void Update()
        {
            if(!_playing || _clip == null)
                return;

            _time += Time.deltaTime;

            if(_time > _clip.length)
            {
                switch(_wrapMode)
                {
                    case WrapMode.Once:
                        _playing = false;
                        return;

                    case WrapMode.Loop:
                        _time %= _clip.length;
                        break;

                    case WrapMode.PingPong:
                        // Optional: Add PingPong support if needed.
                        break;
                }
            }

            _clip.SampleAnimation(gameObject, _time);
        }

        public void Play()
        {
            _time = 0;
            _playing = true;
        }

        public void Stop()
        {
            _playing = false;
        }

        private void OnDestroy()
        {
            if(_dummyTarget != null)
            {
                DestroyImmediate(_dummyTarget);
            }
        }
    }
}

//using UnityEngine;

//namespace Prototypes.Interactables.Dynamics
//{
//    public enum TinyAnimatorPlayMode
//    {
//        Once,
//        Loop,
//        PingPong
//    }

//    [DisallowMultipleComponent]
//    public class TinyAnimator : MonoBehaviour
//    {
//        [SerializeField]
//        private AnimationClip _clip;

//        [SerializeField]
//        private TinyAnimatorPlayMode _playMode = TinyAnimatorPlayMode.Once;

//        [SerializeField]
//        private bool _playOnStart = true;

//        [SerializeField]
//        private string _clipNameOverride = "";

//        private Animation _animation;

//        private void Start()
//        {
//            if(_clip == null)
//                return;

//            _animation = gameObject.GetComponent<Animation>();
//            if(_animation == null)
//                _animation = gameObject.AddComponent<Animation>();

//            string clipName = string.IsNullOrEmpty(_clipNameOverride) ? _clip.name : _clipNameOverride;
//            //_clip.legacy = true;
//            _animation.AddClip(_clip, clipName);
//            _animation.clip = _clip;
//            _animation.wrapMode = ConvertToWrapMode(_playMode);

//            if(_playOnStart)
//                _animation.Play(clipName);
//        }

//        public void Play()
//        {
//            if(_animation != null && _clip != null)
//            {
//                string clipName = string.IsNullOrEmpty(_clipNameOverride) ? _clip.name : _clipNameOverride;
//                _animation.Play(clipName);
//            }
//        }

//        private static WrapMode ConvertToWrapMode(TinyAnimatorPlayMode playMode)
//        {
//            switch(playMode)
//            {
//                case TinyAnimatorPlayMode.Loop:
//                    return WrapMode.Loop;
//                case TinyAnimatorPlayMode.PingPong:
//                    return WrapMode.PingPong;
//                case TinyAnimatorPlayMode.Once:
//                default:
//                    return WrapMode.Once;
//            }
//        }
//    }
//}
