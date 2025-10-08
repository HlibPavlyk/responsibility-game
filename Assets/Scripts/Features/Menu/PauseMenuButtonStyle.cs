using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.Menu
{
    [RequireComponent(typeof(Button))]
    public class PauseMenuButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [Header("Text Settings")]
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Color normalTextColor = new(0.2f, 0.2f, 0.2f);
        [SerializeField] private Color highlightedTextColor = new(0.3f, 0.8f, 1f); // Cyan

        [Header("Outline Settings")]
        [SerializeField] private bool useOutline = true;
        [SerializeField] private Color outlineColor = Color.black;
        [SerializeField] private float outlineWidth = 0.2f;

        [Header("Animation Settings")]
        [SerializeField] private bool useScaleAnimation = true;
        [SerializeField] private float normalScale = 1f;
        [SerializeField] private float highlightedScale = 1.1f;
        [SerializeField] private float animationSpeed = 10f;

        [Header("Sound (Optional)")]
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private AudioClip clickSound;

        private Button _button;
        private Image _backgroundImage;
        private Vector3 _targetScale;
        private bool _isHighlighted;
        private AudioSource _audioSource;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _backgroundImage = GetComponent<Image>();

            // Get or create AudioSource for sounds
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null && (hoverSound != null || clickSound != null))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
            }

            // Auto-find text if not assigned
            if (buttonText == null)
            {
                buttonText = GetComponentInChildren<TextMeshProUGUI>();
            }

            SetupTextStyle();
            _targetScale = Vector3.one * normalScale;
        }

        private void Start()
        {
            // Add click sound listener
            if (clickSound != null && _audioSource != null)
            {
                _button.onClick.AddListener(() => PlaySound(clickSound));
            }
        }

        private void SetupTextStyle()
        {
            if (buttonText == null) return;

            buttonText.color = normalTextColor;

            // Setup outline
            if (useOutline)
            {
                buttonText.outlineColor = outlineColor;
                buttonText.outlineWidth = outlineWidth;
            }

            // Pixel-perfect settings
            buttonText.enableAutoSizing = false;
            buttonText.alignment = TextAlignmentOptions.Center;
        }

        private void Update()
        {
            // Smooth scale animation
            if (useScaleAnimation)
            {
                transform.localScale = Vector3.Lerp(
                    transform.localScale,
                    _targetScale,
                    Time.unscaledDeltaTime * animationSpeed
                );
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            SetHighlighted(true);
            PlaySound(hoverSound);

            // Set selected in EventSystem
            EventSystem.current?.SetSelectedGameObject(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            SetHighlighted(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!_button.interactable) return;

            SetHighlighted(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!_button.interactable) return;

            SetHighlighted(false);
        }

        private void SetHighlighted(bool highlighted)
        {
            _isHighlighted = highlighted;

            if (buttonText != null)
            {
                buttonText.color = highlighted ? highlightedTextColor : normalTextColor;
            }

            if (useScaleAnimation)
            {
                _targetScale = Vector3.one * (highlighted ? highlightedScale : normalScale);
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }
    }
}
