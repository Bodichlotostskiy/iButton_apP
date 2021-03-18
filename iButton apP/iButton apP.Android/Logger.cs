using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iButton_apP.Droid
{
    class Logger
    {
		public  const int MODE_LOGCAT = 0;
		public  const int MODE_TOAST = 1;
		private int mMode = 0;
		private Context mContext;

		public Logger(Context context)
		{
			mContext = context;
		}

		public void setMode(int mode)
		{
			mMode = mode;
		}

		public void log(String tag, String msg)
		{
			//switch (mMode)
			//{
			//	case MODE_LOGCAT:
			//		Log.Debug(tag, msg);
			//		break;
			//	case MODE_TOAST:
			//		Toast.MakeText(mContext, msg, Toast.).show();
			//		break;
			//}
		}
	}
}