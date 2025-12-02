using ActUtlTypeLib;
using System;
using System.Windows.Forms;

namespace Wagaon_bulge
{
    internal class PLC
    {
        public ActUtlType plc = new ActUtlType();


        public String PLc_communication()
        {


            //Try to establish a connection to the PLC
            String conn_Status = "";
            try
            {
                //Set the logical station number to 1
                plc.ActLogicalStationNumber = 1;
                //Open the connection
                plc.Open();
                string PLC_Type; int PlC_No;
                int PLC_Status = plc.GetCpuType(out PLC_Type, out PlC_No);
                if (PLC_Status == 0) { conn_Status = "Connected"; } else { conn_Status = "Diconnected"; }



            }
            catch
            {
                //Display an error message if the connection fails
                conn_Status = "Diconnected";
                MessageBox.Show("PLC _Communication error");
            }
            //Return the connection status
            return conn_Status;
        }

        // read ultrasonic sensor status =======================================
        short sensor1, sensor2, sensor3, sensor4, LH_Image_Tgr, RH_Image_Tgr, count_sensor;


        
        public int LH_Sensor_1()
        {
            try
            {
                plc.GetDevice2("D0", out sensor1); // LH Sesnor-UP Status
            }
            catch { };
            return sensor1;
            
           
        } 
        public int LH_Sensor_2()
        {

            try
            {
                plc.GetDevice2("D1", out sensor2); // LH Sesnor-DN Status
            } catch { };
            return sensor2;
        }


        //This code retrieves the value of a sensor from a PLC device and returns it. 
        public int RH_Sensor_1()
        {




            //Retrieve the value of the sensor from the PLC device
            try
            {
                plc.GetDevice2("D2", out sensor3);
            } catch { };
            //Return the sensor value
            return sensor3;
        }
        public int RH_Sensor_2()
        {


            try
            {
                plc.GetDevice2("D3", out sensor4); // RH Sesnor-DN Status
            } catch { };

            return sensor4;
        }

        public int LH_image_Trigger()
        {
            try
            {
                plc.GetDevice2("D10", out LH_Image_Tgr); // RH Sesnor-DN Status
            } catch { };

            return LH_Image_Tgr;
        }

        public int RH_image_Trigger()
        {


            try
            {
                plc.GetDevice2("D20", out RH_Image_Tgr); // RH Sesnor-DN Status
            }
            catch { }

            return RH_Image_Tgr;
        }

        short boggie_No;

        public int boggie_no_read()
        {
            try
            {
                plc.GetDevice2("D30", out boggie_No); // Boggie No Counter 
            } catch { };

            return boggie_No;
        }

        public int Count_Sensor_1()
        {
            try
            {
                plc.GetDevice2("D4", out count_sensor); // Counter Sesnor 
            } catch { };
            return count_sensor;
        }

        short test_no;
        public int test_No()
        {
            try
            {
                plc.GetDevice2("D200", out test_no); // test no 
            } catch { };
            return test_no;
        }

        short image_resultlog;
        public int image_resultlog_read()
        {
            try
            {
                plc.GetDevice2("D25", out image_resultlog); //bulge image log Trigger 
            } catch { };
            return image_resultlog;
        }

        // Reset All bit afte bulge image logging ======
        public void reset_all_data()
        {
            try {
                plc.SetDevice2("D10", 0);
                plc.SetDevice2("D20", 0);
                plc.SetDevice2("D25", 0);
                } catch { }
        }

    }
}
