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
using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
    /// <summary>
    /// It is not expected that typical users of the SlateWithManipulators prefab
    /// should need to either interact with or understand this script.
    ///
    /// This script contains the logic controlling the border affordance for the slate,
    /// as well as prefab state logic. This affordance's behavior is highly specialized,
    /// and so too is the logic implemented here. Specifically, this logic deals with the
    /// shading of the "rail" affordance, the placement, animation, and shading of the
    /// "capsule" affordances, and the logic associated with prefab statefulness (i.e.,
    /// disabling the slate interactions and affordances when something else on the prefab
    /// is selected).
    /// </summary>
    public class PanelWithManipulatorsBorderAffordanceController : MonoBehaviour
    {
        /// <summary>
        /// Calculates the projected position of a world space point onto the edge of a rounded box. Note that this function
        /// is designed specifically for the slate border visual use case and includes a built in "dead zone" which rejects
        /// projections from within the rounded box's defining rectangle.
        /// </summary>
        /// <param name="worldSpacePoint">The world space point to be projected</param>
        /// <param name="targetTransform">The transform defining the space into which the projection should occur</param>
        /// <param name="boneTransform">Transform of the "bone" of the rounded box, which lies at the corner of the defining rectangle</param>
        /// <param name="arcRadius">The radius of the arcs which form the corners of the rounded box</param>
        /// <returns>If the world space point's planar projection lies within the defining rectangle, returns null. Otherwise, returns the projected position in world space.</returns>
        private static Vector3? _projectToRoundedBoxEdge(Vector3 worldSpacePoint, Transform targetTransform, Transform boneTransform, float arcRadius)
        {
            // Project the world space point onto the XY plane in the local space of the target transform.
            var localPoint = targetTransform.InverseTransformPointUnscaled(worldSpacePoint);
            localPoint.z = 0f;

            // Project the bone position to the target transform local space; this is the corner of the
            // defining rectangle for the rounded box. (A rounded box can be defined as the set of all
            // points which are exactly a given distance from, but not contained by, a rectangle.)
            var cornerPoint = targetTransform.InverseTransformPointUnscaled(boneTransform.position);

            float x = Mathf.Sign(localPoint.x) * Mathf.Min(Mathf.Abs(cornerPoint.x), Mathf.Abs(localPoint.x));
            float y = Mathf.Sign(localPoint.y) * Mathf.Min(Mathf.Abs(cornerPoint.y), Mathf.Abs(localPoint.y));
            bool fullWidth = Mathf.Abs(Mathf.Abs(x) - Mathf.Abs(cornerPoint.x)) < Mathf.Epsilon;
            bool fullHeight = Mathf.Abs(Mathf.Abs(y) - Mathf.Abs(cornerPoint.y)) < Mathf.Epsilon;
            if (fullWidth || fullHeight)
            {
                var pt = new Vector3(x, y, 0f);
                var dir = (localPoint - pt).normalized;
                return targetTransform.TransformPointUnscaled(pt + dir * arcRadius);
            }

            return null;
        }

        [SerializeField, Tooltip("The grab interactable for the slate itself (as opposed to the surrounding affordances)")]
        private GrabInteractable _grabInteractable;

        [SerializeField, Tooltip("The hand grab interactable for the slate itself (as opposed to the surrounding affordances)")]
        private HandGrabInteractable _handGrabInteractable;

        [SerializeField, Tooltip("The state signaler for the SlateWithManipulators prefab")]
        private PanelWithManipulatorsStateSignaler _stateSignaler;

        [SerializeField, Tooltip("The grabbable associated with the slate itself (i.e., the grabbable with One- and TwoGrabFreeTransformers")]
        private Grabbable _grabbale;

        [SerializeField, Tooltip("The transform of one of the bones of the rail affordance (used in calculating capsule placement)")]
        private Transform _boneTransform;

        [SerializeField, Tooltip("The radius of the arcs at the corners of the rail affordance (used in calculating capsule placement)")]
        private float _cornerArcRadius;

        [SerializeField, Tooltip("The animator controlling the overall opacity of the rail affordance " +
            "(note that this is independent of the localized opacities associated with the capsule affordances)")]
        private Animator _railOpacityAnimator;

        [Serializable]
        class Affordance
        {
            [SerializeField, Tooltip("The parent transform of the geometry (i.e., visuals) which should be moved to place the capsule affordance")]
            private Transform _geometry;

            [SerializeField, Tooltip("Then transform controlled by an animation whose X axis magnitude will be used to control the affordance's opacity")]
            private Transform _opacityTransform;

            [SerializeField, Tooltip("The animators (canonically geometry and opacity) whose 'state' variables should be controlled by this affordance")]
            private Animator[] _animators;

            private int _animationState = 0;
            public int AnimationState
            {
                get
                {
                    return _animationState;
                }
                set
                {
                    if (value == _animationState)
                    {
                        return;
                    }

                    _animationState = value;

                    foreach (var animator in _animators)
                    {
                        animator.SetInteger("state", _animationState);
                    }
                }
            }

            private Vector3 _lastKnownPositionParentSpace;
            public Vector3 LastKnownPositionParentSpace
            {
                get
                {
                    return _lastKnownPositionParentSpace;
                }
                set
                {
                    _lastKnownPositionParentSpace = value;
                }
            }

            public Transform Geometry
            {
                get
                {
                    return _geometry;
                }
            }

            public float Opacity
            {
                get
                {
                    return Mathf.Abs(_opacityTransform.localPosition.x);
                }
            }
        }

        [SerializeField, Tooltip("The capsule affordances")]
        private Affordance[] _affordances;

        [SerializeField, Tooltip("The renderer controlling shading for the rail affordance")]
        private SkinnedMeshRenderer _railRenderer;

        private Vector4[] _fadePoints;

        private void Start()
        {
            _fadePoints = new Vector4[2] { Vector4.zero, Vector4.zero };

            _railOpacityAnimator.SetInteger("state", 1);
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

        private void Update()
        {
            // Affordances are correlated with interaction positions in decreasing order of animation state: selections first,
            // then hovers, then any remaining points are transitioned to idle. To simplify this logic and minimize the degree
            // to which affordances need to change states, _affordances is sorted by current animation state as a preliminary
            // step.
            Array.Sort(_affordances, (Affordance a, Affordance b) => { return b.AnimationState - a.AnimationState; });

            int affordanceIdx = 0;

            // First, we assign each selecting point an affordance to represent it.
            for (int pointIdx = 0; affordanceIdx < _affordances.Length && pointIdx < _grabbale.SelectingPointsCount; ++pointIdx)
            {
                var pt = _projectToRoundedBoxEdge(_grabbale.SelectingPoints[pointIdx].position, _grabbale.transform, _boneTransform, _cornerArcRadius);
                if (pt.HasValue)
                {
                    _affordances[affordanceIdx].Geometry.position = pt.Value;
                    _affordances[affordanceIdx].AnimationState = 3;
                    _affordances[affordanceIdx].LastKnownPositionParentSpace = transform.parent.InverseTransformPoint(pt.Value);
                    ++affordanceIdx;
                }
            }

            // Second, we assign each hovering point an affordance to represent it.
            for (int pointIdx = 0; affordanceIdx < _affordances.Length && pointIdx < _grabbale.PointsCount; ++pointIdx)
            {
                // Grabbable does not contractually guarantee the ordering of either Points or SelectingPoints, and
                // since SelectingPoints also reappear in Points, we have to manually check each point against
                // SelectingPoints to determine whether it is a selecting point, in which case it has already been
                // assigned an affordance and should be skipped.
                bool shouldSkip = false;
                foreach (var selectingPoint in _grabbale.SelectingPoints)
                {
                    if (Vector3.Distance(selectingPoint.position, _grabbale.Points[pointIdx].position) < 0.001f)
                    {
                        shouldSkip = true;
                        break;
                    }
                }
                if (shouldSkip)
                {
                    continue;
                }

                var pt = _projectToRoundedBoxEdge(_grabbale.Points[pointIdx].position, _grabbale.transform, _boneTransform, _cornerArcRadius);
                if (pt.HasValue)
                {
                    _affordances[affordanceIdx].Geometry.position = pt.Value;
                    _affordances[affordanceIdx].AnimationState = 2;
                    _affordances[affordanceIdx].LastKnownPositionParentSpace = transform.parent.InverseTransformPoint(pt.Value);
                    ++affordanceIdx;
                }
            }

            // Finally, any remaining affordances which are not assigned to represent an interaction point should
            // disabled. However, because disabling is visually controlled by an animation, this can take time, so
            // even these affordances must be positioned, using their LastKnownPositionParentSpace in place of an
            // interaction point.
            for (; affordanceIdx < _affordances.Length; ++affordanceIdx)
            {
                _affordances[affordanceIdx].AnimationState = 0;
                var pt = _projectToRoundedBoxEdge(transform.parent.TransformPoint(_affordances[affordanceIdx].LastKnownPositionParentSpace), _grabbale.transform, _boneTransform, _cornerArcRadius);
                if (pt.HasValue)
                {
                    _affordances[affordanceIdx].Geometry.position = pt.Value;
                }
            }

            // Calculate the "fade points" for the rail material based on the positions of the affordances. By
            // design, the rail is suppposed to fade to transparent near each affordance at a rate and range
            // dictated by the opacity of the affordance; this is controlled by the W axis of the fade point,
            // set in the following code by a bespoke bit of procedural logic.
            const float hydrationSpeed = 2f;
            for (int idx = 0; idx < _affordances.Length; ++idx)
            {
                _fadePoints[idx].x = _affordances[idx].Geometry.position.x;
                _fadePoints[idx].y = _affordances[idx].Geometry.position.y;
                _fadePoints[idx].z = _affordances[idx].Geometry.position.z;
                _fadePoints[idx].w = Mathf.Min(1f, hydrationSpeed * _affordances[idx].Opacity);
            }
            _railRenderer.material.SetVectorArray("_WorldSpaceFadePoints", _fadePoints);
        }

        private void HandleInteractableStateChanged(InteractableStateChangeArgs args)
        {
            // Specialized logic to deal with fading the rail only on the second selection. Second selections
            // will not result in state changed events, so detecting second selections requires registering for
            // an alternative signal.
            if (args.NewState == InteractableState.Select)
            {
                _stateSignaler.CurrentState = PanelWithManipulatorsStateSignaler.State.Selected;

                _grabInteractable.WhenSelectingInteractorAdded.Action += HandleMultipleSelection;
                _grabInteractable.WhenSelectingInteractorRemoved.Action += HandleMultipleSelection;
                _handGrabInteractable.WhenSelectingInteractorAdded.Action += HandleMultipleSelection;
                _handGrabInteractable.WhenSelectingInteractorRemoved.Action += HandleMultipleSelection;
            }
            else if (args.PreviousState == InteractableState.Select)
            {
                _stateSignaler.CurrentState = PanelWithManipulatorsStateSignaler.State.Default;

                _grabInteractable.WhenSelectingInteractorAdded.Action -= HandleMultipleSelection;
                _grabInteractable.WhenSelectingInteractorRemoved.Action -= HandleMultipleSelection;
                _handGrabInteractable.WhenSelectingInteractorAdded.Action -= HandleMultipleSelection;
                _handGrabInteractable.WhenSelectingInteractorRemoved.Action -= HandleMultipleSelection;
            }

            int grabAnimatorState = _grabInteractable.State == InteractableState.Disabled ? 0 : 1;
            int handGrabAnimatorState = _handGrabInteractable.State == InteractableState.Disabled ? 0 : 1;
            _railOpacityAnimator.SetInteger("state", Mathf.Max(grabAnimatorState, handGrabAnimatorState));
        }

        private void HandleMultipleSelection(object obj)
        {
            int selectingInteractors = Mathf.Max(_grabInteractable.SelectingInteractors.Count, _handGrabInteractable.SelectingInteractors.Count);
            if (selectingInteractors > 1)
            {
                _railOpacityAnimator.SetInteger("state", 3);
            }
            else if (selectingInteractors == 1)
            {
                _railOpacityAnimator.SetInteger("state", 1);
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
