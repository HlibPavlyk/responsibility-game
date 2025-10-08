using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Menu
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
        
        [Header("Background Blur (Optional)")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.7f);
        
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
            
            if (backgroundImage == null)
                backgroundImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            PlayShowAnimation();
        }

        public void PlayShowAnimation()
        {
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);
            
            _currentAnimation = StartCoroutine(ShowAnimation());
        }

        public void PlayHideAnimation(System.Action onComplete = null)
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
            
            if (useScaleAnimation && panelToScale != null)
            {
                panelToScale.localScale = Vector3.zero;
            }
            
            float elapsed = 0f;
            float duration = Mathf.Max(fadeInDuration, scaleInDuration);
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / duration;
                
                // Fade in
                if (elapsed < fadeInDuration)
                {
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                }
                else
                {
                    canvasGroup.alpha = 1f;
                }
                
                // Scale in
                if (useScaleAnimation && panelToScale != null && elapsed < scaleInDuration)
                {
                    float scaleProgress = elapsed / scaleInDuration;
                    float curveValue = scaleCurve.Evaluate(scaleProgress);
                    panelToScale.localScale = Vector3.one * curveValue;
                }
                else if (panelToScale != null)
                {
                    panelToScale.localScale = Vector3.one;
                }
                
                yield return null;
            }
            
            // Ensure final state
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            
            if (panelToScale != null)
            {
                panelToScale.localScale = Vector3.one;
            }
        }

        private IEnumerator HideAnimation(System.Action onComplete)
        {
            canvasGroup.interactable = false;
            
            float elapsed = 0f;
            float duration = Mathf.Max(fadeOutDuration, scaleInDuration * 0.5f);
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / duration;
                
                // Fade out
                if (elapsed < fadeOutDuration)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                }
                else
                {
                    canvasGroup.alpha = 0f;
                }
                
                // Scale out (optional, faster)
                if (useScaleAnimation && panelToScale != null)
                {
                    float scaleProgress = elapsed / (scaleInDuration * 0.5f);
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

        public void HideImmediate()
        {
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);
            
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
    }
}
