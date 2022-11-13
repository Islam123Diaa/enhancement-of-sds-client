using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TETRA_Coverage_Monitor.localhost1;
using TETRA_Coverage_Monitor.localhost1;

namespace TETRA_Coverage_Monitor
{
    
    class SDS_Remote_WS
    {
        SDS_Remote_Control_WS SDS_Remote_Control_WS_Obj = new SDS_Remote_Control_WS();


        #region users
        public Users[] Users_Select_ALL(string username, string password)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Select_All(username, password);
            }
            catch
            {
                return null;
            }
        }
        public Users Users_Insert(string username, string password, int userid, Users user)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Insert(username, password, userid, user);
            }
            catch
            {
                return null;
            }
        }
        public Users Users_Select(string username, string password)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Select(username, password);
            }
            catch
            {
                return null;
            }
        }
        public Users Users_SelectByUserId(string username, string password, int userid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_SelectByUserId(username, password, userid);
            }
            catch
            {
                return null;
            }
        }
        public bool Users_Update(string username, string password, Users user)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Update(username, password, user);
            }
            catch
            {
                return false;
            }
        }

        public bool Users_Admin_Delete(string username, string password, int userid, int adminid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Admin_Delete(username, password, userid, adminid);
            }
            catch
            {
                return false;
            }
        }
        public Users_Admin Users_Admin_Insert(string username, string password, int userid, int adminid, Users_Admin user)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Admin_Insert(username, password, userid, adminid, user);
            }
            catch
            {
                return null;
            }
        }
        public int Users_Admin_Insert_Bulk(string username, string password, int userid, string query)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Admin_Insert_Bulk(username, password, userid, query);
            }
            catch
            {
                return -1;
            }
        }
        public Users_Admin Users_Admin_SelectByAdminId(string username, string password, int adminid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Admin_SelectByAdminId(username, password, adminid);
            }
            catch
            {
                return null;
            }
        }

        public Users_Admin Users_Admin_SelectByUserId(string username, string password, int userid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Admin_SelectByUserId(username, password, userid);
            }
            catch
            {
                return null;
            }
        }

        public Users_Admin[] Users_Admin_Select_All(string username, string password)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Users_Admin_Select_All(username, password);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        
        #region Zone 


        public bool Zone_Delete(string username, string password, int zoneid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Zone_Delete(username, password, zoneid);
            }
            catch
            {
                return false;
            }
        }
        public Zone Zone_Insert(string username, string password, Zone zone)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Zone_Insert(username, password, zone);

            }
            catch
            {
                return null;
            }
        }


        public Zone Zone_Select_By_ID(string username, string password, int zoneID)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Zone_Select_By_ID(username, password, zoneID);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Zone[] Zone_Select_All(string username, string password)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Zone_Select_All(username, password);
            }
            catch
            {
                return null;
            }
        }

        public bool Zone_Update(string username, string password, int zoneid, Zone zone)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Zone_Update(username, password, zoneid, zone);

            }
            catch
            {
                return false;
            }
        }

        public Zone Zone_Select_By_Name(string username, string password, string zoneName)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Zone_Select_By_Name(username, password, zoneName);
            }
            catch (Exception e)
            {
                return null;
            }
        }


        #endregion

        #region sites
        public bool Sites_Delete(string username, string password, int siteid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Sites_Delete(username, password, siteid);
            }catch
            {
                return false;
            }
        }

        public Sites Site_Insert(string username, string password, int userid, Sites site)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Site_Insert(username, password,  site);
            }catch
            {
                return null;
            }
        }

        public Sites Site_Select_By_ID(string username, string password, int siteid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Site_Select_By_ID(username, password, siteid);
            }catch
            {
                return null;
            }
        }

        public Sites[] Sites_Select_All(string username, string password)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Sites_Select_All(username, password);
            }catch
            {
                return null;
            }
        }

        public Sites[] Sites_Select_By_ZoneID(string username, string password, int zoneid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Sites_Select_By_ZoneID(username, password, zoneid);
            }catch
            {
                return null;
            }
        }

        public Sites Sites_Select_By_SiteName(string username, string password, string sitename)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Sites_Select_By_SiteName(username, password, sitename);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Sites Sites_Select_By_LA(string username, string password, string LA)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Sites_Select_By_LA(username, password, LA);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool Site_Update(string username, string password, int siteid, Sites site)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Site_Update(username, password, siteid, site);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region radios
        public bool Radios_Delete(string username, string password, int radioid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radios_Delete(username, password, radioid);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Radios Radios_Insert(string username, string password, Radios radio)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radios_Insert(username, password, radio);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Radios Radios_SelectByISSI(string username, string password, int issi)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radios_SelectByISSI(username, password, issi);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Radios Radios_SelectBySerialNum(string username, string password, int SerialNum)
        {
            try
            {

                return SDS_Remote_Control_WS_Obj.Radios_SelectBySerialNum(username, password, SerialNum);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Radios[] Radios_SelectByModel(string username, string password, string Model)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radios_SelectByModel(username, password, Model);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Radios Radios_SelectByTEI(string username, string password, int TEI)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radios_SelectByTEI(username, password, TEI);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Radios [] Radios_SelectBySiteID(string username, string password, int siteid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radios_SelectBySiteID(username, password, siteid);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Radios Radios_SelectByRadioID(string username, string password, int radioid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radios_SelectByRadioID(username, password, radioid);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Radios[] Radios_Select_All(string username, string password)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radios_Select_All(username, password);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool Radios_Update(string username, string password, int radioid, Radios radio)
        {
            try
            {

                return SDS_Remote_Control_WS_Obj.Radios_Update(username, password, radioid, radio);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Radio_Channel

        public Radio_Channel Radio_Channel_Insert(string username, string password, int channelid, int radioid, Radio_Channel radiochannel)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radio_Channel_Insert(username, password, radiochannel);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Radio_Channel[] Radio_Channel_Select_All(string username, string password)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radio_Channel_Select_All(username, password);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Channels[] Radio_Channel_Select_Channels(string username, string password, int radioid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radio_Channel_Select_Channels(username, password, radioid);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Radios[] Radio_Channel_Select_Radios(string username, string password, int channelid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radio_Channel_Select_Radios(username, password, channelid);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool Radio_Channel_Update(string username, string password, int channelid, Radio_Channel radiochannel)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Radio_Channel_Update(username, password, channelid, radiochannel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Channels
        public bool Channel_Delete(string username, string password, int channelid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Channel_Delete(username, password, channelid);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Channels Channel_Insert(string username, string password, Channels channel)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Channel_Insert(username, password, channel);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Channels Channels_Select_By_GSSI(string username, string password, int gssi)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Channels_Select_By_GSSI(username, password, gssi);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Channels Channels_Select_By_ID(string username, string password, int channelid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Channels_Select_By_ID(username, password, channelid);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Channels Channels_Select_By_Name(string username, string password, string channelname)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Channels_Select_By_Name(username, password, channelname);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Channels[]Channel_Select_All(string username, string password)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Channel_Select_All(username, password);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public bool Channel_Update(string username, string password, int channelid, Channels channel)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Channel_Update(username, password, channelid, channel);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Logs


        public Logs[] Logs_Select_By_Date_Time(string username, string password, DateTime start, DateTime End)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Logs_Select_By_Date_Time(username, password, start, End);
            }
            catch (Exception e)
            {

                return null;
            }
        }
        public Logs[] Logs_Select_By_Date_Time_RadioType(string username, string password, DateTime start, DateTime End, string radioType)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Logs_Select_By_Date_Time_RadioType(username, password, start, End, radioType);
            }
            catch (Exception e)
            {

                return null;
            }
        }
        public Logs[] Logs_Select_By_Date_Time_SiteID_RadioType(string username, string password, DateTime start, DateTime End, string radioType, int siteID)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Logs_Select_By_Date_Time_SiteID_RadioType(username, password, start, End, radioType , siteID);
            }
            catch (Exception e)
            {

                return null;
            }
        }
        public Logs[] Logs_Select_By_Radio_ID(string username, string password, int radioid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Logs_Select_By_Radio_ID(username, password, radioid);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Logs[] Logs_Select_By_Site_ID(string username, string password, int siteid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Logs_Select_By_Site_ID(username, password, siteid);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Logs[] Logs_Select_By_LA(string username, string password, string LA)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Logs_Select_By_LA(username, password, LA);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Logs[] Logs_Select_By_Pos_ID(string username, string password, int posid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Logs_Select_By_Pos_ID(username, password, posid);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public Logs Logs_Select_Max_By_Pos_ID(string username, string password, int posid)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.Logs_Select_Max_By_Pos_ID(username, password, posid);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region GPS_Position

        public bool GPS_Position_Update(string username, string password, int radioid, GPS_Position position)
        {
            try
            {
                return SDS_Remote_Control_WS_Obj.GPS_Position_Update(username, password, radioid, position);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion





    }


}
