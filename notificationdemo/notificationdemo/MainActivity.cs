using Android.App;
using Android.Widget;
using Android.OS;
using Gcm.Client;

namespace notificationdemo
{
    [Activity(Label = "notificationdemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
              SetContentView (Resource.Layout.Main);
              GcmClient.CheckDevice(this);
              GcmClient.CheckManifest(this);
              GcmClient.Register(this, "notificationdemo-3fa42");

             GcmService.Initialize(this);
             GcmService.Register(this);

        }
       
    }
}

