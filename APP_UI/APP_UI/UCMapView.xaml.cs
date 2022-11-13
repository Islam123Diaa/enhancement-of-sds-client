using Esri.ArcGISRuntime.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Threading;
using System.IO;
using System.Threading;
using Esri.ArcGISRuntime.ArcGISServices;
using System.ComponentModel;
using Esri.ArcGISRuntime.Data;
using System.Runtime.CompilerServices;
//using Alarm_ESRI_Control;
using SYSTEL_ESRI_Control;




namespace TETRA_Coverage_Monitor
{
    /// <summary>
    /// Interaction logic for UCMapView.xaml
    /// </summary>
    public partial class UCMapView : UserControl
    {
        public GraphicsOverlay _graphicsOverlay;

        public UCMapView()
        {
            try
            {
                InitializeComponent();
                _graphicsOverlay = MainMapView.GraphicsOverlays["online"];
                loadmap();
            }
            catch (Exception ex)
            {

            }

        }

        private void loadmap()
        {
         
            string path = "http://192.168.1.100:6080/arcgis/rest/services/BaseMap/MOI_Vector2020/MapServer";
            string LayerName = "online";
            MainMapView.Map.Layers.Insert(MainMapView.Map.Layers.Count, new ArcGISTiledMapServiceLayer() { ServiceUri = path, DisplayName = LayerName });

        }


        #region graphiccollections


        List<Gcollection> TheCollection = new List<Gcollection>();
        public class Gcollection
        {
            public GraphicCollection collection;
            public string GraphicLayerName;

        }

        public bool CheckGraphcollection(string GraphicOverlay_Layer_Name)
        {
            try
            {
                bool Find_Flag = false;

                MainMapView.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    foreach (Gcollection collection in TheCollection)
                    {
                        if (GraphicOverlay_Layer_Name == collection.GraphicLayerName)
                        {
                            Find_Flag = true;
                            break;
                        }
                    }
                }));
                return Find_Flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private Gcollection CreateNewGcollection(string GraphicOverlay_Layer_Name)
        {
            var newcollection = new Gcollection() { GraphicLayerName = GraphicOverlay_Layer_Name };
            TheCollection.Add(newcollection);
            return newcollection;

        }
        public Gcollection getcollection(string GraphicOverlay_Layer_Name)
        {
            Gcollection result = TheCollection.Find(y => y.GraphicLayerName == GraphicOverlay_Layer_Name);
            return result;
        }
        #endregion

        #region map layer managment
        /// <summary>
        /// get maplayer index by searching with its name
        /// </summary>
        /// <param name="Layer_Name"></param>
        /// <returns></returns>
        public int GetLayerIndex(string Layer_Name)
        {
            try
            {
                for (int i = 0; i < MainMapView.Map.Layers.Count; i++)
                {
                    if (MainMapView.Map.Layers[i].DisplayName == Layer_Name)
                    {
                        return i;
                    }
                }
                return -1;
            }
            catch
            {
                return -1;
            }


        }
        /// <summary>
        /// get maplayer name by searching with its index (order in mapview)
        /// </summary>
        /// <param name="layerindex"></param>
        /// <returns></returns>
        public string GetLayerName(int layerindex)
        {
            if (MainMapView.Map.Layers.Count > 0)
            {
                return MainMapView.Map.Layers[layerindex].DisplayName;
            }
            return null;
        }
        /// <summary>
        /// delete a maplayer and get it by  the map name
        /// </summary>
        /// <param name="layername"></param>
        /// <returns></returns>
        public bool DeleteMapLayer(string layername)
        {

            int index = GetLayerIndex(layername);
            if (index != -1)
            {
                //found map layer
                MainMapView.Map.Layers.RemoveAt(index);
                return true;
            }
            else
            {
                //no map layer found
                return false;
            }
        }
        /// <summary>
        /// delete the last map layer (one appearing infront of user)
        /// </summary>
        /// <returns></returns>
        public bool DeleteLastMapLayer()
        {
            if (MainMapView.Map.Layers.Count > 0)
            {//found map layer
                MainMapView.Map.Layers.RemoveAt(MainMapView.Map.Layers.Count - 1);
                return true;
            }
            return false;
        }
        /// <summary>
        /// delete map layer by its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool DeleteMapLayer(int index)
        {
            if (index != -1)
            {
                MainMapView.Map.Layers.RemoveAt(index);
                return true;
            }
            return false;
        }
        //delete all maps
        public bool DeleteAllMaps()
        {
            for (int i = 0; i < MainMapView.Map.Layers.Count; i++)
            {
                MainMapView.Map.Layers.RemoveAt(i);
                return true;

            }
            return false;
        }
        /// <summary>
        /// add maplayer using string path 
        /// </summary>
        /// <param name="isOnline"></param>
        /// <param name="path"></param>
        /// <param name="LayerName"></param>
        public void AddMapLayer(bool isOnline, string path, string LayerName)
        {
            int index = GetLayerIndex(LayerName);
            if (isOnline)
            {
                //add  new online map
                if (index == -1)
                {
                    MainMapView.Map.Layers.Insert(MainMapView.Map.Layers.Count, new ArcGISTiledMapServiceLayer() { ServiceUri = path, DisplayName = LayerName });

                }
                else
                { // replace an existing layer

                    // remove layer with the same name
                    MainMapView.Map.Layers.RemoveAt(index);
                    // add the layer instead
                    MainMapView.Map.Layers.Insert(MainMapView.Map.Layers.Count, new ArcGISTiledMapServiceLayer() { ServiceUri = path, DisplayName = LayerName });

                }
            }
            else
            {
                //add offline map
                if (index == -1)
                {
                    ArcGISLocalTiledLayer Tile_Map_Layer = new ArcGISLocalTiledLayer(path);
                    Tile_Map_Layer.DisplayName = LayerName;
                    MainMapView.Map.Layers.Insert(MainMapView.Map.Layers.Count, Tile_Map_Layer);
                }
                else
                {
                    MainMapView.Map.Layers.RemoveAt(index);
                    ArcGISLocalTiledLayer Tile_Map_Layer = new ArcGISLocalTiledLayer(path);
                    Tile_Map_Layer.DisplayName = LayerName;
                    MainMapView.Map.Layers.Insert(MainMapView.Map.Layers.Count, Tile_Map_Layer);
                }
                {

                }

            }
        }

        public bool clearlayer(string layername)
        {
            foreach (Gcollection collection in TheCollection)
            {
                if (collection.GraphicLayerName == layername)
                {
                    collection.collection.Clear();
                }

            }

            return true;
        }
        /// <summary>
        /// clear all graphicsfrom layer
        /// </summary>
        /// <param name="layername"></param>
        /// <returns></returns>
        /// 
        public bool Check_graphics_overlay(string GraphicOverlay_Layer_Name)
        {
            try
            {
                bool Find_Flag = false;

                MainMapView.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    foreach (GraphicsOverlay graphicslayer in MainMapView.GraphicsOverlays)
                    {
                        if (GraphicOverlay_Layer_Name == graphicslayer.ID.ToString())
                        {
                            Find_Flag = true;
                            break;
                        }
                    }

                    if (Find_Flag == false)
                    {
                        MainMapView.GraphicsOverlays.Add(new GraphicsOverlay() { ID = GraphicOverlay_Layer_Name });
                        GraphicsOverlay myGraphicsOverlayLayer = MainMapView.GraphicsOverlays[GraphicOverlay_Layer_Name];
                    }

                    Find_Flag = true;

                }));
                return Find_Flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        #endregion

        #region Structures

        public struct MapPoint_Position
        {
            public double Longitude;
            public double Latitude;
        }

        public struct User_Simple_MapPoint
        {
            public string User_Mappoint_Text;
            public MapPoint_Position User_Mappoint_Postion;
            public int SimpleMarkerSymbol_Size;
            public System.Windows.Media.Color MapPoint_Simple_Color;
            public int MapPoint_Simple_Style; //Circle= 0, Cross= 1, Diamond= 2, Square= 3, X= 4, Triangle= 5 
            public string Font_Family;
            public int Font_Size;
            public System.Windows.Media.Color Font_Color;
            public int Text_YOffset;
            public int Text_XOffset;
            public bool User_Mappoint_Visibility;
            public string info;
        }

        public struct User_Picture_MapPoint
        {
            public MapPoint_Position User_Mappoint_Postion;
            public string Uri;
            public bool User_Mappoint_Visibility;
            public int SimpleMarkerSymbol_Size;
            public System.Windows.Media.Color MapPoint_Simple_Color;
            public int MapPoint_Simple_Style; //Circle= 0, Cross= 1, Diamond= 2, Square= 3, X= 4, Triangle= 5
            public string User_Mappoint_Text;
            public string Font_Family;
            public int Font_Size;
            public System.Windows.Media.Color Font_Color;
            public int Text_YOffset;
            public int Text_XOffset;
            public string info;

        }
        #endregion
        public CompositeSymbol AddColoredSymbol(CompositeSymbol pointSymbol_new)
        {
            pointSymbol_new.Symbols.Add(new SimpleMarkerSymbol()
            {
                Style = 0 ,
                Size = 33 , 
                Color = Colors.LightBlue 
            });

            return pointSymbol_new;
        }
        public CompositeSymbol AddTextSymbol(CompositeSymbol pointSymbol_new , string text)
        {
            pointSymbol_new.Symbols.Add(new TextSymbol()
            {
                Text = text ,
                VerticalTextAlignment = VerticalTextAlignment.Top,
                HorizontalTextAlignment = HorizontalTextAlignment.Right,
                IsRightToLeft = false,
                Color = Colors.Black,
                XOffset = 15
            });

            return pointSymbol_new;
        }
        public async System.Threading.Tasks.Task<bool> Draw_Site_Point(float lattitude, float longtitude, string title)
        {
            string layerName = "online";
            try
            {
                if (CheckGraphcollection(layerName))
                {
                    var gcollection = getcollection(layerName);
                    string picture_path = "https://www.pngegg.com/en/png-ioxux";
                    var symbol = new CompositeSymbol();
                    var picsymbol = new PictureMarkerSymbol() { Width = 35, Height = 35, YOffset = 20 };
                    await picsymbol.SetSourceAsync(new Uri(picture_path));
                    AddColoredSymbol(symbol);

                    symbol.Symbols.Add(picsymbol);

                    AddTextSymbol(symbol, title);
                    MapPoint p = new MapPoint(longtitude, lattitude, SpatialReferences.Wgs84);
                    var gra = new Graphic(p, symbol);
                    gcollection.collection.Add(gra);
                    gra.Attributes.Add("Name", title);

                    await Dispatcher.BeginInvoke(new ThreadStart(() => MainMapView.GraphicsOverlays["online"].GraphicsSource = gcollection.collection));
                    return true;
                }
                else
                {
                    var gcollection = CreateNewGcollection(layerName);
                    gcollection.collection = new GraphicCollection();
                    string picture_path = "https://www.pngegg.com/en/png-ioxux";

                    var symbol = new CompositeSymbol();
                    var picsymbol = new PictureMarkerSymbol() { Width = 35, Height = 35, YOffset = 20 };
                    await picsymbol.SetSourceAsync(new Uri(picture_path));
                    AddColoredSymbol(symbol);

                    symbol.Symbols.Add(picsymbol);

                    AddTextSymbol(symbol, title);
                    MapPoint p = new MapPoint(longtitude, lattitude, SpatialReferences.Wgs84);
                    var gra = new Graphic(p, symbol);
                    gcollection.collection.Add(gra);
                    gra.Attributes.Add("Name", title);

                    await Dispatcher.BeginInvoke(new ThreadStart(() => MainMapView.GraphicsOverlays["online"].GraphicsSource = gcollection.collection));
                    return true;
                }
                return false;
            }
            catch
            {
                return false;

            }

        }
        public async System.Threading.Tasks.Task<bool> Draw_Radio_Point( float lattitude , float longtitude , string title )
        {
            string layerName = "online";
            try
            {
                if (Check_graphics_overlay(layerName))
                {
                    if (CheckGraphcollection(layerName))
                    {
                        var gcollection = getcollection(layerName);

                        string picture_path = "https://th.bing.com/th/id/OIP.MdmtSdaEO5678ak5_YfpbAHaNb?pid=ImgDet&rs=1";
                        //string picture_path = "https://pngimg.com/uploads/building/building_PNG87.png";
                        var symbol = new CompositeSymbol();
                        var picsymbol = new PictureMarkerSymbol() { Width = 35, Height = 35, YOffset = 20 };
                        await picsymbol.SetSourceAsync(new Uri(picture_path));
                        AddColoredSymbol(symbol);

                        symbol.Symbols.Add(picsymbol);

                        AddTextSymbol(symbol, title);
                        MapPoint p = new MapPoint(longtitude, lattitude, SpatialReferences.Wgs84);
                        var gra = new Graphic(p, symbol);
                        gcollection.collection.Add(gra);
                        gra.Attributes.Add("Name", title);

                        //MainMapView.GraphicsOverlays["online"].Graphics.Add(gra);
                        //MainMapView.GraphicsOverlays["online"].GraphicsSource = gcollection.collection;

                        await Dispatcher.BeginInvoke(new ThreadStart(() => MainMapView.GraphicsOverlays["online"].GraphicsSource = gcollection.collection));
                        return true;
                    }
                    else
                    {
                        var gcollection = CreateNewGcollection(layerName);
                        gcollection.collection = new GraphicCollection();
                        //string picture_path = "https://th.bing.com/th/id/R.d3f28810c6a56b4053f45616161cd805?rik=klTkugUQlatgAg&pid=ImgRaw&r=0";
                        string picture_path = "https://th.bing.com/th/id/OIP.MdmtSdaEO5678ak5_YfpbAHaNb?pid=ImgDet&rs=1";

                        //string picture_path = "https://pngimg.com/uploads/building/building_PNG87.png";

                        var symbol = new CompositeSymbol();
                        var picsymbol = new PictureMarkerSymbol() { Width = 35, Height = 35, YOffset = 20 };
                        await picsymbol.SetSourceAsync(new Uri(picture_path));
                        AddColoredSymbol(symbol);

                        symbol.Symbols.Add(picsymbol);

                        AddTextSymbol(symbol, title);
                        MapPoint p = new MapPoint(longtitude, lattitude, SpatialReferences.Wgs84);
                        var gra = new Graphic(p, symbol);
                        gcollection.collection.Add(gra);
                        gra.Attributes.Add("Name", title);
                        //MainMapView.GraphicsOverlays["online"].GraphicsSource = gcollection.collection;

                        //MainMapView.GraphicsOverlays["online"].Graphics.Add(gra);

                        await Dispatcher.BeginInvoke(new ThreadStart(() => MainMapView.GraphicsOverlays["online"].GraphicsSource = gcollection.collection));
                        return true;
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;

            }

        }
        public async Task draw_radio_on_map()
            {
            try
            {
                var latt = (float)30.033333;
                var longt = (float)31.233334;
                string title = "radio";
                await Draw_Radio_Point(latt, longt, title);
                

            }
            catch (Exception ex)
            {

            }
        }

        #region Map_Tapped

        int Resolution = 0;
        public delegate void TappedEventHandler(object sender, TappedReturnEventArgs args, int option);
        public event TappedEventHandler Send_TappedMapPoint_Info;
        private async void MainMapView_MapViewTapped(object sender, MapViewInputEventArgs e)
        {
            try
            {
                TappedReturnEventArgs retvals = null;

                GraphicsOverlayCollection GraphicsOverlays_Collection = MainMapView.GraphicsOverlays;
                if (GraphicsOverlays_Collection.Count > 0)
                { if (retvals == null)
                    {
                        var Graphic_Obj2 = await GraphicsOverlays_Collection[Code.Singleton.Site_layer].HitTestAsync(MainMapView, e.Position);

                        if (Graphic_Obj2 != null && Graphic_Obj2.Symbol != null  && (Graphic_Obj2.Symbol as CompositeSymbol) != null && (Graphic_Obj2.Symbol as CompositeSymbol).Symbols.Count > 0)
                        {

                            string ID = Graphic_Obj2.Attributes["ID"].ToString();
                            if (ID != "")
                            {
                                var mapPointGeo2 = GeometryEngine.Project(e.Location, SpatialReferences.Wgs84) as MapPoint;
                                retvals = new TappedReturnEventArgs(ID, GraphicsOverlays_Collection[Code.Singleton.Site_layer].ID, e.Position.X, e.Position.Y, mapPointGeo2.Y, mapPointGeo2.X);
                                Send_TappedMapPoint_Info(this, retvals, 1);
                            }
                        }
                    }

                    if (retvals == null)
                    {
                        for (int i = 0; i < GraphicsOverlays_Collection.Count; i++)
                        {
                            var Graphic_Obj_3 = await GraphicsOverlays_Collection[i].HitTestAsync(MainMapView, e.Position);

                            if (Graphic_Obj_3 != null && Graphic_Obj_3.Symbol != null && Graphic_Obj_3.Attributes["Placemark"] != null && (Graphic_Obj_3.Symbol as CompositeSymbol) != null && (Graphic_Obj_3.Symbol as CompositeSymbol).Symbols.Count > 0)
                            {
                                string Placemark = Graphic_Obj_3.Attributes["Placemark"].ToString();
                                if (Placemark != "")
                                {
                                    var mapPointGeo2 = GeometryEngine.Project(e.Location, SpatialReferences.Wgs84) as MapPoint;
                                    retvals = new TappedReturnEventArgs(Placemark, GraphicsOverlays_Collection[i].ID, e.Position.X, e.Position.Y, mapPointGeo2.Y, mapPointGeo2.X);
                                    Send_TappedMapPoint_Info(this, retvals, 2);
                                }
                            }

                        }
                    }
                }

                if (retvals == null)
                {
                    var mapPointGeo2 = GeometryEngine.Project(e.Location, SpatialReferences.Wgs84) as MapPoint;
                    retvals = new TappedReturnEventArgs("", "", e.Position.X, e.Position.Y, mapPointGeo2.Y, mapPointGeo2.X);
                    Send_TappedMapPoint_Info(this, retvals, -1);
                }
            }
            catch (Exception ex)
            {

            }
        }


        public delegate void DropedEventHandler(string name);
        public event DropedEventHandler Send_DroppedMapPoint_Info;
        public async void MainMapView_MapViewDroped(object sender, DragEventArgs e)
        {
            try
            {
                GraphicsOverlayCollection GraphicsOverlays_Collection = MainMapView.GraphicsOverlays;
                if (GraphicsOverlays_Collection.Count > 0)
                {
                    System.Windows.Point myUIPointLocation = e.GetPosition(MainMapView);
                    var Graphic_Obj = await GraphicsOverlays_Collection["Radios_Layer_1"].HitTestAsync(MainMapView, myUIPointLocation);

                    if (Graphic_Obj != null && Graphic_Obj.Symbol != null && (Graphic_Obj.Symbol as CompositeSymbol) != null && (Graphic_Obj.Symbol as CompositeSymbol).Symbols.Count > 0)
                    {

                        string Radio_ISSI = Graphic_Obj.Attributes["ISSI"].ToString();
                        if (Radio_ISSI != "")
                        {
                            Send_DroppedMapPoint_Info(Radio_ISSI);
                        }
                    }
                    else
                    {
                        Send_DroppedMapPoint_Info("");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public delegate void ExtentChangedEventHandler(double w, double h, double resolution);
        public event ExtentChangedEventHandler Send_MAP_ExtentChanged_Info;
        private void MainMapView_ExtentChanged(object sender, EventArgs e)
        {
            try
            {
                Send_MAP_ExtentChanged_Info(MainMapView.Extent.Width, MainMapView.Extent.Height, Resolution);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}
