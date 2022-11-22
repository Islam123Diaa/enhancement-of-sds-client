using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TETRA_Coverage_Monitor.localhost1;
using TETRA_Coverage_Monitor;
using System.Threading;
using System.Windows.Controls;
using SC3_Alarm_Application_Server;
using C1.Win.C1Ribbon;
using System.Windows.Forms.Integration;
using SYSTEL_ESRI_Control;
using Esri.ArcGISRuntime.ArcGISServices;
using static SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control;


namespace TETRA_Coverage_Monitor.Code
{
    public class Web_service_Control
    {
        SDS_Remote_Control_WS WS_Obj = new SDS_Remote_Control_WS();
        public Web_service_Control()
        {

        }

        #region zones
        public Zone[] Select_all_zones(string username , string password)
        {
            try
            {
                return WS_Obj.Zone_Select_All(username, password);
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
                return null;
            }
        }
        public bool Add_Zone(string username , string password, Zone new_zone)
        {
            Zone z_obj = WS_Obj.Zone_Insert(username, password, new_zone);
            if (z_obj != null )
            {
                return true;
            }else
            {
                return false;
            }
        }
        public bool Delete_zone(string username , string password , int zone_id)
        {
         bool check = WS_Obj.Zone_Delete(username, password, zone_id);
            if(check)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool Edit_zone(string username , string password , int id , Zone updated_zone)
        {
            bool check = WS_Obj.Zone_Update(username, password, id, updated_zone);
            if(check)
            {
                return true;
            }else
            {
                return false;
            }

        }
        #endregion

        #region sites
        public Sites[] Select_all_sites(string username , string password)
        {
            try
            {
                return WS_Obj.Sites_Select_All(username, password);
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
                return null;
            }

        }
        public bool Add_site(string username , string password , Sites new_site)
        {
            Sites obj = WS_Obj.Site_Insert(username, password, new_site);
            if(obj != null)
            {
                return true;
            }else
            {
                return false;
            }
        }
        public bool Delete_site(string username , string password , int id)
        {
            bool check = WS_Obj.Sites_Delete(username, password, id);
            if(check)
            {
                return true;
            }else
            {
                return false;
            }

        }
        public bool Edit_site(string username , string password , int site_id , Sites site)
        {
            bool check = WS_Obj.Site_Update(username, password, site_id, site);
            if (check)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region radios
        public Radios[] Select_all_radios(string username , string password)
        {
            return  WS_Obj.Radios_Select_All(username, password);
        }
        public bool Add_radio(string username , string password , Radios new_radio)
        {
            Radios added_rad = WS_Obj.Radios_Insert(username, password, new_radio);
            if(added_rad != null)
            {
                return true;
            }else
            {
                return false;
            }

        }
        public bool Delete_radio(string username , string password , int id)
        {
            bool check = WS_Obj.Radios_Delete(username, password, id);
            if (check)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool Edit_radio(string username , string password ,int radio_id , Radios radio)
        {
            bool check = WS_Obj.Radios_Update(username, password, radio_id, radio);
            if (check)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region radio_logs
        public Logs[] Get_logs_filtered_by_Pos_ID(string username , string password , int pos_id)
        {
            return  WS_Obj.Logs_Select_By_Pos_ID(username, password, pos_id);
        }
        public Logs[] Get_logs_filtered_by_datetime_radioType(string username , string password , DateTime start_date , DateTime end_date , string type)
        {
            return WS_Obj.Logs_Select_By_Date_Time_RadioType(username, password, start_date, end_date, type);
        }
        public Logs[] Get_logs_filtered_by_datetime_SiteID_radioType(string username , string password , DateTime start_date,DateTime end_date , string type , int id)
        {
          return WS_Obj.Logs_Select_By_Date_Time_SiteID_RadioType(username, password, start_date, end_date, type, id);
        }
        public Logs Get_one_log_filtered_by_Pos_ID_with_max_rssi(string username , string password , int id)
        {
            return WS_Obj.Logs_Select_Max_By_Pos_ID(username, password, id);

        }
        #endregion

        #endregion

        #region city
        public City[] Select_All_cities(string Username, string Password)
        {
            return WS_Obj.City_Select_All(Username, Password);
        }
        public bool Add_city(string username , string password , City new_city)
        {
            City added_city = WS_Obj.City_Insert(username, password, new_city);
            if (added_city != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Delete_city(string username , string password , int city_id)
        {
            bool check = WS_Obj.City_Delete(username, password, city_id);
            if (check)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool Edit_city(string username , string password , int city_id , City city)
        {
            bool check = WS_Obj.City_Update(username, password, city_id, city);
            if (check)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
        public Users Select_users(string Username, string Password)
        {
            return WS_Obj.Users_Select(Username, Password);
        }
    }
}
