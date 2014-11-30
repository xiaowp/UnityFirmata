using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Sharpduino.EventArguments;
using Sharpduino.Base;
using UnityEngine;

namespace Sharpduino.SerialProviders
{
    public class ComPortProvider : ISerialProvider
    {
        private SerialPort port;
		private Thread thread;
		private bool isRunning;

        public ComPortProvider(string portName, 
            int baudRate = 57600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            port = new SerialPort(portName,baudRate,parity,dataBits,stopBits);
        }

         #region Proper Dispose Code

        // Proper Dispose code should contain the following. See
        // http://stackoverflow.com/questions/538060/proper-use-of-the-idisposable-interface

        ~ComPortProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected void Dispose(bool shouldDispose)
        {
            if ( shouldDispose )
            {
                // Dispose of the com port as safely as possible
                if ( port != null )
                {
                    if( port.IsOpen )
                        port.Close();
                    port.Dispose();
                    port = null;
                }
            }
        }
        #endregion

        public void Open()
        {
            port.Open();
            if (port.IsOpen) 
			{
				port.DataReceived += ComPort_DataReceived;
				isRunning = true;
				thread = new Thread(ComPort_Read);
				thread.Start ();
			}
        }

        public void Close()
        {
			isRunning = false;
			if (thread != null && thread.IsAlive) 
			{
				thread.Join();
			}

            if (port.IsOpen)
            {
                port.Close();
                port.DataReceived -= ComPort_DataReceived;                    
            }
        }

		private void ComPort_Read()
		{
			while (isRunning && port != null && port.IsOpen) {
				try {
					if (port.BytesToRead > 0) {
						byte[] bytes = new byte[port.BytesToRead];
						port.Read(bytes, 0, bytes.Length);
						//string str=string.Join(",",bytes.Select(t=>t.ToString()).ToArray());
						//Debug.Log ("Recv:" + str);
						OnDataReceived(bytes);
					}
				} catch (System.Exception e) {
					Debug.LogWarning(e.Message);
				}
			}
		}
		
		private void ComPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] bytes = new byte[port.BytesToRead];
            port.Read(bytes, 0, bytes.Length);
            OnDataReceived(bytes);
        }

		public event EventHandler<DataReceivedEventArgs> DataReceived;

		private void OnDataReceived(byte[] bytes)
        {
            var handler = DataReceived;
            if ( handler != null )
                handler(this,new DataReceivedEventArgs(bytes));
        }

        public void Send(IEnumerable<byte> bytes)
        {
            byte[] buffer = bytes.ToArray();
            port.Write(buffer,0,buffer.Length);
			//string str=string.Join(",",buffer.Select(t=>t.ToString()).ToArray());
			//Debug.Log ("Send:" + str);
        }
    }
}