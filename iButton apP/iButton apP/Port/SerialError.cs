﻿namespace iButton_apP.Port
{
    public enum SerialError
    {
        RXOver = 1,
        Overrun = 2,
        RXParity = 4,
        Frame = 8,
        TXFull = 256
    } 
}
