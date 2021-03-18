using System;
using System.Collections.Generic;
using System.Text;
using DalSemi;
using DalSemi.OneWire;
using DalSemi.OneWire.Adapter;
using DalSemi.Serial;
using System.IO.Ports;
using MonoSerialPort;
using Android.Hardware.Usb;
using Android.Hardware.Input;

namespace iButton_apP
{
    public class Connect
    {

        PortAdapter port_adapter;
        byte[] address;
        public bool GetFirstDevice { get; set; }
        string port_name { get; set; }
        string adapter_name { get; set; }


        public InputManager.IInputDeviceListener Newmethod { get; set; }

        



         
    }
}
