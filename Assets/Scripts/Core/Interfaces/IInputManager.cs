using UnityEngine.InputSystem;

namespace ResponsibilityGame.Core.Interfaces
{
    public interface IInputManager
    {
        void InteractButtonPressed(InputAction.CallbackContext context);
        void SubmitButtonPressed(InputAction.CallbackContext context);
        bool GetInteractPressed();
        bool GetSubmitPressed();
    }
}
