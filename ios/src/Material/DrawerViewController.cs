using System;
using System.Linq;

using UIKit;
using Foundation;
using CoreGraphics;

namespace ios
{
    public partial class DrawerViewController : UIViewController
    {
        #region Initialization

        UIView contentMask;

        public DrawerViewController() : base() {
            Initialize();
        }

        void Initialize() {
            View.AddSubview(new UIView(View.Frame).apply(it => {
                contentMask = it;
                it.BackgroundColor = UIColor.Black;
                it.Alpha = 0f;
                setupSubViews();
            }));
        }

        #endregion

        #region Public Properties (Content, LeftDrawer, RightDrawer)

        private UIViewController content;
        public UIViewController Content {
            get { return content; }
            set {
                if (content != null) {
                    content.View.RemoveFromSuperview();
                    content.RemoveFromParentViewController();
                    content.Dispose();
                }
                content = value;
                AddChildViewController(content);
                View.AddSubview(content.View);
                setupSubViews();
            }
        }

        private UIViewController leftDrawer;
        public UIViewController LeftDrawer {
            get { return leftDrawer; }
            set {
                if (leftDrawer != null) {
                    leftDrawer.View.RemoveFromSuperview();
                    leftDrawer.RemoveFromParentViewController();
                    leftDrawer.Dispose();
                }
                leftDrawer = value;
                AddChildViewController(leftDrawer);
                View.AddSubview(leftDrawer.View);
                calcLeftDrawerFrame();
                setupSubViews();
            }
        }

        private UIViewController rightDrawer;
        public UIViewController RightDrawer {
            get { return rightDrawer; }
            set {
                if (rightDrawer != null) {
                    rightDrawer.View.RemoveFromSuperview();
                    rightDrawer.RemoveFromParentViewController();
                    rightDrawer.Dispose();
                }
                rightDrawer = value;
                AddChildViewController(rightDrawer);
                View.AddSubview(rightDrawer.View);
                calcRightDrawerFrame();
                setupSubViews();
            }
        }

        #endregion

        #region Public Methods

        public void OpenLeftDrawer() { openLeftDrawer(); }
        public void CloseLeftDrawer() { closeLeftDrawer(); }

        public void OpenRightDrawer() { openRightDrawer(); }
        public void CloseRightDrawer() { closeRightDrawer(); }

        #endregion

        #region Orientation Change Handling

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation) {
            base.DidRotate(fromInterfaceOrientation);
            calcLeftDrawerFrame();
            calcRightDrawerFrame();
            setupSubViews();
        }

        #endregion

        #region Private Properties

        UIView leftView {
            get { return leftDrawer == null ? null : leftDrawer.View; }
        }

        UIView rightView {
            get { return rightDrawer == null ? null : rightDrawer.View; }
        }

        CGRect leftFrame {
            get { return leftView == null ? CGRect.Empty : leftView.Frame; }
            set { if (leftView != null) leftView.Frame = value; }
        }

        CGRect rightFrame {
            get { return rightView == null ? CGRect.Empty : rightView.Frame; }
            set { if (rightView != null) rightView.Frame = value; }
        }

        bool isLeftVisible {
            get { return leftFrame == CGRect.Empty ? false : leftFrame.Left > -leftFrame.Width; }
        }

        bool isRightVisible {
            get { return rightFrame == CGRect.Empty ? false : rightFrame.Left < View.Frame.Width; }
        }

        nfloat DrawerWidth { get {
                var unit = Material.ActionBarHeight;
                var maxWidth = unit * 5f;
                var width = View.Bounds.Width - unit;
                return width > maxWidth ? maxWidth : width;
            }
        }

        #endregion

        #region Size and Z-order Calculations

        void calcLeftDrawerFrame() {
            if (leftDrawer == null)
                return;
            leftFrame = new CGRect(-DrawerWidth, 0f, DrawerWidth, View.Bounds.Height);
            leftView.SetNeedsDisplay();
        }

        void calcRightDrawerFrame() {
            if (rightDrawer == null)
                return;
            rightFrame = new CGRect(View.Bounds.Width, 0f, DrawerWidth, View.Bounds.Height);
            rightView.SetNeedsDisplay();
        }

        void setupSubViews() {
            if (content != null)
                View.BringSubviewToFront(content.View);
            if (contentMask != null)
                View.BringSubviewToFront(contentMask);
            if (leftDrawer != null)
                View.BringSubviewToFront(leftView);
            if (rightDrawer != null)
                View.BringSubviewToFront(rightView);
        }

        #endregion

        #region Pan, Open and Close Handling

        CGPoint lastPosition;

        bool isPanning;

        bool isPanningLeft;

        bool isPanningRight;

        public override void TouchesBegan(NSSet touches, UIEvent evt) {
            base.TouchesBegan(touches, evt);
            var location = evt.AllTouches.Cast<UITouch>().First().LocationInView(View);
            lastPosition = location;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt) {
            base.TouchesMoved(touches, evt);
            isPanning = true;
            var position = evt.AllTouches.Cast<UITouch>().First().LocationInView(View);
            if (isLeftVisible) {
                if (isPanningLeft)
                    panLeft(position);
                else if (rightFrame.IntersectsWith(getTouchWindow(position))) {
                    closeLeftDrawer();
                    panRight(position);
                }
                else
                    isPanningLeft = leftFrame.Contains(position);
            }
            else if (isRightVisible) {
                if (isPanningRight)
                    panRight(position);
                else if (leftFrame.IntersectsWith(getTouchWindow(position))) {
                    closeRightDrawer();
                    panLeft(position);
                }
                else
                    isPanningRight = rightFrame.Contains(position);
            }
            else {
                if (position.X - leftFrame.GetMaxX() < Material.ActionBarHeight / 2) {
                    panLeft(position);
                } else if (rightFrame.GetMinX() - position.X < Material.ActionBarHeight / 2) {
                    panRight(position);
                }
            }
            lastPosition = position;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt) {
            base.TouchesEnded(touches, evt);
            var position = evt.AllTouches.Cast<UITouch>().First().LocationInView(View);
            if (!isPanning) {
                if (isLeftVisible && !leftFrame.Contains(position))
                    closeLeftDrawer();
                if (isRightVisible && !rightFrame.Contains(position))
                    closeRightDrawer();
            }
            else if (isPanningLeft) {
                if (Math.Abs(leftFrame.Left) < leftFrame.Width / 2)
                    openLeftDrawer();
                else
                    closeLeftDrawer();
            }
            else if (isPanningRight) {
                if (View.Frame.Width - rightFrame.Left > rightFrame.Width / 2)
                    openRightDrawer();
                else
                    closeRightDrawer();
            }
            isPanning = isPanningLeft = isPanningRight = false;
            lastPosition = CGPoint.Empty;
        }

        CGRect getTouchWindow(CGPoint point) {
            var distance = Material.ActionBarHeight;
            return new CGRect(point.X - distance / 2, point.Y - distance / 2, distance, distance);
        }

        void panLeft(CGPoint position) {
            isPanningRight = false;
            isPanningLeft = true;
            var rect = leftFrame.offset(position.X - lastPosition.X, 0f);
            if (rect.Left > 0f)
                rect = new CGRect(new CGPoint(0f, 0f), rect.Size);
            leftFrame = rect;
            var fraction = (leftFrame.Left + leftFrame.Width) / leftFrame.Width;
            contentMask.Alpha = fraction * 0.75f;
        }

        void panRight(CGPoint position) {
            isPanningLeft = false;
            isPanningRight = true;
            var rect = rightFrame.offset(position.X - lastPosition.X, 0f);
            if (rect.Right <= rightView.Superview.Frame.Width)
                rect = new CGRect(new CGPoint(rightView.Superview.Frame.Width - rect.Width, 0f), rect.Size);
            rightFrame = rect;
            var fraction = (rightView.Superview.Frame.Width - rightFrame.Left) / rightFrame.Width;
            contentMask.Alpha = fraction * 0.75f;
        }

        void openLeftDrawer() {
            var view = leftDrawer.View;
            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(0.2);
            UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            view.Frame = new CGRect(new CGPoint(0f, 0f), view.Frame.Size);
            contentMask.Alpha = Material.DrawerMaskMaxAlpha;
            UIView.CommitAnimations();
        }

        void closeLeftDrawer() {
            var view = leftDrawer.View;
            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(0.2);
            UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            view.Frame = new CGRect(new CGPoint(-view.Frame.Width, 0f), view.Frame.Size);
            contentMask.Alpha = 0f;
            UIView.CommitAnimations();
        }

        void openRightDrawer() {
            var view = rightDrawer.View;
            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(0.2);
            UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            view.Frame = new CGRect(new CGPoint(view.Superview.Frame.Width - view.Frame.Width, 0f), view.Frame.Size);
            contentMask.Alpha = Material.DrawerMaskMaxAlpha;
            UIView.CommitAnimations();
        }

        void closeRightDrawer() {
            var view = rightDrawer.View;
            UIView.BeginAnimations(null);
            UIView.SetAnimationDuration(0.2);
            UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
            view.Frame = new CGRect(new CGPoint(view.Superview.Frame.Width, 0f), view.Frame.Size);
            contentMask.Alpha = 0f;
            UIView.CommitAnimations();
        }

        #endregion
    }

    public static class DrawerViewControllerExtensions {
        public static CGRect offset(this CGRect rect, nfloat x, nfloat y) {
            return new CGRect(new CGPoint(rect.X + x, rect.Y + y), rect.Size);
        }
    }
}
