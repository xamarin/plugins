using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace ReactiveXSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView _tv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            _tv = FindViewById<TextView>(Resource.Id.app_textview);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_reactivex)
            {
                var r = new ReactiveXSampleLibrary.TestClass();
                r.TestReactiveX(_tv);

                return true;
            }
            else if (id == Resource.Id.action_rxkotlin)
            {
                var r = new ReactiveXSampleLibrary.TestClass();
                r.TestRxKotlin(_tv);

                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
