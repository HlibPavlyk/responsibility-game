using UnityEngine;
using VContainer;
using ResponsibilityGame.Core.Interfaces;

namespace ResponsibilityGame.GameSystems.Dialogue
{
    public class DialogueTriggerDI : MonoBehaviour
    {
        [Header("Dialogue Settings")]
        [SerializeField] private TextAsset inkJSON;
        [SerializeField] private bool triggerOnEnter = true;
        [SerializeField] private bool canTriggerMultipleTimes = false;
        
        // DI залежності замість прямих посилань
        [Inject] private IDialogueManager dialogueManager;
        [Inject] private IInputManager inputManager;
        [Inject] private GameState gameState;
        
        private bool hasTriggered = false;
        private bool playerInRange = false;

        private void Update()
        {
            // Перевіряємо input для взаємодії
            if (playerInRange && !triggerOnEnter && inputManager.GetInteractPressed())
            {
                TriggerDialogue();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                
                if (triggerOnEnter)
                {
                    TriggerDialogue();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
            }
        }

        private void TriggerDialogue()
        {
            // Перевіряємо, чи можемо запустити діалог
            if (hasTriggered && !canTriggerMultipleTimes)
                return;

            if (gameState.isDialoguePlaying)
                return;

            if (inkJSON == null)
            {
                Debug.LogError("InkJSON is not assigned to DialogueTrigger!");
                return;
            }

            // Запускаємо діалог через DI залежність
            dialogueManager.EnterDialogueMode(inkJSON);
            hasTriggered = true;
            
            Debug.Log($"Dialogue triggered: {inkJSON.name}");
        }

        // Метод для ручного запуску діалогу
        public void StartDialogue()
        {
            TriggerDialogue();
        }

        // Метод для скидання тригера
        public void ResetTrigger()
        {
            hasTriggered = false;
        }
    }
}
