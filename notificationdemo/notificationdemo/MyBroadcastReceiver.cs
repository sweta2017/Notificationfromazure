using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Util;
using Gcm.Client;
using WindowsAzure.Messaging;
using System.Diagnostics;


[assembly: Permission(Name = "notificationsHub.Android.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "notificationsHub.Android.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
namespace notificationdemo
{
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class MyBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        public static string[] SENDER_IDS = new string[] { Constants.SenderID };

        public const string TAG = "MyBroadcastReceiver-GCM";
    }
    [Service]
    public class GcmService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }
        static NotificationHub Hub { get; set; }
        public GcmService() : base("notificationdemo-3fa42")
        {
            Log.Info(MyBroadcastReceiver.TAG, "GcmService() constructor");
        }
        protected override void OnError(Context context, string errorId)
        {
            //Manage errors  
        }
        public static void Register(Context Context)
        {
           
            GcmClient.Register(Context, MyBroadcastReceiver.SENDER_IDS);
        }
        public static void Initialize(Context context)
        {
            Hub = new NotificationHub(Constants.NotificationHubPath, Constants.ConnectionString, context);
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            if (Hub != null)
                Hub.Unregister();

            Log.Verbose(MyBroadcastReceiver.TAG, "GCM Unregistered: " + registrationId);

            createNotification("GcmService Unregistered...", "The device has been unregistered, Tap to View!");
        }


        protected override void OnMessage(Context context, Intent intent)
        {
            Log.Info(MyBroadcastReceiver.TAG, "GCM Message Received!");

            var msg = new StringBuilder();

            if (intent != null && intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                    msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
            }

            string messageText = intent.Extras.GetString("msg");
            if (!string.IsNullOrEmpty(messageText))
            {
                createNotification("New hub message!", messageText);
                return;
            }

            createNotification("Unknown message details", msg.ToString());
        }

        protected override void OnRegistered(Context context, string registrationId)
        {
            Log.Verbose(MyBroadcastReceiver.TAG, "GCM Registered: " + registrationId);
            RegistrationID = registrationId;

            if (Hub != null)
                Hub.Register(registrationId, "TEST");
        }

        void createNotification(string title, string desc)
        {
            Android.Support.V4.App.NotificationCompat.Builder builder = new Android.Support.V4.App.NotificationCompat.Builder(this);
            var uiIntent = new Intent(this, typeof(MainActivity));
            var notification1 = builder.SetContentIntent(PendingIntent.GetActivity(this, 0, uiIntent, 0)) .SetSmallIcon(Resource.Drawable.Icon).SetTicker(desc).SetWhen(DateTime.Now.Millisecond).SetAutoCancel(true).SetContentTitle(title).SetContentText(desc).Build();
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
          
           // var notification = new Notification(Resource.Drawable.Icon, title);
          //  notification.Flags = NotificationFlags.AutoCancel;
          //  notification.SetLatestEventInfo(this, title, desc, PendingIntent.GetActivity(this, 0, uiIntent, 0));

            notificationManager.Notify(1, notification1);
        }

        
    }
}