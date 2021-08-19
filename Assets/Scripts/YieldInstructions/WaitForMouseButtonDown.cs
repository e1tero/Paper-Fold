using UnityEngine;

namespace Assets.Scripts.YieldInstructions
{
    public class WaitForMouseButtonDown : CustomYieldInstruction
    {
        private bool _isClicked;
        private int _indexOfMouseButton;

        public WaitForMouseButtonDown(int indexOfMouseButton)
        {
            _indexOfMouseButton = indexOfMouseButton;
        }
        
        public override bool keepWaiting => !Input.GetMouseButtonDown(_indexOfMouseButton);
    }
}