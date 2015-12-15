using System;
using System.Linq;

using UIKit;

namespace ios
{
    public partial class MainViewController : UIViewController
    {
        public MainViewController() : base()
        {
            Initialize();
        }

        void Initialize()
        {
            AddChildViewController(new DrawerViewController {
                Content = new UIViewController().apply(it => { it.View.BackgroundColor = UIColor.White; }),
                LeftDrawer = new NavigationViewController().apply(it => { it.View.BackgroundColor = UIColor.Orange; }),
                RightDrawer = new UIViewController().apply(it => { it.View.BackgroundColor = UIColor.Red; })
            }.apply(it => View.AddSubview(it.View)));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
    }
}


