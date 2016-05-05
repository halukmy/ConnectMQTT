using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using SaveTheDate.Droid.Helpers;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;

namespace SaveTheDate.Droid.Helpers
{
    public static partial class AndroidUtils
    {
        public static Activity Context { get; set; }
        public static View SnackbarView { get; set; }
    }
}

namespace ConnectMQTT
{
    [Activity(Label = "ConnectMQTT", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        View layout;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = "Hello from Appcompat Toolbar";

            layout = FindViewById(Resource.Id.sample_main_layout);

            var toolbarBottom = FindViewById<Toolbar>(Resource.Id.toolbar_bottom);

            toolbarBottom.Title = "Photo Editing";
            //toolbarBottom.InflateMenu(Resource.Menu.photo_edit);
            toolbarBottom.MenuItemClick += (sender, e) =>
            {
                Toast.MakeText(this, "Bottom toolbar pressed: " + e.Item.TitleFormatted, ToastLength.Short).Show();
            };

            FindViewById<ImageView>(Resource.Id.image).Click += (sender, e) =>
            {
                //StartActivity(typeof(DetailActivity));
            };



            MqttClient client = new MqttClient("crossgate.dynu.com");
            byte code = client.Connect(Guid.NewGuid().ToString());

            client.ProtocolVersion = MqttProtocolVersion.Version_3_1;

            client.MqttMsgPublished += client_MqttMsgPublished;

            ushort msgId = client.Publish("System/test", // topic
                                          Encoding.UTF8.GetBytes("MyMessageBody"), // message body
                                          MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                                          true); // retained


        ushort msgIdX    = client.Subscribe(new string[] { "System/test", "/topic_2" },
          new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                  MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;


        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Snackbar.Make(layout, "hey", Snackbar.LengthLong)
                                   .SetAction("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic, (view) => { /*Undo message sending here.*/ })
                                   .Show();  // Don’t forget to show!         
        }

            void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            Snackbar.Make(layout, e.MessageId, Snackbar.LengthLong)
                        .SetAction(e.MessageId, (view) => { /*Undo message sending here.*/ })
                        .Show();  // Don’t forget to show!   
        }

        private void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Snackbar.Make(layout, "Message sent", Snackbar.LengthLong)
            .SetAction("Undo", (view) => { /*Undo message sending here.*/ })
            .Show(); // Don’t forget to show!
        }

        /// <Docs>The options menu in which you place your items.</Docs>
        /// <returns>To be added.</returns>
        /// <summary>
        /// This is the menu for the Toolbar/Action Bar to use
        /// </summary>
        /// <param name="menu">Menu.</param>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.home, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();

 
            Snackbar.Make(layout, "Message sent", Snackbar.LengthLong)
  .SetAction("Undo", (view) => { /*Undo message sending here.*/ })
  .Show(); // Don’t forget to show!

            return base.OnOptionsItemSelected(item);
        }
    }
}