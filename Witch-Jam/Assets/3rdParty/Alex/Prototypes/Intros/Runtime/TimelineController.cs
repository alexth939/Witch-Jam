using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

namespace Prototypes.Intros
{
    public class TimelineController : MonoBehaviour
    {
        [Header("Timeline Settings")]
        public PlayableDirector playableDirector;
        public bool playOnAwake = true;

        [Header("Events")]
        public UnityEvent onTimelineEnded;

        private void Awake()
        {
            // Play the timeline immediately if playOnAwake is true
            if(playOnAwake && playableDirector != null)
            {
                playableDirector.Play();
            }
        }

        private void OnEnable()
        {
            // Register to listen for the timeline's end event
            if(playableDirector != null)
            {
                playableDirector.stopped += OnTimelineEnded;
            }
        }

        private void OnDisable()
        {
            // Unregister the event when the object is disabled
            if(playableDirector != null)
            {
                playableDirector.stopped -= OnTimelineEnded;
            }
        }

        // Callback for when the timeline ends
        private void OnTimelineEnded(PlayableDirector director)
        {
            onTimelineEnded.Invoke();  // Invoke the callback
        }

        // You can also manually play or stop the timeline from other scripts
        public void PlayTimeline()
        {
            if(playableDirector != null)
            {
                playableDirector.Play();
            }
        }

        public void StopTimeline()
        {
            if(playableDirector != null)
            {
                playableDirector.Stop();
            }
        }
    }
}
