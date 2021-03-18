using Android.Hardware.Input;
using Android.Hardware.Usb;
using Android.OS;
using DalSemi.Serial;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Hardware.Input.InputManager;

namespace iButton_apP.Views
{
    public partial class AboutPage : ContentPage
    {
        Connect connect;
        DalSemi.Serial.SerialPort serialport;

        bool t;

        public AboutPage()
        {
            InitializeComponent();
            
        }

        string temp;


        public void button1_Clicked(object sender, EventArgs e)
        {
            
                lable1.Text = temp;
            

            //connect = new Connect();
            //if (connect.GetFirstDevice)
            //    lable1.Text = "GetFirstDevice=true";
            //if (!connect.GetFirstDevice)
            //    lable1.Text = "GetFirstDevice=false";
        }
    }
}