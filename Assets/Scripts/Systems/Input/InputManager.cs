using Core.Abstractions;
using UnityEngine.InputSystem;

namespace ResponsibilityGame.GameSystems.Input
{
    public class InputManager : IInputManager
    {
        private bool interactPressed;
        private bool submitPressed;

        public void InteractButtonPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                interactPressed = true;
            }
            else if (context.canceled)
            {
                interactPressed = false;
            }
        }

        public void SubmitButtonPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                submitPressed = true;
            }
            else if (context.canceled)
            {
                submitPressed = false;
            }
        }

        public bool GetInteractPressed()
        {
            bool result = interactPressed;
            interactPressed = false;
            return result;
        }

        public bool GetSubmitPressed()
        {
            bool result = submitPressed;
            submitPressed = false;
            return result;
        }
    }
}
