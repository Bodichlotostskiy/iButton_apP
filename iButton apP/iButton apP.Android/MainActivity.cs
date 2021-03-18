using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views.InputMethods;
using Android.Hardware.Input;
using Android.Content;
using Android.Hardware.Usb;
using Java.Util.Logging;
using Android.Widget;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Android;

namespace iButton_apP.Droid
{
    [Activity(Label = "iButton_apP", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached })]
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        BroadcastReceiver mUsbDetachReceiver;
        BroadcastReceiver mUsbAttachReceiver;
        BroadcastReceiver mUsbReceiver;
        private TextView mInfo;
        private Logger mLogger;
        private Dictionary<UsbDevice, UsbDataBinder> mHashMap = new Dictionary<UsbDevice, UsbDataBinder>();
        private UsbManager mUsbManager;
        private PendingIntent mPermissionIntent;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            mUsbManager = (UsbManager)GetSystemService(Context.UsbService);

            mInfo = (TextView)FindViewById(Resource.Id.action_text);

           
            usbConnection();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            
        }

        private void usbConnection()
        {
            IntentFilter filter = new IntentFilter(UsbManager.ActionUsbDeviceAttached);
            RegisterReceiver(mUsbAttachReceiver, filter);
            filter = new IntentFilter(UsbManager.ActionUsbDeviceDetached);
            RegisterReceiver(mUsbDetachReceiver, filter);

            mPermissionIntent = PendingIntent.GetBroadcast(this, 0, new Intent(ACTION_USB_PERMISSION), 0);
            filter = new IntentFilter(ACTION_USB_PERMISSION);
            RegisterReceiver(mUsbReceiver, filter);

            showDevices();
        }

        protected void onDestroy()
        {
            base.OnDestroy();
            UnregisterReceiver(mUsbDetachReceiver);
            UnregisterReceiver(mUsbAttachReceiver);
            UnregisterReceiver(mUsbReceiver);
        }

        private void setBroadcastReceiversAttached(Context context, Intent intent)
        {
            mUsbAttachReceiver.OnReceive(context, intent);
            string action = intent.Action;
            if (UsbManager.ActionUsbDeviceDetached.Equals(action))
            {
                UsbDevice device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                if (device != null)
                {
                    UsbDataBinder binder = mHashMap[device];
                    if (binder != null)
                    {
                        binder.onDestroy();
                        mHashMap.Remove(device);
                    }
                }
            }
        }

        private void setBroadcastReceiversDetach(Context context, Intent intent)
        {
            mUsbDetachReceiver.OnReceive(context, intent);
            string action = intent.Action;
            if (UsbManager.ActionUsbDeviceAttached.Equals(action))
            {
                showDevices();
            }
        }
        private static  String ACTION_USB_PERMISSION = "com.companyname.ibutton_app.USB_PERMISSION";

        private void setBroadcastReceivers(Context context, Intent intent)
        {
            mUsbReceiver.OnReceive(context, intent);
            string action = intent.Action;
            if (ACTION_USB_PERMISSION.Equals(action))
            {
                lock (this)
                {
                    UsbDevice device = (UsbDevice)intent
                            .GetParcelableExtra(UsbManager.ExtraDevice);

                    if (intent.GetBooleanExtra(
                            UsbManager.ExtraPermissionGranted, false))
                    {
                        if (device != null)
                        {
                            // call method to set up device communication
                            UsbDataBinder binder = new UsbDataBinder(mUsbManager, device);
                            
                            var a = mHashMap[device];
                        }
                    }
                }
            }
        }
        private void showDevices()
        {
            Dictionary<String, UsbDevice> deviceList = (Dictionary<string, UsbDevice>)mUsbManager.DeviceList;
            IEnumerator<UsbDevice> deviceIterator = deviceList.Values.GetEnumerator();
            while (deviceIterator.MoveNext())
            {
                UsbDevice device = (UsbDevice)deviceIterator.MoveNext();
                mUsbManager.RequestPermission(device, mPermissionIntent);
                //your code
                mLogger.log("usb", "name: " + device.DeviceName + ", " +
                        "ID: " + device.DeviceId);
                mInfo.Append(device.DeviceName + "\n");
                mInfo.Append(device.DeviceId + "\n");
                mInfo.Append(device.DeviceProtocol + "\n");
                mInfo.Append(device.ProductId + "\n");
                mInfo.Append(device.VendorId + "\n");
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}