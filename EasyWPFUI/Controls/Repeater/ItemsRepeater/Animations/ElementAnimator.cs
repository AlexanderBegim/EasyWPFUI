// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EasyWPFUI.Controls
{
    public interface IElementAnimatorOverrides
    {
        bool HasShowAnimationCore(UIElement element, AnimationContext context);
        bool HasHideAnimationCore(UIElement element, AnimationContext context);
        bool HasBoundsChangeAnimationCore(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds);
        void StartShowAnimation(UIElement element, AnimationContext context);
        void StartHideAnimation(UIElement element, AnimationContext context);
        void StartBoundsChangeAnimation(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds);
    }

    // Given some elements and their animation context, ElementAnimator
    // animates them (show, hide and bounds change) and ensures the timing
    // is correct (hide -> bounds change -> show).
    // It's possible to customize the animations by inheriting from ElementAnimator
    // and overriding virtual/abstract members.
    public class ElementAnimator
    {
        public event EventHandler<UIElement> ShowAnimationCompleted;
        public event EventHandler<UIElement> HideAnimationCompleted;
        public event EventHandler<UIElement> BoundsChangeAnimationCompleted;

        public ElementAnimator()
        {
            m_animatingElements = new List<ElementInfo>();
        }

        #region IElementAnimator

        public void OnElementShown(UIElement element, AnimationContext context)
        {
            if (HasShowAnimation(element, context))
            {
                HasShowAnimationsPending = true;
                SharedContext |= context;
                QueueElementForAnimation(new ElementInfo(element, AnimationTrigger.Show, context));
            }
        }

        public void OnElementHidden(UIElement element, AnimationContext context)
        {
            if (HasHideAnimation(element, context))
            {
                HasHideAnimationsPending = true;
                SharedContext |= context;
                QueueElementForAnimation(new ElementInfo(element, AnimationTrigger.Hide, context));
            }
        }

        public void OnElementBoundsChanged(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds)
        {
            if (HasBoundsChangeAnimation(element, context, oldBounds, newBounds))
            {
                HasBoundsChangeAnimationsPending = true;
                SharedContext |= context;
                QueueElementForAnimation(new ElementInfo(element, AnimationTrigger.BoundsChange, context, oldBounds, newBounds));
            }
        }

        public bool HasShowAnimation(UIElement element, AnimationContext context)
        {
            return HasShowAnimationCore(element, context);
        }

        public bool HasHideAnimation(UIElement element, AnimationContext context)
        {
            return HasHideAnimationCore(element, context);
        }

        public bool HasBoundsChangeAnimation(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds)
        {
            return HasBoundsChangeAnimationCore(element, context, oldBounds, newBounds);
        }

        #endregion


        /* */

        #region IElementAnimatorOverrides

        protected virtual bool HasShowAnimationCore(UIElement element, AnimationContext context)
        {
            throw new NotImplementedException();
        }

        protected virtual bool HasHideAnimationCore(UIElement element, AnimationContext context)
        {
            throw new NotImplementedException();
        }

        protected virtual bool HasBoundsChangeAnimationCore(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds)
        {
            throw new NotImplementedException();
        }

        protected virtual void StartShowAnimation(UIElement element, AnimationContext context)
        {
            throw new NotImplementedException();
        }

        protected virtual void StartHideAnimation(UIElement element, AnimationContext context)
        {
            throw new NotImplementedException();
        }

        protected virtual void StartBoundsChangeAnimation(UIElement element, AnimationContext context, Rect oldBounds, Rect newBounds)
        {
            throw new NotImplementedException();
        }

        #endregion


        /* */

        #region IElementAnimatorProtected

        protected bool HasShowAnimationsPending{ get; private set; }

        protected bool HasHideAnimationsPending { get; private set; }

        protected bool HasBoundsChangeAnimationsPending { get; private set; }

        protected AnimationContext SharedContext { get; private set; }

        protected void OnShowAnimationCompleted(UIElement element)
        {
            ShowAnimationCompleted?.Invoke(this, element);
        }

        protected void OnHideAnimationCompleted(UIElement element)
        {
            HideAnimationCompleted?.Invoke(this, element);
        }

        protected void OnBoundsChangeAnimationCompleted(UIElement element)
        {
            BoundsChangeAnimationCompleted?.Invoke(this, element);
        }

        #endregion


        /* */


        private void QueueElementForAnimation(ElementInfo elementInfo)
        {
            m_animatingElements.Add(elementInfo);

            if (m_animatingElements.Count == 1)
            {
                CompositionTarget.Rendering += OnRendering;
            }
        }

        private void OnRendering(object sender, EventArgs args)
        {
            CompositionTarget.Rendering -= OnRendering;

            try
            {
                foreach(ElementInfo elementInfo in m_animatingElements)
                {
                    switch(elementInfo.Trigger())
                    {
                        case AnimationTrigger.Show:
                            {
                                // Call into the derivied class's StartShowAnimation override
                                StartShowAnimation(elementInfo.Element(), elementInfo.Context());
                                break;
                            }
                        case AnimationTrigger.Hide:
                            {
                                // Call into the derivied class's StartHideAnimation override
                                StartHideAnimation(elementInfo.Element(), elementInfo.Context());
                                break;
                            }
                        case AnimationTrigger.BoundsChange:
                            {
                                // Call into the derivied class's StartBoundsChangeAnimation override
                                StartBoundsChangeAnimation(elementInfo.Element(), elementInfo.Context(), elementInfo.OldBounds(), elementInfo.NewBounds());
                                break;
                            }
                    }
                }
            }
            finally
            {
                ResetState();
            }
        }

        private void ResetState()
        {
            m_animatingElements.Clear();
            HasShowAnimationsPending = HasHideAnimationsPending = HasBoundsChangeAnimationsPending = false;
            SharedContext = AnimationContext.None;
        }

        private enum AnimationTrigger
        {
            Show,
            Hide,
            BoundsChange
        }

        private struct ElementInfo
        {
            public ElementInfo(UIElement element, AnimationTrigger trigger, AnimationContext context)
            {
                m_element = new WeakReference<UIElement>(element);
                m_trigger = trigger;
                m_context = context;
                m_oldBounds = new Rect();
                m_newBounds = new Rect();
            }

            public ElementInfo(UIElement element, AnimationTrigger trigger, AnimationContext context, Rect oldBounds, Rect newBounds)
            {
                m_element = new WeakReference<UIElement>(element);
                m_trigger = trigger;
                m_context = context;
                m_oldBounds = oldBounds;
                m_newBounds = newBounds;
            }

            public UIElement Element()
            {
                if (m_element.TryGetTarget(out UIElement target))
                {
                    return target;
                }

                return null;
            }

            public AnimationTrigger Trigger()
            {
                return m_trigger;
            }

            public AnimationContext Context()
            {
                return m_context;
            }

            public Rect OldBounds()
            {
                return m_oldBounds;
            }
            
            public Rect NewBounds()
            {
                return m_newBounds;
            }


            private WeakReference<UIElement> m_element;
            private AnimationTrigger m_trigger;
            private AnimationContext m_context;
            // Valid for Trigger == BoundsChange
            private Rect m_oldBounds;
            private Rect m_newBounds;
        }


        private List<ElementInfo> m_animatingElements;
    }
}
