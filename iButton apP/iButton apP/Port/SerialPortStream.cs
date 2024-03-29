﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace iButton_apP.Port
{
    class SerialPortStream : Stream, ISerialStream, IDisposable
    {
        int fd;
        int read_timeout;
        int write_timeout;
        bool disposed;

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern int open_serial(string portName);

        internal SerialPortStream(string portName, int baudRate, int dataBits, Parity parity, StopBits stopBits,
                bool dtrEnable, bool rtsEnable, Handshake handshake, int readTimeout, int writeTimeout,
                int readBufferSize, int writeBufferSize, bool IsAVirtualPort)
        {
            fd = open_serial(portName);
            if (fd == -1)
                ThrowIOException();

            if (!set_attributes(fd, baudRate, parity, dataBits, stopBits, handshake))
                ThrowIOException(); // Probably Win32Exc for compatibility

            read_timeout = readTimeout;
            write_timeout = writeTimeout;

            //If a por is Virtual SetSignal  DTR cannot be used
            if (!IsAVirtualPort)
                SetSignal(SerialSignal.Dtr, dtrEnable);

            if (handshake != Handshake.RequestToSend &&
                    handshake != Handshake.RequestToSendXOnXOff && !IsAVirtualPort)
                SetSignal(SerialSignal.Rts, rtsEnable);
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return true;
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return read_timeout;
            }
            set
            {
                if (value < 0 && value != SerialPort.InfiniteTimeout)
                    throw new ArgumentOutOfRangeException("value");

                read_timeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return write_timeout;
            }
            set
            {
                if (value < 0 && value != SerialPort.InfiniteTimeout)
                    throw new ArgumentOutOfRangeException("value");

                write_timeout = value;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override void Flush()
        {
            // If used, this _could_ flush the serial port
            // buffer (not the SerialPort class buffer)
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern int read_serial(int fd, byte[] buffer, int offset, int count);


        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern bool poll_serial(int fd, out int error, int timeout);

        public override int Read([In, Out] byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException("offset or count less than zero.");

            if (buffer.Length - offset < count)
                throw new ArgumentException("offset+count",
                                  "The size of the buffer is less than offset + count.");

            int error;
            bool poll_result = poll_serial(fd, out error, read_timeout);
            if (error == -1)
                ThrowIOException();

            if (!poll_result)
            {
                // see bug 79735   http://bugzilla.ximian.com/show_bug.cgi?id=79735
                // should the next line read: return -1; 
                throw new TimeoutException();
            }

            return read_serial(fd, buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern int write_serial(int fd, byte[] buffer, int offset, int count, int timeout);

        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException();

            if (buffer.Length - offset < count)
                throw new ArgumentException("offset+count",
                                 "The size of the buffer is less than offset + count.");

            if (write_serial(fd, buffer, offset, count, write_timeout) < 0)
                throw new TimeoutException("The operation has timed-out");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposed = true;
            close_serial(fd);
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern void close_serial(int fd);

        public override void Close()
        {
            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SerialPortStream()
        {
            Dispose(false);
        }

        void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern bool set_attributes(int fd, int baudRate, Parity parity, int dataBits, StopBits stopBits, Handshake handshake);

        public void SetAttributes(int baud_rate, Parity parity, int data_bits, StopBits sb, Handshake hs)
        {
            if (!set_attributes(fd, baud_rate, parity, data_bits, sb, hs))
                ThrowIOException();
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern int get_bytes_in_buffer(int fd, int input);

        public int BytesToRead
        {
            get
            {
                return get_bytes_in_buffer(fd, 1);
            }
        }

        public int BytesToWrite
        {
            get
            {
                return get_bytes_in_buffer(fd, 0);
            }
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern void discard_buffer(int fd, bool inputBuffer);

        public void DiscardInBuffer()
        {
            discard_buffer(fd, true);
        }

        public void DiscardOutBuffer()
        {
            discard_buffer(fd, false);
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern SerialSignal get_signals(int fd, out int error);

        public SerialSignal GetSignals()
        {
            int error;
            SerialSignal signals = get_signals(fd, out error);
            if (error == -1)
                ThrowIOException();

            return signals;
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern int set_signal(int fd, SerialSignal signal, bool value);

        public void SetSignal(SerialSignal signal, bool value)
        {
            if (signal < SerialSignal.Cd || signal > SerialSignal.Rts ||
                    signal == SerialSignal.Cd ||
                    signal == SerialSignal.Cts ||
                    signal == SerialSignal.Dsr)
                throw new Exception("Invalid internal value");

            if (set_signal(fd, signal, value) == -1)
                ThrowIOException();
        }

        [DllImport("MonoPosixHelper", SetLastError = true)]
        static extern int breakprop(int fd);

        public void SetBreakState(bool value)
        {
            if (value)
                breakprop(fd);
        }

        [DllImport("libc")]
        static extern IntPtr strerror(int errnum);

        static void ThrowIOException()
        {
            int errnum = Marshal.GetLastWin32Error();
            string error_message = Marshal.PtrToStringAnsi(strerror(errnum));

            throw new IOException(error_message);
        }
    }
}
