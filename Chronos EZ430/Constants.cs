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
    /// GetAPStatus system states
    /// </summary>
    public enum APStatus : byte
    {
        HW_IDLE = 0x00,
        HW_SIMPLICITI_STOPPED = 0x01,
        HW_SIMPLICITI_TRYING_TO_LINK = 0x02,
        HW_SIMPLICITI_LINKED = 0x03,
        HW_BLUEROBIN_STOPPED = 0x04,
        HW_BLUEROBIN_TRANSMITTING = 0x05,
        HW_ERROR = 0x05,
        HW_NO_ERROR = 0x06,
        HW_NOT_CONNECTED = 0x07
    }

    /// <summary>
    /// Packet commands
    /// </summary>
    internal enum APCommand : byte
    {
        // Generic commands
        BM_RESET = 0x01,
        BM_GET_STATUS = 0x00,
        BM_GET_PRODUCT_ID = 0x20,

        // BlueRobin
        BM_START_BLUEROBIN = 0x02,
        BM_SET_BLUEROBIN_ID = 0x03,
        BM_GET_BLUEROBIN_ID = 0x04,
        BM_SET_HEARTRATE = 0x05,
        BM_SET_SPEED = 0x0A,
        BM_STOP_BLUEROBIN = 0x06,

        // SimpliciTI acc/ppt mode
        BM_START_SIMPLICITI = 0x07,
        BM_GET_SIMPLICITIDATA = 0x08,
        BM_STOP_SIMPLICITI = 0x09,

        // SimpliciTI sync mode
        BM_SYNC_START = 0x30,
        BM_SYNC_SEND_COMMAND = 0x31,
        BM_SYNC_GET_BUFFER_STATUS = 0x32,
        BM_SYNC_READ_BUFFER = 0x33,

        //Wireless BSL
        BM_START_WBSL = 0x40,
        BM_GET_WBSL_STATUS = 0x41,
        BM_INIT_OK_WBSL = 0x42,
        BM_INIT_INVALID_WBSL = 0x43,
        BM_TRANSFER_OK_WBSL = 0x44,
        BM_TRANSFER_INVALID_WBSL = 0x45,
        BM_STOP_WBSL = 0x46,
        BM_SEND_DATA_WBSL = 0x47,
        BM_GET_PACKET_STATUS_WBSL = 0x48,
        BM_GET_MAX_PAYLOAD_WBSL = 0x49,

        // Production test commands
        BM_INIT_TEST = 0x70,
        BM_NEXT_TEST = 0x71,
        BM_WRITE_BYTE = 0x72,
        BM_GET_TEST_RESULT = 0x73
    }

    /// <summary>
    /// Sync status (takes from BM_SYNC.cpp)
    /// </summary>
    public enum SyncStatus : byte
    {
        SYNC_USB_DATA_EMPTY = 0,
        SYNC_USB_DATA_READY = 1,
        SYNC_USB_DATA_LOCKED = 2
    }

    /// <summary>
    /// Constants taken from BM-API.h and BM_SYNC.cpp
    /// </summary>
    public class Constants
    {
        /* BM-API.h *********************************************************************/

        internal const byte PACKET_OVERHEAD_BYTES = 3;
        internal const byte PACKET_DATA_BYTES = 28;
        internal const byte PACKET_TOTAL_BYTES = (PACKET_OVERHEAD_BYTES + PACKET_DATA_BYTES);

        /* BM_SYNC.cpp ******************************************************************/

        // USB transmission packet length
        public const byte SYNC_HEADER_LENGTH = 2;       // 2 + 26 = 28 total data length
        public const byte SYNC_DATA_LENGTH = 26;

        // Shortcuts to address packet byte-wise (indices)
        internal const byte PACKET_BYTE_START = 0;
        internal const byte PACKET_BYTE_CMD = 1;
        internal const byte PACKET_BYTE_SIZE = 2;
        internal const byte PACKET_DATA_START = 3;

        // SimpliciTI sync mode packet length
        public const byte BM_SYNC_DATA_LEN = 19;



        // SimpliciTI sync mode commands (taken from firmware) --> scope of conrete application 
        public const byte SYNC_AP_CMD_NOP = 0x01;
        public const byte SYNC_AP_CMD_GET_STATUS = 0x02;
        public const byte SYNC_AP_CMD_SET_WATCH = 0x03;
        public const byte SYNC_AP_CMD_GET_MEMORY_BLOCKS_MODE_1 = 0x04;
        public const byte SYNC_AP_CMD_GET_MEMORY_BLOCKS_MODE_2 = 0x05;
        public const byte SYNC_AP_CMD_ERASE_MEMORY = 0x06;
        public const byte SYNC_AP_CMD_EXIT = 0x07;

        // Datalog mode (taken from TI Data logger firmware)
        public const byte DATALOG_MODE_HEARTRATE = 1;			
        public const byte DATALOG_MODE_TEMPERATURE = 2;		
        public const byte DATALOG_MODE_ALTITUDE	 = 4;		
        public const byte DATALOG_MODE_ACCELERATION	= 8;	

    }

}
