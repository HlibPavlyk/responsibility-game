using UnityEngine.InputSystem;

namespace Core.Abstractions
{
    public interface IInputManager
    {
        void InteractButtonPressed(InputAction.CallbackContext context);
        void SubmitButtonPressed(InputAction.CallbackContext context);
        bool GetInteractPressed();
        bool GetSubmitPressed();
    }
}
