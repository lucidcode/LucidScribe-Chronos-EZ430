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

namespace lucidcode.LucidScribe.Plugin.TI.EZ430
{
    /// <summary>
    /// COM port packet format
    ///
    /// Byte 0	Start marker (0xFF)
    /// Byte 1	Command code
    /// Byte 2	Packet size (including overhead)
    /// Byte 3+	Data
    /// </summary>
    class Packet
    {
        private byte[] m_data;

        public byte[] Data
        {
            get { return m_data; }
        }

        private Packet(byte[] data)
        {
            m_data = data;
        }
        
        /// <summary>
        /// Create a new packet
        /// </summary>
        /// <param name="cmd">packet command</param>
        /// <param name="data">data to transfer</param>
        /// <returns></returns>
        public static Packet Create(APCommand cmd, byte[] data)
        {
            byte len = (data == null) ? (byte)0 : (byte)data.Length;
            byte[] tx_buf = new byte[len + Constants.PACKET_OVERHEAD_BYTES];

            tx_buf[0] = 0xFF;		                                            // Start marker
            tx_buf[1] = (byte)cmd;                                              // Command code
            tx_buf[2] = (byte)(len + Constants.PACKET_OVERHEAD_BYTES);          // Packet length
            if (data != null)
                for (int i = 0; i < data.Length; i++)
                    tx_buf[Constants.PACKET_OVERHEAD_BYTES + i] = data[i];	    // Packet data (no reordering)

            return new Packet(tx_buf);
        }

        public static Packet CreateResponse(byte[] data)
        {
            return new Packet(data);
        }

    }
}
