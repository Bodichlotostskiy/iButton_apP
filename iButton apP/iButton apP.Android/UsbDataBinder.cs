using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iButton_apP.Droid
{
    public class UsbDataBinder
    {
		private byte[] bytes = new byte[1024];
		private static int TIMEOUT = 0;
		private bool forceClaim = true;
		private UsbDevice mDevice;
		private UsbManager mUsbManager;
		private UsbDeviceConnection mConnection;
		private UsbInterface mIntf;
		private UsbEndpoint mEndpoint;
		public UsbDataBinder(UsbManager manager, UsbDevice device)
		{
			// TODO Auto-generated constructor stub
			mUsbManager = manager;
			mDevice = device;
			mIntf = mDevice.GetInterface(0);
			mEndpoint = mIntf.GetEndpoint(0);
			mConnection = mUsbManager.OpenDevice(mDevice);
		}
		public void onDestroy()
		{
			if (mConnection != null)
			{
				mConnection.ReleaseInterface(mIntf);
				mConnection.Close();
			}
		}
	}
}

	
