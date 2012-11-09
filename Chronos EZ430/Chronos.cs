/********************************************************************************
 *                                                                              *
 * Project: eZ430_Chronos_Net                                                   *
 *                                                                              *
 * COPYRIGHT AND PERMISSION NOTICE                                              *
 *                                                                              *
 * Copyright (c) 2010 Tobias Gaertner (tobias@nospace.de)                       *
 *                                                                              *
 * All rights reserved.                                                         *
 *                                                                              *
 * Permission to use, copy, modify, and distribute this software for any        *
 * purpose with or without fee is hereby granted, provided that the above       *
 * copyright notice and this permission notice appear in all copies.            *
 *                                                                              *
 * THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS     *
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,  *
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF THIRD-PARTY RIGHTS.  *
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,  *
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR        *
 * OTHERWISE, ARISING FROM, OUT OF OR INCONNECTION WITH THE SOFTWARE OR THE     *
 * USE OR OTHER DEALINGS IN THE SOFTWARE.                                       *
 *                                                                              *
 * Except as contained in this notice, the name of a copyright holder shall     *
 * not be used in advertising or otherwise to promote the sale, use or other    *
 * dealings in this Software without prior written permission of the copyright  *
 * holder.                                                                      *
 *                                                                              *
 * You may opt to use, copy, modify, merge, publish, distribute and/or sell     *
 * copies of this Software, and permit persons to whom the Software is          *
 * furnished to do so, under these terms.                                       *
 *                                                                              *
 ********************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

namespace lucidcode.LucidScribe.Plugin.TI.EZ430
{
    /// <summary>
    /// Communication class to communicate with eZ430-Chronos access point (with standard firmware)
    /// </summary>
    public class Chronos
    {
        #region Fields

        private SerialPort _port;
        private List<byte> _buffer = new List<byte>();

        #endregion

        #region Properties

        public bool PortOpen
        {
            get
            {
                if (_port != null)
                {
                    return _port.IsOpen;
                }
                return false;
            }
        }

        #endregion

        #region General Methods

        /// <summary>
        /// Reset RF access point
        /// </summary>
        /// <returns></returns>
        public bool ResetAP()
        {
            // can't realize why there is sent a zero byte and received one byte which isn't used,
            // but it's written this way in the c++ lib

            byte[] data = new byte[1] { 0x00 };
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_RESET, data), data.Length, 1);
            return CheckResponse(response);
        }

        /// <summary>
        ///  Get RF access point dummy ID - used by live check
        /// </summary>
        /// <param name="ID">ID of the connected access point</param>
        /// <returns></returns>
        public bool GetID(out UInt32 ID)
        {
            byte[] data = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_GET_PRODUCT_ID, data), data.Length, 1);

            ID = (uint)(response.Data[Constants.PACKET_DATA_START + 3] << 24) +
                (uint)(response.Data[Constants.PACKET_DATA_START + 2] << 16) +
                (uint)(response.Data[Constants.PACKET_DATA_START + 1] << 8) +
                (response.Data[Constants.PACKET_DATA_START]);

            return CheckResponse(response);
        }

        /// <summary>
        /// Get generic hardware status
        /// </summary>
        /// <param name="status">
        /// Returns one of the following values:
        /// HW_IDLE, HW_SIMPLICITI_STOPPED, HW_SIMPLICITI_TRYING_TO_LINK, HW_SIMPLICITI_LINKED,
        /// HW_BLUEROBIN_STOPPED, HW_BLUEROBIN_TRANSMITTING, HW_ERROR, HW_NO_ERROR, HW_NOT_CONNECTED
        /// </param>
        /// <returns></returns>
        public bool GetAPStatus(out APStatus status)
        {
            byte[] data = new byte[1] { 0x00 };
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_GET_STATUS, data), data.Length, 1);
            status = (APStatus)response.Data[Constants.PACKET_DATA_START];
            return CheckResponse(response);
        }

        private Packet SendAndReceive(Packet packet, int receive, int delay)
        {
            if (_port.IsOpen) _port.DiscardOutBuffer();
            SendPacket(packet);
            Thread.Sleep(delay);
            Packet result = GetResponse(receive + Constants.PACKET_OVERHEAD_BYTES);

            //if (_port.IsOpen) _port.DiscardInBuffer();
            //_buffer.Clear();

            return result;
        }

        private bool CheckResponse(Packet response)
        {
            APStatus BM_errorstate = (APStatus)response.Data[Constants.PACKET_BYTE_CMD];
            if (BM_errorstate != APStatus.HW_NO_ERROR) return false;
            return true;
        }

        #endregion

        #region SimpliciTI methods

        /// <summary>
        /// Start SimpliciTI stack in acc/ppt mode
        /// </summary>
        /// <returns></returns>
        public bool StartSimpliciTI()
        {
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_START_SIMPLICITI, null), 0, 1);
            return CheckResponse(response);
        }

        /// <summary>
        /// Stop SimpliciTI stack
        /// </summary>
        /// <returns></returns>
        public bool StopSimpiliTI()
        {
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_STOP_SIMPLICITI, null), 0, 1);
            return CheckResponse(response);
        }

        /// <summary>
        /// Get SimpliciTI acc/ppt data (4 bytes)
        /// </summary>
        /// <returns></returns>
        public bool GetData(out UInt32 data)
        {
            byte[] send = new byte[4] { 0x00, 0x00, 0x00, 0x00 };
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_GET_SIMPLICITIDATA, send), send.Length, 1);

            data = (uint)(response.Data[Constants.PACKET_DATA_START + 3] << 24) +
                (uint)(response.Data[Constants.PACKET_DATA_START + 2] << 16) +
                (uint)(response.Data[Constants.PACKET_DATA_START + 1] << 8) +
                (response.Data[Constants.PACKET_DATA_START]);

            return CheckResponse(response);
        }


        /* SimpliciTI sync mode methods **********************************************************************************/

        /// <summary>
        /// Start RF access point sync mode
        /// </summary>
        /// <returns></returns>
        public bool StartSyncMode()
        {
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_SYNC_START, null), 0, 1);
            return CheckResponse(response);
        }

        /// <summary>
        /// Send command packet to watch
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendSyncCommand(byte[] data)
        {
            if (data.Length > Constants.BM_SYNC_DATA_LEN)
                return false;
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_SYNC_SEND_COMMAND, data), data.Length, 1);
            return CheckResponse(response);
        }

        /// <summary>
        /// Send command packet to watch, wait a given time, and receive a packet with a given length
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendSyncCommand(byte[] data, int rcvlength, int delay)
        {
            if (data.Length > Constants.BM_SYNC_DATA_LEN)
                return false;
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_SYNC_SEND_COMMAND, data), rcvlength, delay);
            return CheckResponse(response);
        }

        /// <summary>
        /// Get sync buffer status from RF access point
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool GetSyncBufferStatus(out SyncStatus status)
        {
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_SYNC_GET_BUFFER_STATUS, null), 1, 0);
            status = (SyncStatus)response.Data[Constants.PACKET_DATA_START];
            return CheckResponse(response);
        }

        /// <summary>
        /// Read data bytes from sync buffer
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ReadSyncBuffer(out byte[] data)
        {
            Packet response = SendAndReceive(Packet.Create(APCommand.BM_SYNC_READ_BUFFER, null), Constants.BM_SYNC_DATA_LEN, 0);

            data = new byte[Constants.BM_SYNC_DATA_LEN];
            for (int i = 0; i < Constants.BM_SYNC_DATA_LEN; i++)
                data[i] = response.Data[Constants.PACKET_DATA_START + i];

            return CheckResponse(response);
        }

        #endregion

        #region Blue Robin methods

        // nothing here yet

        #endregion

        #region Com Port methods

        /// <summary>
        /// Get COM port name from Windows system using friendly name
        /// </summary>
        /// <returns></returns>
        public string GetComPortName()
        {
            Hashtable table = PortName.BuildPortNameHash(SerialPort.GetPortNames());

            string friendlyName = "TI CC1111 Low-Power RF to USB CDC Serial Port";
            foreach (string key in table.Keys)
            {
                if (key.StartsWith(friendlyName))
                {
                    return (String)table[key];
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Open COM port
        /// </summary>
        /// <param name="portName">"COMx" - May be automatically determined with <seealso cref="GetComPortName"/>GetComPortName</seealso> </param>
        /// <returns></returns>
        public bool OpenComPort(String portName)
        {
            if (_port != null && _port.IsOpen)
            {
                return false;
            }
            _buffer.Clear();
            _port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
            _port.WriteTimeout = 30000;
            _port.ReadTimeout = 30000;
            try
            {
                _port.Handshake = Handshake.RequestToSendXOnXOff;
                _port.DtrEnable = true;
                _port.RtsEnable = true;
                _port.ReceivedBytesThreshold = 1;
                _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                _port.Open();
            }
            catch (Exception)
            {
                if (_port.IsOpen) _port.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Close COM port
        /// </summary>
        public void CloseComPort()
        {
            if (_port == null || !_port.IsOpen)
            {
                return;
            }
            try
            {
                _port.Close();
            }
            catch (Exception)
            {
            }
            _buffer.Clear();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] buf = new byte[sp.BytesToRead];
            sp.Read(buf, 0, buf.Length);
            _buffer.AddRange(buf);
        }

        private void SendPacket(Packet packet)
        {
            if (_port == null || !_port.IsOpen)
            {
                return;
            }
            try
            {
                _port.Write(packet.Data, 0, packet.Data.Length);
                while (_port.BytesToWrite > 0) Thread.Sleep(1);
            }
            catch (Exception)
            {
                // exception handling
            }
        }

        private Packet GetResponse(int count)
        {
            if (_port == null || !_port.IsOpen)
            {
                return Packet.CreateResponse(new byte[count]);
            }
            int loop = 0;
            while (_buffer.Count < count)
            {
                if (loop++ > _port.ReadTimeout)
                {
                    return Packet.CreateResponse(new byte[count]);
                }
                Thread.Sleep(1);
            }
            Packet packet = Packet.CreateResponse(_buffer.GetRange(0, count).ToArray());
            _buffer.RemoveRange(0, count);
            return packet;
        }

        #endregion
    }
}
