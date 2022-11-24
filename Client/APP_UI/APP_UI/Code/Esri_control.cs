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
using static SYSTEL_ESRI_Control.SYSTEL_ESRI_Main_Control;
using static TETRA_Coverage_Monitor.Esri_control;
using TETRA_Coverage_Monitor.Code;

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

        

        #region coverage information
        public struct Coverage_info
        {
            public string Site_Name;
            public int ISSI;
            public float RSSI;
            public int PosID;
            public string dbm_value;
        }
        public List<Coverage_info> Coverage_points_info = new List<Coverage_info>();
        #endregion

        #region drawing data collection
        public struct Drawing_Data
        {
            public System.Drawing.Color picturecolour;
            public System.Windows.Media.Color color;
            public int RSSI;
            public string drawing_layername;
            public List<User_Simple_MapPoint> Graphic_points_collection;
            public List<MapPoint_Position> polygon_points;
            public List<double> placemarks_IDs;
            public string RSSI_to_DBM_value;
        }
        public Drawing_Data[] Drawing_data_collection = new Drawing_Data[32];
        public List<System.Drawing.Color> get_colors()
        {
            List<System.Drawing.Color> colorslist = new List<System.Drawing.Color>();
            foreach(Drawing_Data drawing_layer in Drawing_data_collection)
            {
                colorslist.Add(drawing_layer.picturecolour );
            }
            return colorslist;
        }
        public string convert_rssi_to_dbm_value(int rssi)
        {
            foreach (Drawing_Data Data_collection in Drawing_data_collection)
            {
                if (rssi == Data_collection.RSSI)
                {
                    return Data_collection.RSSI_to_DBM_value;
                }
            }
            if (rssi > 31)
            {
                return Drawing_data_collection[31].RSSI_to_DBM_value;
            }
            else
            {
                return "";
            }
        }


        //
        public void read_from_txt_file()
        {
            
        }

        public void initialize_drawing_data_collection_array()
        {
            try
            {
                #region get colors from txt file
                //string path = @"C:\Users\WADG#2\Desktop\Client\APP_UI\colors.txt";
                string path = AppDomain.CurrentDomain.BaseDirectory + @"Files\colors.txt";
                StreamReader stream = new StreamReader(path);
                string file_data = stream.ReadToEnd();
                int counter = 0;
                foreach (string line in System.IO.File.ReadLines(path))
                {
                    string[] line_no_space = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    Drawing_data_collection[counter].picturecolour = System.Drawing.Color.FromName(line_no_space[0]);
                    Drawing_data_collection[counter].color = System.Windows.Media.Color.FromRgb(Byte.Parse(line_no_space[1]), Byte.Parse(line_no_space[2]), Byte.Parse(line_no_space[3]));
                    counter++;
                }
                #endregion
                #region rssi 0
                Drawing_data_collection[0].drawing_layername = System.Drawing.Color.DimGray.ToString() + "_points";
                Drawing_data_collection[0].RSSI = 0;
                Drawing_data_collection[0].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[0].placemarks_IDs = new List<double>();
                Drawing_data_collection[0].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[0].RSSI_to_DBM_value = "-113 Dbm or less";
                #endregion
                #region rssi 1
                Drawing_data_collection[1].drawing_layername = System.Drawing.Color.White.ToString() + "_points";
                Drawing_data_collection[1].RSSI = 1;
                Drawing_data_collection[1].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[1].placemarks_IDs = new List<double>();
                Drawing_data_collection[1].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[1].RSSI_to_DBM_value = "-111 to -112 dBm";
                #endregion
                #region rssi 2   
                Drawing_data_collection[2].drawing_layername = System.Drawing.Color.LightGray.ToString() + "_points";
                Drawing_data_collection[2].RSSI = 2;
                Drawing_data_collection[2].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[2].placemarks_IDs = new List<double>();
                Drawing_data_collection[2].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[2].RSSI_to_DBM_value = "-110 to -109 dBm";

                #endregion
                #region rssi 3           
                Drawing_data_collection[3].drawing_layername = System.Drawing.Color.LightPink.ToString() + "_points";
                Drawing_data_collection[3].RSSI = 3;
                Drawing_data_collection[3].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[3].placemarks_IDs = new List<double>();
                Drawing_data_collection[3].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[3].RSSI_to_DBM_value = "-108 to -107 dBm";

                #endregion
                #region rssi 4           
                Drawing_data_collection[4].drawing_layername = System.Drawing.Color.Red.ToString() + "_points";
                Drawing_data_collection[4].RSSI = 4;
                Drawing_data_collection[4].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[4].placemarks_IDs = new List<double>();
                Drawing_data_collection[4].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[4].RSSI_to_DBM_value = "-106 to -105 dBm";
                #endregion
                #region rssi 5           
                Drawing_data_collection[5].drawing_layername = System.Drawing.Color.LightSalmon.ToString() + "_points";
                Drawing_data_collection[5].RSSI = 5;
                Drawing_data_collection[5].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[5].placemarks_IDs = new List<double>();
                Drawing_data_collection[5].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[5].RSSI_to_DBM_value = "-104 to -103 dBm";
                #endregion
                #region rssi 6            
                Drawing_data_collection[6].drawing_layername = System.Drawing.Color.Orange.ToString() + "_points";
                Drawing_data_collection[6].RSSI = 6;
                Drawing_data_collection[6].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[6].placemarks_IDs = new List<double>();
                Drawing_data_collection[6].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[6].RSSI_to_DBM_value = "-102 to -101 dBm";
                #endregion
                #region rssi 7        
                Drawing_data_collection[7].drawing_layername = System.Drawing.Color.DarkOrange.ToString() + "_points";
                Drawing_data_collection[7].RSSI = 7;
                Drawing_data_collection[7].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[7].placemarks_IDs = new List<double>();
                Drawing_data_collection[7].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[7].RSSI_to_DBM_value = "-100 to -99 dBm";
                #endregion
                #region rssi 8            
                Drawing_data_collection[8].drawing_layername = System.Drawing.Color.Tan.ToString() + "_points";
                Drawing_data_collection[8].RSSI = 8;
                Drawing_data_collection[8].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[8].placemarks_IDs = new List<double>();
                Drawing_data_collection[8].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[8].RSSI_to_DBM_value = "-98 to -97 dBm";

                #endregion
                #region rssi 9            
                Drawing_data_collection[9].drawing_layername = System.Drawing.Color.Bisque.ToString() + "_points";
                Drawing_data_collection[9].RSSI = 9;
                Drawing_data_collection[9].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[9].placemarks_IDs = new List<double>();
                Drawing_data_collection[9].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[9].RSSI_to_DBM_value = "-96 to -95 dBm";

                #endregion
                #region rssi 10
                Drawing_data_collection[10].drawing_layername = System.Drawing.Color.Cornsilk.ToString() + "_points";
                Drawing_data_collection[10].RSSI = 10;
                Drawing_data_collection[10].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[10].placemarks_IDs = new List<double>();
                Drawing_data_collection[10].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[10].RSSI_to_DBM_value = "-94 to -93 dBm";
                #endregion
                #region rssi 11           
                Drawing_data_collection[11].drawing_layername = System.Drawing.Color.DarkGoldenrod.ToString() + "_points";
                Drawing_data_collection[11].RSSI = 11;
                Drawing_data_collection[11].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[11].placemarks_IDs = new List<double>();
                Drawing_data_collection[11].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[11].RSSI_to_DBM_value = "-92 to -91 dBm";
                #endregion
                #region rssi 12  
                Drawing_data_collection[12].drawing_layername = System.Drawing.Color.LawnGreen.ToString() + "_points";
                Drawing_data_collection[12].RSSI = 12;
                Drawing_data_collection[12].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[12].placemarks_IDs = new List<double>();
                Drawing_data_collection[12].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[12].RSSI_to_DBM_value = "-90 to -89 dBm";

                #endregion
                #region rssi 13            
                Drawing_data_collection[13].drawing_layername = System.Drawing.Color.LimeGreen.ToString() + "_points";
                Drawing_data_collection[13].RSSI = 13;
                Drawing_data_collection[13].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[13].placemarks_IDs = new List<double>();
                Drawing_data_collection[13].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[13].RSSI_to_DBM_value = "-88 to -87 dBm";

                #endregion
                #region rssi 14           
                Drawing_data_collection[14].drawing_layername = System.Drawing.Color.Green.ToString() + "_points";
                Drawing_data_collection[14].RSSI = 14;
                Drawing_data_collection[14].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[14].placemarks_IDs = new List<double>();
                Drawing_data_collection[14].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[14].RSSI_to_DBM_value = "-86 to -85 dBm";

                #endregion
                #region rssi 15            
                Drawing_data_collection[15].drawing_layername = System.Drawing.Color.MediumSeaGreen.ToString() + "_points";
                Drawing_data_collection[15].RSSI = 15;
                Drawing_data_collection[15].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[15].placemarks_IDs = new List<double>();
                Drawing_data_collection[15].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[15].RSSI_to_DBM_value = "-84 to -83 dBm";

                #endregion
                #region rssi 16            
                Drawing_data_collection[16].drawing_layername = System.Drawing.Color.Turquoise.ToString() + "_points";
                Drawing_data_collection[16].RSSI = 16;
                Drawing_data_collection[16].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[16].placemarks_IDs = new List<double>();
                Drawing_data_collection[16].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[16].RSSI_to_DBM_value = "-82 to -81 dBm";

                #endregion
                #region rssi 17        
                Drawing_data_collection[17].drawing_layername = System.Drawing.Color.DeepSkyBlue.ToString() + "_points";
                Drawing_data_collection[17].RSSI = 17;
                Drawing_data_collection[17].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[17].placemarks_IDs = new List<double>();
                Drawing_data_collection[17].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[17].RSSI_to_DBM_value = "-80 to -79 dBm";

                #endregion
                #region rssi 18            
                Drawing_data_collection[18].drawing_layername = System.Drawing.Color.BlueViolet.ToString() + "_points";
                Drawing_data_collection[18].RSSI = 18;
                Drawing_data_collection[18].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[18].placemarks_IDs = new List<double>();
                Drawing_data_collection[18].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[18].RSSI_to_DBM_value = "-78 to -77 dBm";

                #endregion
                #region rssi 19          
                Drawing_data_collection[19].drawing_layername = System.Drawing.Color.Plum.ToString() + "_points";
                Drawing_data_collection[19].RSSI = 19;
                Drawing_data_collection[19].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[19].placemarks_IDs = new List<double>();
                Drawing_data_collection[19].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[19].RSSI_to_DBM_value = "-76 to -75 dBm";

                #endregion
                #region rssi 20
                Drawing_data_collection[20].drawing_layername = System.Drawing.Color.Violet.ToString() + "_points";
                Drawing_data_collection[20].RSSI = 20;
                Drawing_data_collection[20].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[20].placemarks_IDs = new List<double>();
                Drawing_data_collection[20].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[20].RSSI_to_DBM_value = "-74 to -73 dBm";

                #endregion
                #region rssi 21           
                Drawing_data_collection[21].drawing_layername = System.Drawing.Color.Pink.ToString() + "_points";
                Drawing_data_collection[21].RSSI = 21;
                Drawing_data_collection[21].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[21].placemarks_IDs = new List<double>();
                Drawing_data_collection[21].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[21].RSSI_to_DBM_value = "-72 to -71 dBm";

                #endregion
                #region rssi 22     
                Drawing_data_collection[22].drawing_layername = System.Drawing.Color.Yellow.ToString() + "_points";
                Drawing_data_collection[22].RSSI = 22;
                Drawing_data_collection[22].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[22].placemarks_IDs = new List<double>();
                Drawing_data_collection[22].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[22].RSSI_to_DBM_value = "-70 to -69 dBm";

                #endregion
                #region rssi 23            
                Drawing_data_collection[23].drawing_layername = System.Drawing.Color.DarkSalmon.ToString() + "_points";
                Drawing_data_collection[23].RSSI = 23;
                Drawing_data_collection[23].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[23].placemarks_IDs = new List<double>();
                Drawing_data_collection[23].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[23].RSSI_to_DBM_value = "-68 to -67 dBm";

                #endregion
                #region rssi 24           
                Drawing_data_collection[24].drawing_layername = System.Drawing.Color.Blue.ToString() + "_points";
                Drawing_data_collection[24].RSSI = 24;
                Drawing_data_collection[24].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[24].placemarks_IDs = new List<double>();
                Drawing_data_collection[24].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[24].RSSI_to_DBM_value = "-66 to -65 dBm";

                #endregion
                #region rssi 25            
                Drawing_data_collection[25].drawing_layername = System.Drawing.Color.Orchid.ToString() + "_points";
                Drawing_data_collection[25].RSSI = 25;
                Drawing_data_collection[25].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[25].placemarks_IDs = new List<double>();
                Drawing_data_collection[25].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[25].RSSI_to_DBM_value = "-64 to -63 dBm";

                #endregion
                #region rssi 26          
                Drawing_data_collection[26].drawing_layername = System.Drawing.Color.PaleVioletRed.ToString() + "_points";
                Drawing_data_collection[26].RSSI = 26;
                Drawing_data_collection[26].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[26].placemarks_IDs = new List<double>();
                Drawing_data_collection[26].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[26].RSSI_to_DBM_value = "-62 to -61 dBm";

                #endregion
                #region rssi 27      
                Drawing_data_collection[27].drawing_layername = System.Drawing.Color.DarkViolet.ToString() + "_points";
                Drawing_data_collection[27].RSSI = 27;
                Drawing_data_collection[27].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[27].placemarks_IDs = new List<double>();
                Drawing_data_collection[27].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[27].RSSI_to_DBM_value = "-60 to -59 dBm";

                #endregion
                #region rssi 28          
                Drawing_data_collection[28].drawing_layername = System.Drawing.Color.Fuchsia.ToString() + "_points";
                Drawing_data_collection[28].RSSI = 28;
                Drawing_data_collection[28].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[28].placemarks_IDs = new List<double>();
                Drawing_data_collection[28].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[28].RSSI_to_DBM_value = "-58 to -57 dBm";

                #endregion
                #region rssi 29           
                Drawing_data_collection[29].drawing_layername = System.Drawing.Color.Crimson.ToString() + "_points";
                Drawing_data_collection[29].RSSI = 29;
                Drawing_data_collection[29].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[29].placemarks_IDs = new List<double>();
                Drawing_data_collection[29].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[29].RSSI_to_DBM_value = "-56 to -55 dBm";

                #endregion
                #region rssi 30            
                Drawing_data_collection[30].drawing_layername = System.Drawing.Color.MediumPurple.ToString() + "_points";
                Drawing_data_collection[30].RSSI = 30;
                Drawing_data_collection[30].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[30].placemarks_IDs = new List<double>();
                Drawing_data_collection[30].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[30].RSSI_to_DBM_value = "-54 to -53 dBm";

                #endregion
                #region rssi 31          
                Drawing_data_collection[31].drawing_layername = System.Drawing.Color.DarkSlateBlue.ToString() + "_points";
                Drawing_data_collection[31].RSSI = 31;
                Drawing_data_collection[31].Graphic_points_collection = new List<User_Simple_MapPoint>();
                Drawing_data_collection[31].placemarks_IDs = new List<double>();
                Drawing_data_collection[31].polygon_points = new List<MapPoint_Position>();
                Drawing_data_collection[31].RSSI_to_DBM_value = "-52 dBm or more";
                #endregion
            }
            catch (Exception ex)
            {
            }
         
        }
        public List<double> ids = new List<double>();

        public void clear_all_drawing_data_collection_layers()
        {
            Delete_All_Zones();
            foreach (Drawing_Data drawing_layer in Drawing_data_collection)
            {
                Invisable_GraphicOverlay_Layer(drawing_layer.drawing_layername);
                Clear_GraphicOverlay_Layer(drawing_layer.drawing_layername);
                drawing_layer.polygon_points.Clear();
                drawing_layer.placemarks_IDs.Clear();
               drawing_layer.Graphic_points_collection.Clear();
            }
        }
        public void reset_drawing_data_collection_layers()
        {
            #region reset drawing data layers lists
            foreach (Drawing_Data drawing_layer in Drawing_data_collection)
            {
                drawing_layer.polygon_points.Clear();
                drawing_layer.placemarks_IDs.Clear();
                drawing_layer.Graphic_points_collection.Clear();
            }
            #endregion

        }
        #endregion

        #region map layer managment
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

        #endregion

        #region map control
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
        #endregion

        #region clear drawings
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

        #region Draw
        public bool add_multiple_color_points(string layername, List<User_Simple_MapPoint> points, List<double> placemark_IDs)
        {
            bool check =_Esri_MAP_Control.Add_User_MapPoints_no_text(layername, points, placemark_IDs);
            
            if (check)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool add_single_Picture_point(string layername , User_Picture_MapPoint point , double ID)
        {
            bool check =_Esri_MAP_Control.Add_User_MapPoint(layername, point, ID);
            if(check)
            {
                return true;
            }else
            {
                return false;
            }

        }
        public void draw_polygons(List<MapPoint_Position> mappoints , byte R, byte G, byte B, byte T = 80)
        {
            _Esri_MAP_Control.Add_Zone(mappoints, T, R, G, B);
            _Esri_MAP_Control.Draw_All_Zones();

        }
        public bool Draw_layer_with_clustered_form_points(string layername , List<User_Simple_MapPoint> map_points , List<double> ID_List)
        {
            bool check =_Esri_MAP_Control.Add_Radios_GraphicOverlay_Layer(layername, map_points, ID_List);
            if (check)
            {
                return true;
            }else
            {
                return false;
            }

        }
        public bool Draw_layer_with_clustered_form_points(string layername , List<User_Picture_MapPoint>Picture_map_points,List<double>ID_list)
        {
            bool check = _Esri_MAP_Control.Add_Radios_GraphicOverlay_Layer(layername, Picture_map_points, ID_list);
            if(check)
            {
                return true;
            }else
            {
                return false;
            }

        }

        #region new drawing methods
        public void draw_all_Sites(List<float> lattitude_list, List<float> longtitude_list, List<string> sitename_list, List<int> site_id_list)
        {
            List<User_Picture_MapPoint> Picture_map_points = new List<User_Picture_MapPoint>();
            List<User_Simple_MapPoint> text_map_points = new List<User_Simple_MapPoint>();
            List<double> picture_points_IDs = new List<double>();
            List<double> text_points_IDS = new List<double>();
            for (int i = 0; i < site_id_list.Count; i++)
            {
                User_Picture_MapPoint point = create_Site_pic_point(lattitude_list[i], longtitude_list[i]);
                if (point.User_Mappoint_Postion.Latitude != 0 && point.User_Mappoint_Postion.Longitude != 0)
                {
                    Picture_map_points.Add(point);
                    picture_points_IDs.Add(site_id_list[i]);
                }
                User_Simple_MapPoint text_point = create_Site_text_point(lattitude_list[i], longtitude_list[i], sitename_list[i]);
                if (text_point.User_Mappoint_Postion.Latitude != 0 && text_point.User_Mappoint_Postion.Longitude != 0)
                {
                    text_map_points.Add(text_point);
                    text_points_IDS.Add(site_id_list[i]);
                }
                //text
                Draw_layer_with_clustered_form_points(Code.Singleton.Radio_layer, text_map_points, text_points_IDS);
                //pictures
                Draw_layer_with_clustered_form_points(Code.Singleton.Site_layer, Picture_map_points, picture_points_IDs);
            }
        }
        public User_Picture_MapPoint create_Site_pic_point(float lattitude, float longtitude)
        {
            User_Picture_MapPoint point = new User_Picture_MapPoint();
            point.Uri = (Code.Singleton.site_picture_uri);
            point.User_Mappoint_Postion.Latitude = lattitude;
            point.User_Mappoint_Postion.Longitude = longtitude;
            point.User_Mappoint_Visibility = true;
            return point;
        }
        public User_Simple_MapPoint create_Site_text_point(float lattitude, float longtitude, string sitename)
        {
            User_Simple_MapPoint point_noPic = new User_Simple_MapPoint();
            point_noPic.User_Mappoint_Text = sitename;
            point_noPic.Font_Size = 8;
            point_noPic.User_Mappoint_Postion.Latitude = lattitude;
            point_noPic.User_Mappoint_Postion.Longitude = longtitude;
            point_noPic.User_Mappoint_Visibility = false;
            point_noPic.Font_Color = System.Windows.Media.Colors.Blue;
            point_noPic.Font_Family = "Arial";
            point_noPic.MapPoint_Simple_Color = System.Windows.Media.Colors.Red;
            point_noPic.MapPoint_Simple_Style = 0;
            point_noPic.SimpleMarkerSymbol_Size = 0;
            point_noPic.Text_XOffset = -1;
            point_noPic.Text_YOffset = -20;
            return point_noPic;

        }
        public User_Picture_MapPoint create_radio_pic_point(double lattitude, double longtitude)
        {
            string Pic_uri = Code.Singleton.radio_picture_uri;
            User_Picture_MapPoint radio_point = new User_Picture_MapPoint();
            radio_point.Uri = Pic_uri;
            MapPoint_Position radio_position = new MapPoint_Position();
            radio_position.Latitude = lattitude;
            radio_position.Longitude = longtitude;
            radio_point.User_Mappoint_Postion = radio_position;
            radio_point.User_Mappoint_Visibility = true;
            return radio_point;
        }
        public void draw_Polygons_for_one_site(List<float> RSSI_list, List<double> lattitude_list, List<double> longtitude_list)
        {
            #region add_polygon points to each drawing layer
            for (int i = 0; i < RSSI_list.Count; i++)
            {
                for (int j = 0; j < Drawing_data_collection.Length; j++)
                {
                    if (Drawing_data_collection[j].RSSI == RSSI_list[i])
                    {
                        MapPoint_Position point = new MapPoint_Position();
                        point.Longitude = longtitude_list[i];
                        point.Latitude = lattitude_list[i];
                        Drawing_data_collection[j].polygon_points.Add(point);
                        break;
                    }
                }
            }
            #endregion

            #region draw the layers

            foreach (Drawing_Data drawing_layer in Drawing_data_collection)
            {
                if (drawing_layer.polygon_points != null && drawing_layer.polygon_points.Count > 2)
                {
                    draw_polygons(drawing_layer.polygon_points, drawing_layer.color.R, drawing_layer.color.G, drawing_layer.color.B, 80);
                    break;
                }
            }
            #endregion

        }
        public void draw_points_for_one_site(List<int> Pos_id_list, List<float> RSSI_list, List<double> lattitude_list, List<double> longtitude_list, List<int> site_id_list, List<int> ISSI_list, List<string> Sitename_list)
        {
            for (int i = 0; i < RSSI_list.Count; i++)
            {
                for (int j = 0; j < Drawing_data_collection.Length; j++)
                {
                    if (Drawing_data_collection[j].RSSI == RSSI_list[i])
                    {
                        #region add point to the drawing layer
                        MapPoint_Position map_point = new MapPoint_Position();
                        User_Simple_MapPoint point = new User_Simple_MapPoint();
                        map_point.Longitude = longtitude_list[i];
                        map_point.Latitude = lattitude_list[i];
                        point.MapPoint_Simple_Color = Drawing_data_collection[j].color;
                        point.SimpleMarkerSymbol_Size = 20;
                        point.User_Mappoint_Postion = map_point;
                        point.User_Mappoint_Visibility = true;
                        point.MapPoint_Simple_Style = 0; //circle
                        Drawing_data_collection[j].Graphic_points_collection.Add(point);
                        double id = Pos_id_list[i];
                        Drawing_data_collection[j].placemarks_IDs.Add(id);
                        #endregion

                        #region add coverage information
                        Coverage_info coverage_obj = new Coverage_info();
                        coverage_obj.RSSI = RSSI_list[i];
                        coverage_obj.dbm_value = Drawing_data_collection[j].RSSI_to_DBM_value;
                        coverage_obj.PosID = Pos_id_list[i];
                        coverage_obj.Site_Name = Sitename_list[i];
                        coverage_obj.ISSI = ISSI_list[i];
                        Coverage_points_info.Add(coverage_obj);
                        #endregion
                        break;
                    }
                }
            }
            #region draw the layers
            foreach (Drawing_Data draw_layer in Drawing_data_collection)
            {
                if (draw_layer.Graphic_points_collection != null && draw_layer.Graphic_points_collection.Count > 0)
                {
                    add_multiple_color_points(draw_layer.drawing_layername, draw_layer.Graphic_points_collection, draw_layer.placemarks_IDs);
                }
            }
            #endregion
        }

        #endregion
        #endregion

    }
}
