/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
    /// <summary>
    /// It is not expected that typical users of the SlateWithManipulators prefab
    /// should need to either interact with or understand this script.
    ///
    /// This script contains the logic for connecting the state of an interactable
    /// manipulator to the affordance rendered to represent that manipulator. This
    /// script is specifically responsible for setting the "state" variable on the
    /// affordances animators, as well as responding to overall changes in the
    /// state of the prefab.
    /// </summary>
    public class ManipulatorAffordanceController : MonoBehaviour
    {
        [SerializeField, Tooltip("The grab interactable for the slate itself (as opposed to the surrounding affordances)")]
        private GrabInteractable _grabInteractable;

        [SerializeField, Tooltip("The hand grab interactable for the slate itself (as opposed to the surrounding affordances)")]
        private HandGrabInteractable _handGrabInteractable;

        [SerializeField, Tooltip("The state signaler for the SlateWithManipulators prefab")]
        private PanelWithManipulatorsStateSignaler _stateSignaler;

        [SerializeField, Tooltip("The animators (canonically geometry and opacity) whose 'state' variables should be controlled by this affordance")]
        private Animator[] _animators;

        private void Start()
        {
            foreach (var animator in _animators)
            {
                animator.SetInteger("state", 1);
            }

            _grabInteractable.WhenStateChanged += HandleInteractableStateChanged;
            _handGrabInteractable.WhenStateChanged += HandleInteractableStateChanged;

            _stateSignaler.WhenStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            _grabInteractable.WhenStateChanged -= HandleInteractableStateChanged;
            _handGrabInteractable.WhenStateChanged -= HandleInteractableStateChanged;

            _stateSignaler.WhenStateChanged -= HandleStateChanged;
        }

        private void HandleInteractableStateChanged(InteractableStateChangeArgs args)
        {
            if (args.NewState == InteractableState.Select)
            {
                _stateSignaler.CurrentState = PanelWithManipulatorsStateSignaler.State.Selected;
            }
            else if (args.PreviousState == InteractableState.Select)
            {
                _stateSignaler.CurrentState = PanelWithManipulatorsStateSignaler.State.Default;
            }

            int grabAnimatorState = 0;
            switch (_grabInteractable.State)
            {
                case InteractableState.Normal:
                    grabAnimatorState = 1;
                    break;
                case InteractableState.Hover:
                    grabAnimatorState = 2;
                    break;
                case InteractableState.Select:
                    grabAnimatorState = 3;
                    break;
            }

            int handGrabAnimatorState = 0;
            switch (_handGrabInteractable.State)
            {
                case InteractableState.Normal:
                    handGrabAnimatorState = 1;
                    break;
                case InteractableState.Hover:
                    handGrabAnimatorState = 2;
                    break;
                case InteractableState.Select:
                    handGrabAnimatorState = 3;
                    break;
            }

            foreach (var animator in _animators)
            {
                animator.SetInteger("state", Mathf.Max(grabAnimatorState, handGrabAnimatorState));
            }
        }

        private void HandleStateChanged(PanelWithManipulatorsStateSignaler.State state)
        {
            if (state != PanelWithManipulatorsStateSignaler.State.Default)
            {
                if (_grabInteractable.State != InteractableState.Select && _handGrabInteractable.State != InteractableState.Select)
                {
                    _grabInteractable.enabled = false;
                    _handGrabInteractable.enabled = false;
                }
            }
            else
            {
                _grabInteractable.enabled = true;
                _handGrabInteractable.enabled = true;
            }
        }
    }
}
