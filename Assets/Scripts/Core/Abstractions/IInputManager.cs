using UnityEngine.InputSystem;

namespace Core.Abstractions
{
    public interface IInputManager
    {
        void InteractButtonPressed(InputAction.CallbackContext context);
        void SubmitButtonPressed(InputAction.CallbackContext context);
        void PauseButtonPressed(InputAction.CallbackContext context);
        bool GetInteractPressed();
        bool GetSubmitPressed();
        bool GetPausePressed();
    }
}
