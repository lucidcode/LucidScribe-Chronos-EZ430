using System;
using System.Collections.Generic;

namespace lucidcode.LucidScribe.Plugin.TI.EZ430
{

    public static class Device
    {

        static Chronos m_objEZ = new Chronos();
        private static bool m_boolInitialized;
        private static bool m_boolRestarted;

        static double m_dblV;
        static double m_dblX;
        static double m_dblY;
        static double m_dblZ;

        public static Boolean Initialize()
        {
            try
            {
                if (!m_boolInitialized)
                {
                    string p = m_objEZ.GetComPortName();
                    if (p == "") { throw (new Exception("port not found.")); }
                    m_objEZ.OpenComPort(p);
                    m_objEZ.StartSimpliciTI();

                    m_boolInitialized = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw (new Exception("The 'TI EZ430' plugin failed to initialize: " + ex.Message));
            }
        }

        public static void UpdateValues()
        {
            uint data;

            if (!m_objEZ.PortOpen)
            {
                if (!m_boolRestarted)
                {
                    m_boolRestarted = true;
                    string p = m_objEZ.GetComPortName();
                    if (p != "")
                    {
                        m_objEZ.OpenComPort(p);
                        m_objEZ.StartSimpliciTI();
                        m_boolRestarted = false;
                    }
                }
            }

            m_objEZ.GetData(out data);

            int intOverFlowTest = (sbyte)((data) & (UInt32)255);
            if (intOverFlowTest != -1)
            {
                m_dblX = (sbyte)((data >> 8) & (UInt32)255);
                m_dblY = (sbyte)((data >> 16) & (UInt32)255);
                m_dblZ = (sbyte)((data >> 24) & (UInt32)255);

                if (m_dblX < 0)
                {
                    m_dblX = m_dblX * -1;
                }
                if (m_dblY < 0)
                {
                    m_dblY = m_dblY * -1;
                }
                if (m_dblZ < 0)
                {
                    m_dblZ = m_dblZ * -1;
                }
                if (m_dblV < 0)
                {
                    m_dblV = m_dblV * -1;
                }

                m_dblV = m_dblX + m_dblY + m_dblZ;
            }

        }

        public static Double GetValue()
        {
            return m_dblV;
        }

        public static Double GetValueX()
        {
            return m_dblX;
        }

        public static Double GetValueY()
        {
            return m_dblY;
        }

        public static Double GetValueZ()
        {
            return m_dblZ;
        }

    }

    namespace TIEZ430
    {
        public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
        {
            public override string Name
            {
                get { return "TI EZ430"; }
            }
            public override bool Initialize()
            {
                return Device.Initialize();
            }
            public override double Value
            {
                get
                {
                    Device.UpdateValues();
                    double dblValue = Device.GetValue();
                    if (dblValue > 999) { dblValue = 999; }
                    return dblValue;
                }
            }
        }
    }

    namespace REM
    {
        public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
        {

            private Double m_dblLastValue; // The value from 100 milliseconds ago
            private Double m_dblInactivityCount; // The amount of ticks since the last move
            private Int32 m_intVibrationsCountDown; // Count down to wait for the vibrations to pass and REM to begin
            private Int32 m_intREMCountDown; // Count down to show the REM line

            public override string Name
            {
                get { return "TI EZ430 REM"; }
            }

            public override bool Initialize()
            {
                return Device.Initialize();
            }

            public override double Value
            {
                get
                {

                    Double dblCurrentValue = Device.GetValue();
                    Double dblDifference = dblCurrentValue - m_dblLastValue;

                    // Calculate the amount of movement since the last tick
                    if (dblDifference < 0) { dblDifference = dblDifference * -1; }

                    // Check if the movement was significant
                    if (dblDifference > 30)
                    {
                        // Check if the movement happened after 15 minutes (*600 ticks a minute)
                        if (m_dblInactivityCount > 9000)
                        {
                            // Activate the timer to wait for the vibrations to pass
                            m_intVibrationsCountDown = 420;
                        }

                        // Reset the inactivity count since movement was detected
                        m_dblInactivityCount = 0;
                    }
                    else
                    {
                        // Increment the inactivity count
                        m_dblInactivityCount += 1;
                    }

                    // Check if we need to count down the vibrations
                    if (m_intVibrationsCountDown > 0)
                    {
                        m_intVibrationsCountDown -= 1;

                        // Check if the vibrations have passed
                        if (m_intVibrationsCountDown == 0)
                        {
                            // Activate the timer to show REM and play the anthem
                            m_intREMCountDown = 210;
                        }
                    }

                    if (m_intREMCountDown > 0)
                    {
                        m_intREMCountDown -= 1;
                        return 888;
                    }

                    return 0;
                }
            }
        }
    }

    namespace X
    {
        public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
        {
            public override string Name
            {
                get { return "TI EZ430 X"; }
            }
            public override bool Initialize()
            {
                return Device.Initialize();
            }
            public override double Value
            {
                get
                {
                    double dblValue = Device.GetValueX();
                    if (dblValue > 999) { dblValue = 999; }
                    return dblValue;
                }
            }
        }
    }

    namespace Y
    {
        public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
        {
            public override string Name
            {
                get { return "TI EZ430 Y"; }
            }
            public override bool Initialize()
            {
                return Device.Initialize();
            }
            public override double Value
            {
                get
                {
                    double dblValue = Device.GetValueY();
                    if (dblValue > 999) { dblValue = 999; }
                    return dblValue;
                }
            }
        }
    }

    namespace Z
    {
        public class PluginHandler : lucidcode.LucidScribe.Interface.LucidPluginBase
        {
            public override string Name
            {
                get { return "TI EZ430 Z"; }
            }
            public override bool Initialize()
            {
                return Device.Initialize();
            }
            public override double Value
            {
                get
                {
                    double dblValue = Device.GetValueZ();
                    if (dblValue > 999) { dblValue = 999; }
                    return dblValue;
                }
            }
        }
    }
}
