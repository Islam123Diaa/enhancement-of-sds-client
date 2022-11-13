using SYSTEL_ESRI_Control;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
//using static SC3_Alarm_Control.Code.Alarm_WS_Controller;
using static SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control;
using static TETRA_Coverage_Monitor.Esri_control;


namespace TETRA_Coverage_Monitor
{
    public class Esri_control
    {

        public delegate void del_Tapped_Building_AlarmUnit(object sender, TappedReturnEventArgs args, int option);
        public event del_Tapped_Building_AlarmUnit ev_Tapped_Building_AlarmUnit;

        public delegate void TappedEventHandler(object sender, TappedReturnEventArgs args, int option);

        public SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control _Esri_MAP_Control;
        public SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control Load_Esri_Control()
        {
            try
            {
                _Esri_MAP_Control = new SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control("lriRsrLANJyLeDDu", "runtimestandard,101," + "rud336290293" + ",none,GB1P0H4EN96HF5KHT247");
                _Esri_MAP_Control.InitializeComponent();
                _Esri_MAP_Control.Send_TappedMapPoint_Info += _Esri_MAP_Control_Send_TappedMapPoint_Info;

                return _Esri_MAP_Control;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void Add_Map_Layer(bool Online_Or_Offline, string Map_Layer_Name, string Map_Path_Url)
        {
            try
            {
                if (Online_Or_Offline)
                {
                    _Esri_MAP_Control.Add_Online_Map_Layer(Map_Layer_Name, Map_Path_Url);
                }
                else
                {
                    if (File.Exists(Map_Path_Url))
                    {
                        _Esri_MAP_Control.Add_Offline_Map_Layer(Map_Layer_Name, Map_Path_Url);
                    }
                }
            }
            catch (Exception ex)
            {
                //Alarm_Auditing.Error(ex.Message);
            }
        }

        public void Set_View(string[] _Array)
        {
            try
            {
                double Max_Log = 0;
                double Max_Lat = 0;

                double Min_Log = 0;
                double Min_Lat = 0;
                for (int j = 0; j < _Array.Length; j++)
                {
                    string[] _Lat_Lon = _Array[j].Split(',');
                    if (_Lat_Lon.Length == 2)
                    {
                        SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position _MapPoint_Obj = new SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position();
                        _MapPoint_Obj.Latitude = double.Parse(_Lat_Lon[0]);
                        _MapPoint_Obj.Longitude = double.Parse(_Lat_Lon[1]);

                        if (j == 0)
                        {
                            Max_Log = _MapPoint_Obj.Longitude;
                            Max_Lat = _MapPoint_Obj.Latitude;

                            Min_Log = _MapPoint_Obj.Longitude;
                            Min_Lat = _MapPoint_Obj.Latitude;
                        }
                        else
                        {
                            if (_MapPoint_Obj.Longitude > Max_Log)
                            {
                                Max_Log = _MapPoint_Obj.Longitude;
                            }

                            if (_MapPoint_Obj.Latitude > Max_Lat)
                            {
                                Max_Lat = _MapPoint_Obj.Latitude;
                            }

                            if (_MapPoint_Obj.Longitude < Min_Log)
                            {
                                Min_Log = _MapPoint_Obj.Longitude;
                            }

                            if (_MapPoint_Obj.Latitude < Min_Lat)
                            {
                                Min_Lat = _MapPoint_Obj.Latitude;
                            }
                        }
                    }
                }

                SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position First_Point = new SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position();
                SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position Last_Point = new SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position();
                First_Point.Longitude = Max_Log;
                First_Point.Latitude = Max_Lat;

                Last_Point.Longitude = Min_Log;
                Last_Point.Latitude = Min_Lat;

                _Esri_MAP_Control.Set_View(First_Point, Last_Point);
            }
            catch (Exception ex)
            {
                //Alarm_Auditing.Error(ex.Message);
            }
        }
        public void Delete_Map_Layer(string Map_Layer_Name)
        {
            try
            {
                _Esri_MAP_Control.Delete_Map_Layer(Map_Layer_Name);
            }
            catch (Exception ex)
            {
                //Alarm_Auditing.Error(ex.Message);
            }

        }


        #region Draw Zones

        public List<SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position> _Zone_Points;

        public void Add_Zone(string Zone_Boundres, string Zone_Color)
        {
            try
            {
                List<SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position> Zone_Bounders = new List<SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position>();
                string[] Zone_Boundes_Array = Zone_Boundres.Split(';');
                for (int j = 0; j < Zone_Boundes_Array.Length; j++)
                {
                    string[] _Lat_Lon = Zone_Boundes_Array[j].Split(',');
                    if (_Lat_Lon.Length == 2)
                    {
                        SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position _MapPoint_Obj = new SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position();
                        _MapPoint_Obj.Latitude = double.Parse(_Lat_Lon[0]);
                        _MapPoint_Obj.Longitude = double.Parse(_Lat_Lon[1]);
                        Zone_Bounders.Add(_MapPoint_Obj);
                    }
                }

                string[] Zone_Color_Info = Zone_Color.Split(',');
                if (Zone_Color_Info.Length == 4)
                {
                    _Esri_MAP_Control.Add_Zone(Zone_Bounders, byte.Parse(Zone_Color_Info[0]), byte.Parse(Zone_Color_Info[1]), byte.Parse(Zone_Color_Info[2]), byte.Parse(Zone_Color_Info[3]));
                }
            }
            catch (Exception ex)
            {
                 //Alarm_Auditing.Error(ex.Message);
            }
        }

        public void Draw_All_Zones()
        {
            try
            {
                _Esri_MAP_Control.Draw_All_Zones();
            }
            catch (Exception ex)
            {
                 //Alarm_Auditing.Error(ex.Message);
            }
        }

        public void Delete_All_Zones()
        {
            try
            {
                _Esri_MAP_Control.Delete_All_Zones();
            }
            catch (Exception ex)
            {
                 //Alarm_Auditing.Error(ex.Message);
            }
        }
        #endregion


        public void Fly_To(double Lat, double Lon, int Zoom)
        {
            try
            {
                if (_Esri_MAP_Control != null)
                {
                    SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position Map_Point_Obj = new SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control.MapPoint_Position();
                    Map_Point_Obj.Latitude = Lat;
                    Map_Point_Obj.Longitude = Lon;
                    _Esri_MAP_Control.Fly_To(Map_Point_Obj, Zoom);
                }
            }
            catch (Exception ex)
            {
                 //Alarm_Auditing.Error(ex.Message);
            }
        }
        private void _Esri_MAP_Control_Send_TappedMapPoint_Info(object sender, TappedReturnEventArgs args, int option)
        {
            if (ev_Tapped_Building_AlarmUnit != null)
            {
                ev_Tapped_Building_AlarmUnit(sender, args, option);
            }
            //throw new NotImplementedException();
        }

        public void Add_User_MapPointSimple(string Layer_Name, string Graphic_Name, double Lat, double Log, double mapPoint_Id)
        {
            try
            {
                _Esri_MAP_Control.Add_User_MapPoints(Layer_Name, new List<User_Simple_MapPoint>(), new List<double>());

                User_Simple_MapPoint Map_Point_Obj = new User_Simple_MapPoint();
                Map_Point_Obj.User_Mappoint_Text = Graphic_Name;
                Map_Point_Obj.Font_Color = System.Windows.Media.Color.FromRgb(0, 0, 0);
                Map_Point_Obj.Font_Family = "Microsoft Sans Serif";
                Map_Point_Obj.Font_Size = 12;
                Map_Point_Obj.MapPoint_Simple_Style = 1;
                Map_Point_Obj.SimpleMarkerSymbol_Size = 100;
                Map_Point_Obj.MapPoint_Simple_Color = System.Windows.Media.Color.FromArgb(1, 0, 0, 255);
                Map_Point_Obj.User_Mappoint_Postion.Latitude = Lat;
                Map_Point_Obj.User_Mappoint_Postion.Longitude = Log;
                Map_Point_Obj.Text_XOffset = 10;
                Map_Point_Obj.Text_YOffset = -30;
                //_Esri_MAP_Control.Add_User_MapPoint(Layer_Name, Map_Point_Obj);

                _Esri_MAP_Control.Add_User_MapPoint(Layer_Name, Map_Point_Obj, mapPoint_Id);
            }
            catch (Exception ex)
            {
                 //Alarm_Auditing.Error(ex.Message);
            }
        }
        public void Add_User_MapPointPic(string Layer_Name, string Pic_Url, double Lat, double Log, double mapPoint_Id)
        {
            try
            {
                _Esri_MAP_Control.Add_User_MapPoints(Layer_Name, new List<User_Simple_MapPoint>(), new List<double>());
                User_Picture_MapPoint Picture_Point_Obj = new User_Picture_MapPoint();

                Picture_Point_Obj.User_Mappoint_Postion.Latitude = Lat;
                Picture_Point_Obj.User_Mappoint_Postion.Longitude = Log;
                Picture_Point_Obj.Uri = Pic_Url;
                //_Esri_MAP_Control.Add_User_MapPoint(Layer_Name, Picture_Point_Obj);

                List<User_Picture_MapPoint> Layer_P_Placemarks = new List<User_Picture_MapPoint>();
                Layer_P_Placemarks.Add(Picture_Point_Obj);
                List<double> PlacemarkIds = new List<double>();
                PlacemarkIds.Add(mapPoint_Id);
                _Esri_MAP_Control.Add_User_MapPoints(Layer_Name, Layer_P_Placemarks, PlacemarkIds);
            }
            catch (Exception ex)
            {
                 //Alarm_Auditing.Error(ex.Message);
            }
        }
        
        public void Clear_GraphicOverlay_Layer(string GraphicOverlay_Name)
        {
            try
            {
                _Esri_MAP_Control.Clear_GraphicOverlay_Layer(GraphicOverlay_Name);
            }
            catch (Exception ex)
            {
                 //Alarm_Auditing.Error(ex.Message);
            }
        }

        public void Invisable_GraphicOverlay_Layer(string GraphicOverlay_Name)
        {
            try
            {
                _Esri_MAP_Control.Invisable_GraphicOverlay_Layer(GraphicOverlay_Name);
            }
            catch (Exception ex)
            {
                 //Alarm_Auditing.Error(ex.Message);
            }

        }
        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        public bool draw_single_colored_point(string layername, User_Simple_MapPoint point, int id)
        {
           bool check = _Esri_MAP_Control.Add_User_MapPoint(layername, point, id);
            if(check)
            {
                return true;
            }else
            {
                return false;
            }
        }
        public bool draw_multiple_color_points(string layername, List<User_Simple_MapPoint> points, List<double> placemark_IDs)
        {
            bool check =_Esri_MAP_Control.Add_User_MapPoints(layername, points, placemark_IDs);
            if (check)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void draw_polygon(List<MapPoint_Position> mappoints , byte R, byte G, byte B, byte T = 80)
        {
            _Esri_MAP_Control.Add_Zone(mappoints, T, R, G, B);
            _Esri_MAP_Control.Draw_All_Zones();

        }




    }
}
