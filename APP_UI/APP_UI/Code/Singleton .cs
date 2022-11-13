using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TETRA_Coverage_Monitor.Code
{
    public static class Singleton
    {
        public static string Missing_Inserted_Data_Message = "Missing required data";
        public static string Wrong_Format_Data_Message = "Incorrect Format";
        public static string Opertation_Successful_Message = "Operation Successful!";
        public static string Operation_failed_Message = "Operation Failed";
        public static string Wrong_issi_input_message = "Incorrect issi input";
        public static string Radio_Picture_Uri = AppDomain.CurrentDomain.BaseDirectory+ @"Files\Icons\walkie_talkie.png";
        public static string site_picture_uri = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\radiotower.png";
        public static string radio_picture_uri = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\walkie_talkie.png";
        public static string Radio_layer = "Radios_Layer_1";
        public static string Site_layer = "Alarm_Layer_1";// Placemark attribute
        //login , privilage
        public static localhost1.Users user_obj;

    }
}
