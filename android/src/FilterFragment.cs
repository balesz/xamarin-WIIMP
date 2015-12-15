using System;
using Android.Support.V4.App;
using Android.Views;
using Android.OS;

namespace net.solutinno.wiimp
{
    public class FilterFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(android.Resource.Layout.filter, container, false);
        }
    }
}
