using Android.App;
using Android.Widget;
using Android.OS;
using Com.Lilarcor.Cheeseknife;
using Android.Views;
using System;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;

namespace android
{
    [Activity(Label = "WIIMP", MainLauncher = true, Icon = "@mipmap/icon", Theme="@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        [InjectView(Resource.Id.main_toolbar)]
        Toolbar toolbar;

        [InjectView(Resource.Id.main_fab)]
        FloatingActionButton floatingButton;

        [InjectView(Resource.Id.main_drawer)]
        DrawerLayout drawer;

        ActionBarDrawerToggle toggle;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main);
            Cheeseknife.Inject(this);
            Initialize();
        }

        private void Initialize() {
            SetSupportActionBar(toolbar);
            floatingButton.Click += (sender, e) => drawer.OpenDrawer((int)GravityFlags.End);
            toggle = new ActionBarDrawerToggle(this, drawer, toolbar,
                Resource.String.app_name, Resource.String.app_name);
            drawer.SetDrawerListener(toggle);
            toggle.SyncState();
        }

        public override void OnBackPressed() {
            if (drawer.IsDrawerOpen((int)GravityFlags.Start))
                drawer.CloseDrawer((int)GravityFlags.Start);
            else
                base.OnBackPressed();
        }
    }
}
