using Core.Abstractions;
using UnityEngine.InputSystem;

namespace Systems.Input
{
    public class InputManager : IInputManager
    {
        private bool _interactPressed;
        private bool _submitPressed;
        private bool _pausePressed;

        public void InteractButtonPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _interactPressed = true;
            }
            else if (context.canceled)
            {
                _interactPressed = false;
            }
        }

        public void SubmitButtonPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _submitPressed = true;
            }
            else if (context.canceled)
            {
                _submitPressed = false;
            }
        }

        public void PauseButtonPressed(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _pausePressed = true;
            }
            else if (context.canceled)
            {
                _pausePressed = false;
            }
        }

        public bool GetInteractPressed()
        {
            var result = _interactPressed;
            _interactPressed = false;
            return result;
        }

        public bool GetSubmitPressed()
        {
            var result = _submitPressed;
            _submitPressed = false;
            return result;
        }

        public bool GetPausePressed()
        {
            var result = _pausePressed;
            _pausePressed = false;
            return result;
        }
    }
}
