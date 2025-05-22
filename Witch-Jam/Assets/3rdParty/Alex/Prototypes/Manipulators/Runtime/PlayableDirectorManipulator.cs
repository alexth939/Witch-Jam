using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Prototypes.Manipulators
{
#if ENABLE_TIMELINE
    public class PlayableDirectorManipulator : MonoBehaviour
    {
        [Header("Timeline Settings")]
        public PlayableDirector _playableDirector;

        public bool _shouldPlayOnAwake = true;

        [Header("Events")]
        public UnityEvent _timelineEnded;

        // You can also manually play or stop the timeline from other scripts
        public void PlayTimeline()
        {
            if(_playableDirector != null)
            {
                _playableDirector.Play();
            }
        }

        public void StopTimeline()
        {
            if(_playableDirector != null)
            {
                _playableDirector.Stop();
            }
        }

        private void Awake()
        {
            // Play the timeline immediately if playOnAwake is true
            if(_shouldPlayOnAwake && _playableDirector != null)
            {
                _playableDirector.Play();
            }
        }

        private void OnDisable()
        {
            // Unregister the event when the object is disabled
            if(_playableDirector != null)
            {
                _playableDirector.stopped -= OnTimelineEnded;
            }
        }

        private void OnEnable()
        {
            // Register to listen for the timeline's end event
            if(_playableDirector != null)
            {
                _playableDirector.stopped += OnTimelineEnded;
            }
        }

        // Callback for when the timeline ends
        private void OnTimelineEnded(PlayableDirector director)
        {
            _timelineEnded.Invoke();  // Invoke the callback
        }
    }
#endif
}
