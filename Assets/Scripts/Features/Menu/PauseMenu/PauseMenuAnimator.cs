using System.Collections;
using UnityEngine;

namespace Features.Menu.PauseMenu
{
    public class PauseMenuAnimator : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeInDuration = 0.2f;
        [SerializeField] private float fadeOutDuration = 0.15f;
        
        [Header("Scale Settings")]
        [SerializeField] private bool useScaleAnimation = true;
        [SerializeField] private RectTransform panelToScale;
        [SerializeField] private float scaleInDuration = 0.2f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private Coroutine _currentAnimation;

        private void Awake()
        {
            // Auto-setup if not assigned
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
            if (panelToScale == null)
                panelToScale = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            PlayShowAnimation();
        }

        private void PlayShowAnimation()
        {
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);
            
            _currentAnimation = StartCoroutine(ShowAnimation());
        }

        private void PlayHideAnimation(System.Action onComplete = null)
        {
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);
            
            _currentAnimation = StartCoroutine(HideAnimation(onComplete));
        }

        private IEnumerator ShowAnimation()
        {
            // Set initial state
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
            
            if (useScaleAnimation && panelToScale)
            {
                panelToScale.localScale = Vector3.zero;
            }
            
            var elapsed = 0f;
            var duration = Mathf.Max(fadeInDuration, scaleInDuration);
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                //float progress = elapsed / duration;
                
                // Fade in
                canvasGroup.alpha = elapsed < fadeInDuration ? Mathf.Lerp(0f, 1f, elapsed / fadeInDuration) : 1f;
                
                // Scale in
                if (useScaleAnimation && panelToScale && elapsed < scaleInDuration)
                {
                    var scaleProgress = elapsed / scaleInDuration;
                    var curveValue = scaleCurve.Evaluate(scaleProgress);
                    panelToScale.localScale = Vector3.one * curveValue;
                }
                else if (panelToScale)
                {
                    panelToScale.localScale = Vector3.one;
                }
                
                yield return null;
            }
            
            // Ensure final state
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            
            if (panelToScale)
            {
                panelToScale.localScale = Vector3.one;
            }
        }

        private IEnumerator HideAnimation(System.Action onComplete)
        {
            canvasGroup.interactable = false;
            
            var elapsed = 0f;
            var duration = Mathf.Max(fadeOutDuration, scaleInDuration * 0.5f);
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                // Fade out
                canvasGroup.alpha = elapsed < fadeOutDuration ? Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration) : 0f;
                
                // Scale out (optional, faster)
                if (useScaleAnimation && panelToScale)
                {
                    var scaleProgress = elapsed / (scaleInDuration * 0.5f);
                    panelToScale.localScale = Vector3.one * (1f - scaleProgress);
                }
                
                yield return null;
            }
            
            // Ensure final state
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            
            onComplete?.Invoke();
        }

        // Public methods for manual control
        public void Show()
        {
            gameObject.SetActive(true);
            PlayShowAnimation();
        }

        public void Hide()
        {
            PlayHideAnimation(() => gameObject.SetActive(false));
        }
    }
}
