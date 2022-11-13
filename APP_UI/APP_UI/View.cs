using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using TETRA_Coverage_Monitor.Code;

namespace TETRA_Coverage_Monitor
{
    public partial class View : Form
    {
        SDS_Remote_Control_WS Web_service_obj = new SDS_Remote_Control_WS();
        UCMapView Map_API = new UCMapView();
        Web_service_Control WS_Control_obj = new Web_service_Control();
        public bool Intentional { get; private set; }
        public bool workdone { get; set; }

        Thread Main_Thread;

        public View()
        {
            try
            {
                InitializeComponent();

                Auditing.InitializeCreatingAuditAndErrorFile();

                //check privilge here
                check_privilage();
                circularProgressBar_radio_panel.Value = 0;
                circularProgressBar_site_panel.Value = 0;
                circularProgressBar_zone_panel.Value = 0;
                drawing_progress_bar_date_pnl.Value = 0;

                #region new drawing data
                initialize_drawing_data_collection_array();
                initialize_picture_boxes_colors();
                #endregion
                #region old drawing region
                load_color_pics();
                load_dbm_values();
                initialize_drawing_zones();
                #endregion
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }

        }

        #region Main Functions

        #region Form Events
        bool first_load = false;

        private void View_Load(object sender, EventArgs e)
        {
            try
            {
                workdone = false;

                #region Initialize esri map
                //initialize map 
                elementHost_Mapview.Child = Esri_control.Load_Esri_Control();
                Esri_control.ev_Tapped_Building_AlarmUnit += Main_MAP_Object_Click;
                string path = "http://192.168.1.100:6080/arcgis/rest/services/BaseMap/MOI_Vector2020/MapServer";
                Esri_control.Add_Map_Layer(true, Code.Singleton.Radio_layer, path);
                #endregion
                //main load loop
                Main_Thread = new Thread(load_all_treeviews_data_cycle);
                Main_Thread.Start();

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        private void View_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Environment.Exit(Environment.ExitCode);

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #endregion

        public void check_privilage()
        {

            string user_privilage = Code.Singleton.user_obj.Privilege;

            string[] privilages = new string[24];
            for (int i = 0; i < user_privilage.Length; i++)
            {
                privilages[i] = user_privilage.Substring(i, 1);
            }
            #region check each privilage 
            if (privilages[0] != "1")
            {
                //not admin
            }
            if (privilages[1] != "1")
            {
                //no loading zones
            }
            if (privilages[2] != "1")
            {
                //not showing zones
            }
            if (privilages[3] != "1")
            {
                //not adding zones
                btn_Add_Zone_Zones_Tab.Visible = false;
            }
            if (privilages[4] != "1")
            {
                //not editing zones
                btn_Edit_Zone_Zones_Tab.Visible = false;
            }
            if (privilages[17] != "1")
            {
                //not deleteing zones
                btn_Delete_Zone_Zones_Tab.Visible = false;
            }
            if (privilages[6] != "1")
            {
                //not searching zones
                btn_Search_Zone_Zones_Tab.Visible = false;
            }
            if (privilages[7] != "1")
            {
                //not loading sites
            }
            if (privilages[8] != "1")
            {
                //not showing sites
            }
            if (privilages[9] != "1")
            {
                //not adding sites
                btn_Add_Site_Sites_Tab.Visible = false;
            }
            if (privilages[10] != "1")
            {
                //not editing sites
                btn_Edit_Site_Sites_Tab.Visible = false;
            }
            if (privilages[11] != "1")
            {
                //not deleting sites
                btn_Delete_Site_Sites_Tab.Visible = false;
            }
            if (privilages[12] != "1")
            {
                //not searching sites
                btn_Search_Site_Sites_Tab.Visible = false;
            }
            if (privilages[13] != "1")
            {
                //not loading radios
            }
            if (privilages[14] != "1")
            {
                //not showing radios
            }
            if (privilages[117] != "1")
            {
                //not adding radios
                btn_Add_Radio_Radios_Tab.Visible = false;
            }
            if (privilages[16] != "1")
            {
                //not editing radios
                btn_Edit_Radio_Radios_Tab.Visible = false;
            }
            if (privilages[17] != "1")
            {
                //not deleting radios
                btn_Delete_Radio_Radios_Tab.Visible = false;
            }
            if (privilages[18] != "1")
            {
                //not searching radios
                btn_Search_Radio_Radios_Tab.Visible = false;
            }
            if (privilages[19] != "1")
            {
                //not turnning off radios
                SB_btn_Turnoff_Radios_Tab.Visible = false;
            }
            if (privilages[20] != "1")
            {
                //not changing volume
                SB_Volume_CB_Radios_Tab.Visible = false;
            }
            if (privilages[21] != "1")
            {
                //not changing tg
                SB_TalkGroups_CB_Radios_Tab.Visible = false;
            }
            if (privilages[22] != "1")
            {
                //not requesting gps
                SB_btn_Request_radio_location_Radios_Tab.Visible = false;
            }
            if (privilages[23] != "1")
            {
                //not viewing coverage
                //showCoverageToolStripMenuItem.Visible = false;
                //button1.Visible = false;
            }
            #endregion
        }

        public void load_all_treeviews_data_cycle()
        {
            try
            {
                while (true)
                {
                    zones = WS_Control_obj.Select_all_zones(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    sites = WS_Control_obj.Select_all_sites(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    radios = WS_Control_obj.Select_all_radios(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    Cities = WS_Control_obj.Select_All_cities(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);

                    load_trv_TETRAZones_Zones_Tab();
                    load_trv_TETRASites_Sites_Tab();
                    load_trv_TETRARadios_Radios_Tab();
                    load_trv_TETRACities_Cities_Tab();

                    Draw_All_Sites();

                    if (!first_load)
                    {
                        load_comboBox_Choose_site_for_coverage();
                        first_load = true;
                    }

                    Thread.Sleep(20000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Auditing.Error(ex.Message);
            }
        }

        #endregion

        #region Zones

        Zone[] zones;

        #region Events

        #region Buttons
        private void btn_Add_Zone_Zones_Tab_Click(object sender, EventArgs e)
        {
            try
            {

                if (txt_Zone_Name_Zones_Tab.Text != "")
                {
                    Zone new_Zone = new Zone();
                    new_Zone.Zone_Name = txt_Zone_Name_Zones_Tab.Text;
                    new_Zone.Info = txt_INFO_Zones_Tab.Text;

                    Thread add_zone_thread = new Thread(() => Add_Zone(new_Zone));
                    add_zone_thread.Start();
                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        private void btn_Delete_Zone_Zones_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Thread delete_zone_thread = new Thread(Delete_Zone);
                delete_zone_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        private void btn_Search_Zone_Zones_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Search_Zones();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        private void btn_Edit_Zone_Zones_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_Zone_Name_Zones_Tab.Text != "")
                {
                    Zone Updated_Zone = new Zone();
                    Updated_Zone.Zone_Name = txt_Zone_Name_Zones_Tab.Text;
                    Updated_Zone.Info = txt_INFO_Zones_Tab.Text;
                    Updated_Zone.Zone_ID = Convert.ToInt32(lbl_txt_Zone_Id_Zones_Tab.Text);
                    Thread Edit_zone_thread = new Thread(() => Edit_Zone(Updated_Zone));
                    Edit_zone_thread.Start();
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #endregion

        #region Treeview

        private void trv_TETRAZones_Zones_Tab_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (trv_TETRAZones_Zones_Tab.SelectedNode.Parent.Text != "Zones")
                {
                    Intentional = true;
                    Show_Zone_Fields((Zone)trv_TETRAZones_Zones_Tab.SelectedNode.Parent.Tag);
                }
                else
                {
                    Show_Zone_Fields((Zone)trv_TETRAZones_Zones_Tab.SelectedNode.Tag);
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #endregion

        #endregion

        #region Delegates

        #region status label
        public delegate void Change_Status_Label_Zones_delegate(string message, Color color);
        public void update_Zones_Statues_label(string message, Color color)
        {
            try
            {
                if (lbl_txt_status_Zones_Tab.InvokeRequired)
                {
                    lbl_txt_status_Zones_Tab.Invoke(new Change_Status_Label_Zones_delegate(update_Zones_Statues_label), new object[] { message, color });
                }
                else
                {
                    lbl_txt_status_Zones_Tab.ForeColor = color;
                    lbl_txt_status_Zones_Tab.Text = message;
                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        #endregion

        #region loading bar
        public delegate void Change_zone_loading_bar_percentage(int value);
        public void change_Zone_loadingbar_percentage_async(int value)
        {
            try
            {
                if (circularProgressBar_zone_panel.InvokeRequired)
                {
                    circularProgressBar_zone_panel.Invoke(new Change_zone_loading_bar_percentage(change_Zone_loadingbar_percentage_async), new object[] { value });
                }
                else
                {
                    circularProgressBar_zone_panel.Value = value;
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #region Treeview
        public delegate void load_trv_TETRAZones_Zones_Tab_delegate();
        public void load_trv_TETRAZones_Zones_Tab()
        {

            try
            {
                if (trv_TETRAZones_Zones_Tab.InvokeRequired)
                {
                    load_trv_TETRAZones_Zones_Tab_delegate load_zones_delegate = new load_trv_TETRAZones_Zones_Tab_delegate(load_trv_TETRAZones_Zones_Tab);
                    trv_TETRAZones_Zones_Tab.Invoke(load_zones_delegate);
                }
                else
                {

                    #region Zones
                    //   zones = WS_Obj.Zone_Select_All(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    int len = zones != null ? zones.Length : 0;
                    load_comboBox_Zone_ID_Sites_Tab(zones);

                    trv_TETRAZones_Zones_Tab.BeginUpdate();


                    #region Zones Add, Edit

                    if (len != 0)
                    {
                        for (int i = 0; i < len; i++)
                        {
                            bool Find_Flag = false;
                            //add , edit operations
                            for (int j = 0; j < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count; j++)
                            {
                                Zone selected_zone = (Zone)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag;
                                if (zones[i].Zone_ID == selected_zone.Zone_ID)
                                {
                                    Find_Flag = true;
                                    trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag = zones[i];
                                    trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Text = zones[i].Zone_Name;
                                    break;
                                }
                            }

                            if (Find_Flag == false)
                            {
                                TreeNode new_zone_node = new TreeNode();
                                new_zone_node.Text = zones[i].Zone_Name;
                                new_zone_node.Tag = zones[i];
                                trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Add(new_zone_node);
                            }
                        }
                    }
                    #endregion

                    #region Zones Delete
                    if (len != 0 && trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count > 0)
                    {
                        for (int j = 0; j < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count; j++)
                        {
                            bool Find_Flag = false;

                            Zone Zone_Obj = (Zone)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag;

                            for (int i = 0; i < zones.Length; i++)
                            {
                                if (Zone_Obj.Zone_ID == zones[i].Zone_ID)
                                {
                                    Find_Flag = true;
                                    break;
                                }
                            }

                            if (Find_Flag == false)
                            {
                                trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Remove();
                            }
                        }

                    }
                    #endregion

                    trv_TETRAZones_Zones_Tab.EndUpdate();

                    #endregion

                    #region Sites
                    // sites = WS_Obj.Sites_Select_All(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    int Sites_length = sites != null ? sites.Length : 0;

                    trv_TETRAZones_Zones_Tab.BeginUpdate();
                    #region Sites Add, Edit
                    for (int i = 0; i < Sites_length; i++)
                    {
                        for (int j = 0; j < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count; j++)
                        {
                            Zone checked_zone = (Zone)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag;

                            if (checked_zone.Zone_ID == sites[i].Zone_ID)
                            {
                                bool Find_Flag = false;
                                // if the site belong to this zone in db
                                for (int loop = 0; loop < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes.Count; loop++)
                                {
                                    Sites Sites_Obj = (Sites)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;

                                    if (Sites_Obj.Site_ID == sites[i].Site_ID)
                                    {
                                        // if the site is a subnode of its zone node
                                        trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes[loop].Text = Sites_Obj.Site_Name;
                                        Find_Flag = true;
                                        break;
                                    }
                                }

                                if (Find_Flag == false)
                                {// add the site as a subnode to that zone
                                    TreeNode new_site_subnode = new TreeNode();
                                    new_site_subnode.Text = sites[i].Site_Name;
                                    new_site_subnode.Tag = sites[i];

                                    trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes.Add(new_site_subnode);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Sites Delete

                    for (int j = 0; j < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count; j++)
                    {
                        Zone Zone_Obj = (Zone)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag;
                        for (int loop = 0; loop < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes.Count; loop++)
                        {
                            bool Find_Flag = false;
                            Sites Site_obj = (Sites)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;

                            //check if the subnode is in sites db   
                            for (int i = 0; i < Sites_length; i++)
                            {
                                if (sites[i].Site_ID == Site_obj.Site_ID && sites[i].Zone_ID == Zone_Obj.Zone_ID)
                                {
                                    Find_Flag = true;
                                    break;
                                }
                            }
                            if (Find_Flag == false)
                            {
                                //remove this subnode
                                trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes.RemoveAt(loop);
                            }
                        }
                    }
                    #endregion
                    trv_TETRAZones_Zones_Tab.EndUpdate();

                    #endregion

                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        #endregion

        #endregion

        #region Other Functions
        public void Show_Zone_Fields(Zone selected_zone)
        {
            try
            {
                #region clear fields
                lbl_txt_Zone_Id_Zones_Tab.Text = "";
                txt_Zone_Name_Zones_Tab.Text = "";
                txt_INFO_Zones_Tab.Text = "";
                #endregion
                lbl_txt_Zone_Id_Zones_Tab.Text = selected_zone.Zone_ID.ToString();
                txt_Zone_Name_Zones_Tab.Text = selected_zone.Zone_Name;
                txt_INFO_Zones_Tab.Text = selected_zone.Info;

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public void Add_Zone(Zone new_zone)
        {
            try
            {
                loading_bar_zone_panel_thread();
                bool check = WS_Control_obj.Add_Zone(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, new_zone);
                if (check)
                {
                    zones = WS_Control_obj.Select_all_zones(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Zones_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRAZones_Zones_Tab();
                }
                else
                {
                    update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Delete_Zone()
        {
            try
            {
                loading_bar_zone_panel_thread();
                bool check = WS_Control_obj.Delete_zone(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, Convert.ToInt32(lbl_txt_Zone_Id_Zones_Tab.Text));

                if (check)
                {
                    zones = WS_Control_obj.Select_all_zones(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    load_trv_TETRAZones_Zones_Tab();
                    //load_trv_TETRASites_Sites_Tab();
                    update_Zones_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                update_Zones_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Edit_Zone(Zone updated_zone)
        {
            try
            {
                loading_bar_zone_panel_thread();

                bool check = WS_Control_obj.Edit_zone(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, updated_zone.Zone_ID, updated_zone);

                if (check)
                {
                    zones = WS_Control_obj.Select_all_zones(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Zones_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRAZones_Zones_Tab();
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                update_Zones_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Search_Zones()
        {
            try
            {
                bool found = false;
                foreach (TreeNode node in trv_TETRAZones_Zones_Tab.Nodes[0].Nodes)
                {
                    Intentional = false;
                    loading_bar_zone_panel_thread();

                    if (node.Text == txt_Zone_Name_Zones_Tab.Text.ToString())
                    {
                        found = true;
                        Intentional = true;
                        trv_TETRAZones_Zones_Tab.SelectedNode = node;
                        Zone selected_zone = (Zone)node.Tag;
                        #region clear fields
                        lbl_txt_Zone_Id_Zones_Tab.Text = "";
                        txt_Zone_Name_Zones_Tab.Text = "";
                        txt_INFO_Zones_Tab.Text = "";
                        #endregion

                        lbl_txt_Zone_Id_Zones_Tab.Text = selected_zone.Zone_ID.ToString();
                        txt_Zone_Name_Zones_Tab.Text = selected_zone.Zone_Name;
                        txt_INFO_Zones_Tab.Text = selected_zone.Info;

                        update_Zones_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                        workdone = true;
                        break;
                    }
                }
                if (!found)
                {
                    update_Zones_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                    workdone = true;
                }
                Intentional = false;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        public void zones_loading_bar_progress_cycle()
        {
            while (!workdone)
            {
                Thread.Sleep(1000);
                change_Zone_loadingbar_percentage_async(100);
                Thread.Sleep(1000);
                change_Zone_loadingbar_percentage_async(0);
            }

        }
        public void loading_bar_zone_panel_thread()
        {
            try
            {
                workdone = false;
                Thread loading_zone_operation_thread = new Thread(zones_loading_bar_progress_cycle);
                loading_zone_operation_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }

        }

        #endregion

        #endregion

        #region Sites
        Sites[] sites;
        #region Events

        #region buttons
        private void btn_Add_Site_Sites_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_Latitude_Sites_Tab.Text != "" &&
                       txt_Longtitude_Sites_Tab.Text != "" &&
                       //txt_LA_Sites_Tab.Text != "" &&
                       //txt_Type_Sites_Tab.Text != "" &&
                       txt_SiteName_Sites_Tab.Text != "" &&
                       //txt_Power_Sites_Tab.Text != "" &&
                       //txt_Height_Sites_Tab.Text != "" &&
                       txt_Cellid_Sites_Tab.Text != "" &&
                       comboBox_Zone_ID_Sites_Tab.Items[comboBox_Zone_ID_Sites_Tab.SelectedIndex].ToString() != "")
                {
                    #region add new site
                    Sites new_Site = new Sites();

                    int int_check;

                    int zoneid = Convert.ToInt32(comboBox_Zone_ID_Sites_Tab.Text);

                    new_Site.Site_Name = txt_SiteName_Sites_Tab.Text;


                    float float_check;

                    float sitepower = float.TryParse(txt_Power_Sites_Tab.Text, out float_check) ? float.Parse(txt_Power_Sites_Tab.Text) : 0;
                    new_Site.Site_Power = sitepower;

                    float sitehight = float.TryParse(txt_Height_Sites_Tab.Text, out float_check) ? float.Parse(txt_Height_Sites_Tab.Text) : 0;
                    new_Site.Site_Height = sitehight;

                    float longitude = float.TryParse(txt_Longtitude_Sites_Tab.Text, out float_check) ? float.Parse(txt_Longtitude_Sites_Tab.Text) : 0;
                    new_Site.Longitude = longitude;

                    float latt = float.TryParse(txt_Latitude_Sites_Tab.Text, out float_check) ? float.Parse(txt_Latitude_Sites_Tab.Text) : 0;
                    new_Site.Latitude = latt;

                    new_Site.LA = !string.IsNullOrEmpty(txt_LA_Sites_Tab.Text) ? txt_LA_Sites_Tab.Text : null;

                    new_Site.Info = txt_INFO_Sites_Tab.Text;
                    new_Site.Site_Type = !string.IsNullOrEmpty(txt_Type_Sites_Tab.Text) ? txt_Type_Sites_Tab.Text : null;


                    int cellid = int.TryParse(txt_Cellid_Sites_Tab.Text, out int_check) ? int.Parse(txt_Cellid_Sites_Tab.Text) : 0;
                    new_Site.Cell_ID = cellid;

                    new_Site.Zone_ID = zoneid;

                    Thread add_site_thread = new Thread(() => Add_Site(new_Site));
                    add_site_thread.Start();
                    #endregion
                }
                else
                {
                    update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
                update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);

            }
        }
        private void btn_Edit_Site_Sites_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_Latitude_Sites_Tab.Text != "" &&
                       //txt_LA_Sites_Tab.Text != "" &&
                       //txt_Type_Sites_Tab.Text != "" &&
                       txt_SiteName_Sites_Tab.Text != "" &&
                       txt_Longtitude_Sites_Tab.Text != "" &&
                       //txt_Power_Sites_Tab.Text != "" &&
                       //txt_Height_Sites_Tab.Text != "" &&
                       comboBox_Zone_ID_Sites_Tab.Items[comboBox_Zone_ID_Sites_Tab.SelectedIndex].ToString() != "")
                {

                    #region Edit site
                    int int_check;
                    float float_check;

                    Sites Updated_Site = new Sites();
                    Updated_Site.LA = txt_LA_Sites_Tab.Text;
                    Updated_Site.Info = txt_INFO_Sites_Tab.Text;
                    Updated_Site.Site_Type = txt_Type_Sites_Tab.Text;

                    float latt = float.TryParse(txt_Latitude_Sites_Tab.Text, out float_check) ? float.Parse(txt_Latitude_Sites_Tab.Text) : 0;
                    Updated_Site.Latitude = latt;

                    float longt = float.TryParse(txt_Longtitude_Sites_Tab.Text, out float_check) ? float.Parse(txt_Longtitude_Sites_Tab.Text) : 0;
                    Updated_Site.Longitude = longt;

                    float height = float.TryParse(txt_Height_Sites_Tab.Text, out float_check) ? float.Parse(txt_Height_Sites_Tab.Text) : 0;
                    Updated_Site.Site_Height = height;

                    Updated_Site.Site_Name = txt_SiteName_Sites_Tab.Text;

                    float power = float.TryParse(txt_Power_Sites_Tab.Text, out float_check) ? float.Parse(txt_Power_Sites_Tab.Text) : 0;
                    Updated_Site.Site_Power = power;
                    Updated_Site.Zone_ID = Convert.ToInt32(comboBox_Zone_ID_Sites_Tab.SelectedItem.ToString());
                    int site_id = Convert.ToInt32(lbl_txt_Site_ID_Sites_Tab.Text);
                    Updated_Site.Site_ID = site_id;

                    int cell = int.TryParse(txt_Cellid_Sites_Tab.Text, out int_check) ? int.Parse(txt_Cellid_Sites_Tab.Text) : 0;
                    Updated_Site.Cell_ID = cell;

                    #endregion

                    Thread Edit_site_thread = new Thread(() => Edit_Site(Updated_Site));

                    Edit_site_thread.Start();
                }
                else
                {
                    update_Sites_Statues_label(Code.Singleton.Missing_Inserted_Data_Message, Color.Red);
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        private void btn_Delete_Site_Sites_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Sites site = (Sites)trv_TETRASites_Sites_Tab.SelectedNode.Tag;
                Thread delete_site_thread = new Thread(() => Delete_Site(site.Site_ID));
                delete_site_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        private void btn_Search_Site_Sites_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Search_Sites();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #endregion

        #region treeview
        private void trv_TETRASites_Sites_Tab_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                if (trv_TETRASites_Sites_Tab.SelectedNode.Parent.Tag.ToString() != "head")
                {
                    Intentional = true;
                    Sites Parent_site = (Sites)trv_TETRASites_Sites_Tab.SelectedNode.Parent.Tag;
                    Show_Sites_Fields(Parent_site);
                }
                else
                {
                    Show_Sites_Fields((Sites)trv_TETRASites_Sites_Tab.SelectedNode.Tag);
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        private void trv_TETRASites_Sites_Tab_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Sites selected_site = (Sites)trv_TETRASites_Sites_Tab.SelectedNode.Tag;
                if (selected_site.Latitude != 0 && selected_site.Longitude != 0)
                {
                    Esri_control.Fly_To(selected_site.Latitude, selected_site.Longitude, 1170);
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #endregion

        #region Delegates

        #region status label
        public delegate void Change_Status_Label_Sites_delegate(string message, Color color);
        public void update_Sites_Statues_label(string message, Color color)
        {
            try
            {
                if (lbl_txt_status_Sites_Tab.InvokeRequired)
                {
                    lbl_txt_status_Sites_Tab.Invoke(new Change_Status_Label_Sites_delegate(update_Sites_Statues_label), new object[] { message, color });
                }
                else
                {
                    lbl_txt_status_Sites_Tab.ForeColor = color;
                    lbl_txt_status_Sites_Tab.Text = message;
                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        #endregion

        #region loading bar
        public delegate void Change_site_loading_bar_percentage(int value);
        public void change_Site_loadingbar_percentage_async(int value)
        {
            try
            {
                if (circularProgressBar_site_panel.InvokeRequired)
                {
                    circularProgressBar_site_panel.Invoke(new Change_site_loading_bar_percentage(change_Site_loadingbar_percentage_async), new object[] { value });
                }
                else
                {
                    circularProgressBar_site_panel.Value = value;
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #region treeview
        public delegate void load_comboBox_Zone_ID_Sites_Tab_delegate(Zone[] Zones_Array);
        public void load_comboBox_Zone_ID_Sites_Tab(Zone[] Zones_Array)
        {
            if (comboBox_Zone_ID_Sites_Tab.InvokeRequired)
            {
                comboBox_Zone_ID_Sites_Tab.Invoke(new load_comboBox_Zone_ID_Sites_Tab_delegate(load_comboBox_Zone_ID_Sites_Tab), new object[] { Zones_Array });
            }
            else
            {
                try
                {
                    List<int> Zone_ID_List = new List<int>();
                    foreach (Zone zone in Zones_Array)
                    {
                        Zone_ID_List.Add(zone.Zone_ID);
                    }
                    comboBox_Zone_ID_Sites_Tab.DataSource = Zone_ID_List;
                }
                catch (Exception ex)
                {
                    Auditing.Error(ex.Message);
                }
            }
        }

        public delegate void load_trv_TETRASites_Sites_Tab_delegate();
        public void load_trv_TETRASites_Sites_Tab()
        {
            try
            {
                if (trv_TETRASites_Sites_Tab.InvokeRequired)
                {
                    load_trv_TETRASites_Sites_Tab_delegate load_sites_delegate = new load_trv_TETRASites_Sites_Tab_delegate(load_trv_TETRASites_Sites_Tab);
                    trv_TETRASites_Sites_Tab.Invoke(load_sites_delegate);

                }
                else
                {
                    trv_TETRASites_Sites_Tab.BeginUpdate();

                    #region Sites
                    int sites_length = sites != null ? sites.Length : 0;


                    #region Sites add, edit

                    for (int i = 0; i < sites_length; i++)

                    {
                        bool Find_Flag = false;
                        for (int j = 0; j < trv_TETRASites_Sites_Tab.Nodes[0].Nodes.Count; j++)
                        {
                            Sites selected_site = (Sites)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Tag;
                            if (sites[i].Site_ID == selected_site.Site_ID)
                            {
                                //edit 
                                Find_Flag = true;
                                trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Tag = sites[i];
                                trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Text = sites[i].Site_Name;

                                break;
                            }
                        }
                        if (Find_Flag == false)
                        {
                            //add
                            TreeNode new_site_node = new TreeNode();
                            new_site_node.Text = sites[i].Site_Name;
                            new_site_node.Tag = sites[i];
                            trv_TETRASites_Sites_Tab.Nodes[0].Nodes.Add(new_site_node);

                        }

                    }
                    #endregion

                    #region Sites delete

                    if (sites_length != 0 && trv_TETRASites_Sites_Tab.Nodes[0].Nodes.Count > 0)
                    {
                        for (int j = 0; j < trv_TETRASites_Sites_Tab.Nodes[0].Nodes.Count; j++)
                        { // delete operation
                            bool Find_Flag = false;

                            Sites Site_obj = (Sites)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Tag;
                            for (int i = 0; i < sites.Length; i++)
                            {
                                if (Site_obj.Site_ID == sites[i].Site_ID)
                                {
                                    Find_Flag = true;
                                    break;
                                }

                            }
                            if (Find_Flag == false)
                            {
                                //delete
                                trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Remove();

                            }
                        }

                    }
                    #endregion

                    #endregion

                    #region radios 

                    #region radios ADD
                    //Radios[] radios = WS_Control_obj.Radios_Select_All(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    int len2 = radios != null ? radios.Length : 0;


                    for (int i = 0; i < len2; i++)
                    {
                        for (int j = 0; j < trv_TETRASites_Sites_Tab.Nodes[0].Nodes.Count; j++)
                        {
                            Sites checked_site = (Sites)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Tag;

                            if (checked_site.Site_ID == radios[i].Site_ID)
                            {
                                bool radios_detected = false;
                                // if the radio belong to this site in db
                                for (int loop = 0; loop < trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes.Count; loop++)
                                {
                                    Radios Radio_Obj = (Radios)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;

                                    if (Radio_Obj.Radio_ID == radios[i].Radio_ID)
                                    {// if the site is already a subnode of its site node

                                        trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes[loop].Text = radios[i].Radio_Name;
                                        radios_detected = true;
                                        break;
                                    }
                                }

                                if (radios_detected == false)
                                {
                                    // add the site as a subnode to that zone
                                    TreeNode radio_subnode = new TreeNode();
                                    radio_subnode.Text = radios[i].Radio_Name;
                                    radio_subnode.Tag = radios[i];
                                    trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes.Add(radio_subnode);
                                }
                                break;
                            }

                        }
                    }
                    #endregion

                    #region radios Delete

                    for (int j = 0; j < trv_TETRASites_Sites_Tab.Nodes[0].Nodes.Count; j++)
                    {
                        Sites Site_Obj = (Sites)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Tag;

                        //foreach (TreeNode subnode in trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes)
                        for (int loop = 0; loop < trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes.Count; loop++)
                        {
                            bool radios_detected = false;
                            Radios Radio_obj = (Radios)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;
                            //check if the subnode is in radios db   
                            for (int i = 0; i < len2; i++)
                            {
                                if (Radio_obj.Radio_ID == radios[i].Radio_ID && radios[i].Site_ID == Radio_obj.Site_ID)
                                {
                                    radios_detected = true;
                                    break;
                                }
                            }
                            if (radios_detected == false)
                            {
                                //remove this subnode
                                trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes.RemoveAt(loop);
                            }
                        }
                    }
                    #endregion
                    #endregion


                    trv_TETRASites_Sites_Tab.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion
        #endregion

        #region other functions

        public void Show_Sites_Fields(Sites selected_site)
        {
            try
            {
                #region clear fields
                lbl_txt_Site_ID_Sites_Tab.Text = "";
                txt_SiteName_Sites_Tab.Text = "";
                txt_Latitude_Sites_Tab.Text = "";
                txt_Longtitude_Sites_Tab.Text = "";
                txt_LA_Sites_Tab.Text = "";
                txt_Power_Sites_Tab.Text = "";
                txt_Type_Sites_Tab.Text = "";
                txt_Height_Sites_Tab.Text = "";
                txt_INFO_Sites_Tab.Text = "";
                txt_Cellid_Sites_Tab.Text = "";

                #endregion
                lbl_txt_Site_ID_Sites_Tab.Text = selected_site.Site_ID.ToString();
                txt_SiteName_Sites_Tab.Text = selected_site.Site_Name;
                comboBox_Zone_ID_Sites_Tab.SelectedItem = selected_site.Zone_ID;
                txt_Latitude_Sites_Tab.Text = selected_site.Latitude.ToString();
                txt_Longtitude_Sites_Tab.Text = selected_site.Longitude.ToString();
                txt_LA_Sites_Tab.Text = selected_site.LA.ToString();
                txt_Power_Sites_Tab.Text = selected_site.Site_Power.ToString();
                txt_Type_Sites_Tab.Text = selected_site.Site_Type;
                txt_Height_Sites_Tab.Text = selected_site.Site_Height.ToString();
                txt_INFO_Sites_Tab.Text = selected_site.Info;
                txt_Cellid_Sites_Tab.Text = selected_site.Cell_ID.ToString();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public void Add_Site(Sites new_site)
        {
            try
            {
                loading_bar_site_panel_thread();

                bool check = WS_Control_obj.Add_site(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, new_site);

                if (check)
                {

                    sites = WS_Control_obj.Select_all_sites(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    load_trv_TETRASites_Sites_Tab();
                    update_Sites_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_comboBox_Choose_site_for_coverage();

                }
                workdone = true;
            }
            catch (Exception ex)
            {
                update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Delete_Site(int siteid)
        {
            try
            {
                loading_bar_site_panel_thread();

                bool check = WS_Control_obj.Delete_site(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, siteid);

                if (check)
                {

                    sites = WS_Control_obj.Select_all_sites(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Sites_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    //load_trv_TETRAZones_Zones_Tab();
                    load_trv_TETRASites_Sites_Tab();
                    //load_trv_TETRARadios_Radios_Tab();
                    load_comboBox_Choose_site_for_coverage();

                }
                else
                {
                    update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);

                }
                workdone = true;

            }
            catch (Exception ex)
            {
                update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Edit_Site(Sites updated_site)
        {
            try
            {
                loading_bar_site_panel_thread();
                bool check = WS_Control_obj.Edit_site(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, updated_site.Site_ID, updated_site);
                if (check)
                {
                    sites = WS_Control_obj.Select_all_sites(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Sites_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRASites_Sites_Tab();
                    load_comboBox_Choose_site_for_coverage();

                }
                else
                {
                    update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
                workdone = true;
            }
        }
        public void Search_Sites()
        {
            try
            {
                bool found = false;

                foreach (TreeNode node in trv_TETRASites_Sites_Tab.Nodes[0].Nodes)
                {
                    Intentional = false;

                    if (node.Text == dropdownlist_search_sites_sites_tab.Text)
                    {
                        #region clear fields
                        lbl_txt_Site_ID_Sites_Tab.Text = "";
                        txt_SiteName_Sites_Tab.Text = "";
                        txt_Latitude_Sites_Tab.Text = "";
                        txt_Longtitude_Sites_Tab.Text = "";
                        txt_LA_Sites_Tab.Text = "";
                        txt_Power_Sites_Tab.Text = "";
                        txt_Type_Sites_Tab.Text = "";
                        txt_Height_Sites_Tab.Text = "";
                        txt_INFO_Sites_Tab.Text = "";
                        txt_Cellid_Sites_Tab.Text = "";
                        dropdownlist_search_sites_sites_tab.Text = "";

                        #endregion

                        found = true;
                        Intentional = true;
                        trv_TETRAZones_Zones_Tab.SelectedNode = node;

                        Sites selected_site = (Sites)node.Tag;

                        lbl_txt_Site_ID_Sites_Tab.Text = selected_site.Site_ID.ToString();
                        txt_SiteName_Sites_Tab.Text = selected_site.Site_Name;
                        comboBox_Zone_ID_Sites_Tab.SelectedItem = selected_site.Zone_ID;
                        txt_Latitude_Sites_Tab.Text = selected_site.Latitude.ToString();
                        txt_Longtitude_Sites_Tab.Text = selected_site.Longitude.ToString();
                        txt_LA_Sites_Tab.Text = selected_site.LA.ToString();
                        txt_Power_Sites_Tab.Text = selected_site.Site_Power.ToString();
                        txt_Type_Sites_Tab.Text = selected_site.Site_Type;
                        txt_Height_Sites_Tab.Text = selected_site.Site_Height.ToString();
                        txt_INFO_Sites_Tab.Text = selected_site.Info;
                        txt_Cellid_Sites_Tab.Text = selected_site.Cell_ID.ToString();

                        update_Sites_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                        workdone = true;
                        break;
                    }
                }
                if (!found)
                {
                    update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                    workdone = true;

                }
                Intentional = false;

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public string get_site_name(int id)
        {
            foreach (Sites site in sites)
            {
                if (id == site.Site_ID)
                {
                    return site.Site_Name;
                }
            }
            return "";
        }
        private void site_names_list_DropDown(object sender, EventArgs e)
        {
            List<string> nameslist = new List<string>();
            string substring = dropdownlist_search_sites_sites_tab.Text;

            if (dropdownlist_search_sites_sites_tab.Text != "")
            {
                foreach (Sites site in sites)
                {
                    if (site.Site_Name.StartsWith(substring))
                    {
                        nameslist.Add(site.Site_Name);
                    }
                }
                dropdownlist_search_sites_sites_tab.DataSource = nameslist;
            }

        }
        public void sites_loading_bar_progress()
        {
            while (!workdone)
            {
                Thread.Sleep(1700);
                change_Site_loadingbar_percentage_async(100);
                Thread.Sleep(1700);
                change_Site_loadingbar_percentage_async(0);
            }

        }
        public void loading_bar_site_panel_thread()
        {
            try
            {
                workdone = false;
                Thread loading_site_operation_thread = new Thread(sites_loading_bar_progress);
                loading_site_operation_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }

        #endregion

        #endregion

        #region Radios
        Radios[] radios;
        Radios[] previous_radio_Data;

        #region Events

        #region buttons

        private void btn_Add_Radio_Radios_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                int int_check;
                if (txt_ISSI_Radios_Tab.Text != "" && int.TryParse(txt_Power_Sites_Tab.Text, out int_check))
                {
                    Radios new_Radio = new Radios();

                    int issi = int.Parse(txt_Power_Sites_Tab.Text);
                    new_Radio.ISSI = issi;
                    new_Radio.Radio_Name = !string.IsNullOrEmpty(txt_model_Radios_Tab.Text) ? txt_Radio_Name_Radios_Tab.Text : issi.ToString();

                    new_Radio.Info = !string.IsNullOrEmpty(txt_INFO_Radios_Tab.Text) ? txt_INFO_Radios_Tab.Text : null;
                    new_Radio.Model = !string.IsNullOrEmpty(txt_model_Radios_Tab.Text) ? txt_model_Radios_Tab.Text : null;
                    //new_Radio.RCPIN = !string.IsNullOrEmpty(txt_.Text) ? txt_Serial_Number_Radios_Tab.Text : null
                    new_Radio.SerialNum = !string.IsNullOrEmpty(txt_Serial_Number_Radios_Tab.Text) ? txt_Serial_Number_Radios_Tab.Text : null;
                    new_Radio.Site_ID = 0;
                    new_Radio.TEI = !string.IsNullOrEmpty(txt_TEI_Radios_Tab.Text) ? txt_TEI_Radios_Tab.Text : null;

                    new_Radio.Radio_Type = !string.IsNullOrEmpty(comboBox_Radio_type_Map_pnl.Text) ? comboBox_Radio_type_Map_pnl.Text : null;
                    //new_Radio.Client_R_C_TG_Flag = 0;
                    if (CB_City_Radios_tab.Text != "none")
                    {
                        foreach (City city in Cities)
                        {
                            if (city.CityName == CB_City_Radios_tab.Text)
                            {
                                new_Radio.CityID = city.CityID;
                                break;
                            }
                        }
                    }
                    Thread add_radio_thread = new Thread(() => Add_Radio(new_Radio));
                    add_radio_thread.Start();

                }
                else
                {
                    update_Radios_Statues_label(Code.Singleton.Missing_Inserted_Data_Message, Color.Red);

                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }

        private void btn_Delete_Radio_Radios_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Radios radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                Thread delete_radio_thread = new Thread(() => Delete_radio(radio.Radio_ID));
                delete_radio_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        private void btn_Edit_Radio_Radios_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                int int_check;
                if (int.TryParse(txt_ISSI_Radios_Tab.Text, out int_check) && !string.IsNullOrEmpty(txt_Radio_Name_Radios_Tab.Text))
                {
                    Radios Updated_Radio = new Radios();
                    Updated_Radio.Radio_Name = !string.IsNullOrEmpty(txt_Radio_Name_Radios_Tab.Text) ? txt_Radio_Name_Radios_Tab.Text : txt_ISSI_Radios_Tab.Text;


                    int issi = int.Parse(txt_ISSI_Radios_Tab.Text);
                    Updated_Radio.ISSI = issi;

                    int siteid = !string.IsNullOrEmpty(txt_Site_ID_Radios_Tab.Text) ? int.Parse(txt_Site_ID_Radios_Tab.Text) : 0;
                    Updated_Radio.Site_ID = siteid;

                    int radioid = int.Parse(lbl_txt_Radio_ID_Radios_Tab.Text);
                    Updated_Radio.Radio_ID = radioid;

                    Updated_Radio.Info = !string.IsNullOrEmpty(txt_INFO_Radios_Tab.Text) ? txt_INFO_Radios_Tab.Text : null;
                    Updated_Radio.Model = !string.IsNullOrEmpty(txt_model_Radios_Tab.Text) ? txt_model_Radios_Tab.Text : null;
                    Updated_Radio.RCPIN = "";
                    Updated_Radio.SerialNum = !string.IsNullOrEmpty(txt_Serial_Number_Radios_Tab.Text) ? txt_Serial_Number_Radios_Tab.Text : null;
                    Updated_Radio.TEI = !string.IsNullOrEmpty(txt_TEI_Radios_Tab.Text) ? txt_TEI_Radios_Tab.Text : null;

                    Updated_Radio.Radio_Type = !string.IsNullOrEmpty(comboBox_Radio_type_Radios_tab.Text) ? comboBox_Radio_type_Radios_tab.Text : null;

                    foreach (City city in Cities)
                    {
                        if (city.CityName == CB_City_Radios_tab.Text && CB_City_Radios_tab.Text != "none")
                        {
                            Updated_Radio.CityID = city.CityID;
                            break;
                        }
                    }
                    workdone = true;
                    Thread Edit_radio_thread = new Thread(() => Edit_Radio(Updated_Radio));
                    Edit_radio_thread.Start();

                }
                else
                {
                    update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        private void btn_Search_Radio_Radios_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Search_Radios();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }


        #endregion

        #region treeview
        private void trv_TETRARadios_Radios_Tab_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                #region clear fields
                lbl_txt_Radio_ID_Radios_Tab.Text = "";
                txt_Radio_Name_Radios_Tab.Text = "";
                txt_ISSI_Radios_Tab.Text = "";
                lbl_txt_Date_Time_Radios_Tab.Text = "";
                lbl_txt_lattitude_Radios_Tab.Text = "";
                lbl_txt_Longtitude_Radios_Tab.Text = "";
                txt_Site_ID_Radios_Tab.Text = "";
                txt_Serial_Number_Radios_Tab.Text = "";
                txt_TEI_Radios_Tab.Text = "";
                txt_model_Radios_Tab.Text = "";
                txt_INFO_Radios_Tab.Text = "";
                comboBox_Radio_type_Radios_tab.SelectedItem = "";
                txt_neighbours_radios_tab.Text = "";
                lbl_txt_sitename_radios_tab.Text = "";
                lbl_txt_site_signal_strength_radios_tab.Text = "";
                #endregion

                if (trv_TETRARadios_Radios_Tab.SelectedNode.Text != "Radios")
                {
                    Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;

                    lbl_txt_Radio_ID_Radios_Tab.Text = selected_radio.Radio_ID.ToString();
                    txt_Radio_Name_Radios_Tab.Text = selected_radio.Radio_Name.ToString();
                    txt_ISSI_Radios_Tab.Text = selected_radio.ISSI.ToString();
                    if (selected_radio.gpsPosition != null)
                    {
                        lbl_txt_Date_Time_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.gpsPosition.Date_Time.ToString()) ? selected_radio.gpsPosition.Date_Time.ToString() : "";
                        lbl_txt_lattitude_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.gpsPosition.Latitude.ToString()) ? selected_radio.gpsPosition.Latitude.ToString() : "";
                        lbl_txt_Longtitude_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.gpsPosition.Longitude.ToString()) ? selected_radio.gpsPosition.Longitude.ToString() : "";

                    }
                    txt_Site_ID_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.Site_ID.ToString()) && selected_radio.Site_ID != -1 ? selected_radio.Site_ID.ToString() : "";

                    txt_Serial_Number_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.SerialNum.ToString()) ? selected_radio.SerialNum.ToString() : "";
                    txt_TEI_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.TEI.ToString()) ? selected_radio.TEI.ToString() : "";
                    txt_model_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.Model.ToString()) ? selected_radio.Model.ToString() : "";
                    txt_INFO_Radios_Tab.Text = selected_radio.Info.ToString();
                    comboBox_Radio_type_Radios_tab.SelectedItem = !string.IsNullOrWhiteSpace(selected_radio.Radio_Type) ? selected_radio.Radio_Type : "";

                    if (selected_radio.Site_ID != 0)
                    {
                        foreach (Sites site in sites)
                        {
                            if (site.Site_ID == selected_radio.Site_ID)
                            {
                                lbl_txt_sitename_radios_tab.Text = site.Site_Name;
                                break;
                            }
                        }
                    }
                    //neigbhours
                    get_neightbour_Sites(selected_radio);
                    show_current_Volume();
                    show_current_talkgroup();

                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        public delegate void get_site_neightbours_delegate(Radios radio);
        public void get_neightbour_Sites(Radios radio)
        {
            try
            {
                if (txt_neighbours_radios_tab.InvokeRequired)
                {
                    get_site_neightbours_delegate del = new get_site_neightbours_delegate(get_neightbour_Sites);
                    txt_neighbours_radios_tab.Invoke(del);
                }
                else
                {

                    Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                    if (selected_radio.gpsPosition != null)
                    {
                        Logs[] radio_logs = WS_Control_obj.Get_logs_filtered_by_Pos_ID(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, radio.gpsPosition.Pos_ID);
                        if (radio_logs != null)
                        {
                            foreach (Logs log in radio_logs)
                            {
                                foreach (Sites site in sites)
                                {

                                    if (site.Site_ID == log.Site_ID)
                                    {
                                        if (selected_radio.Site_ID == site.Site_ID)
                                        {
                                            int site_rssi = Convert.ToInt32(log.RSSI);
                                            lbl_txt_site_signal_strength_radios_tab.Text = " " + Dbm_values[site_rssi - 1];
                                            break;
                                        }

                                        int rssi = Convert.ToInt32(log.RSSI);
                                        txt_neighbours_radios_tab.Text += "Zone " + site.Zone_ID + " ID " + site.Cell_ID + " -" + site.Site_Name + " " + " ( " + " " + Dbm_values[rssi - 1] + " )" + " \t " + "\t" + "\t" + "\t" + "\t";
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        private void trv_TETRARadios_Radios_Tab_DoubleClick(object sender, EventArgs e)
        {
            if (trv_TETRARadios_Radios_Tab.SelectedNode != null && trv_TETRARadios_Radios_Tab.SelectedNode.Text != "Radios")
            {
                trv_TETRARadios_Radios_Tab.SelectedNode.ForeColor = Color.Black;
                Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                if (selected_radio.gpsPosition != null)
                {
                    if (selected_radio.gpsPosition.Latitude != 0 && selected_radio.gpsPosition.Longitude != 0)
                    {
                        string Pic_uri = Code.Singleton.radio_picture_uri;
                        User_Picture_MapPoint radio_point = new User_Picture_MapPoint();
                        radio_point.Uri = Pic_uri;
                        MapPoint_Position radio_position = new MapPoint_Position();
                        radio_position.Latitude = selected_radio.gpsPosition.Latitude;
                        radio_position.Longitude = selected_radio.gpsPosition.Longitude;
                        radio_point.User_Mappoint_Postion = radio_position;
                        radio_point.User_Mappoint_Visibility = true;
                        Esri_control._Esri_MAP_Control.Add_User_MapPoint("radios", radio_point, selected_radio.ISSI);
                        Esri_control.Fly_To(selected_radio.gpsPosition.Latitude, selected_radio.gpsPosition.Longitude, 1170);
                    }

                }
            }
        }
        #endregion

        #endregion

        #region Delegates

        #region  treeview
        public delegate void load_trv_TETRARadios_Radios_Tab_delegate();
        public void load_trv_TETRARadios_Radios_Tab()
        {
            try
            {
                if (trv_TETRARadios_Radios_Tab.InvokeRequired)
                {
                    load_trv_TETRARadios_Radios_Tab_delegate load_radios_delegate = new load_trv_TETRARadios_Radios_Tab_delegate(load_trv_TETRARadios_Radios_Tab);
                    trv_TETRARadios_Radios_Tab.Invoke(load_radios_delegate);

                }
                else
                {

                    int Radios_length = radios != null ? radios.Length : 0;

                    #region Radio add,edit

                    if (Radios_length != 0)
                    {
                        trv_TETRARadios_Radios_Tab.BeginUpdate();

                        for (int i = 0; i < Radios_length; i++)
                        {
                            bool Find_Flag = false;
                            for (int j = 0; j < trv_TETRARadios_Radios_Tab.Nodes[0].Nodes.Count; j++)
                            {
                                Radios Radio_Obj = (Radios)trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].Tag;
                                if (radios[i].Radio_ID == Radio_Obj.Radio_ID)
                                {
                                    Find_Flag = true;
                                    trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].Tag = radios[i];
                                    trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].Text = radios[i].Radio_Name;
                                    if (radios[i].Client_R_C_TG_Flag == 1 || radios[i].Client_R_GPS_Flag == 1 || radios[i].Client_R_Switch_Off_Flag == 1 || radios[i].Client_R_Volum_Flag == 1 || radios[i].Client_R_I_Call_Flag == 1)
                                    {
                                        trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].ForeColor = Color.Orange;
                                    }
                                    #region Update Radios font color based on client requests status
                                    if (previous_radio_Data != null)
                                    {
                                        for (int loop = 0; loop < previous_radio_Data.Length; loop++)
                                        {
                                            if (previous_radio_Data[loop].Radio_ID == radios[i].Radio_ID)
                                            {
                                                if (previous_radio_Data[loop].Client_R_C_TG_Flag != radios[i].Client_R_C_TG_Flag ||
                                                    previous_radio_Data[loop].Client_R_GPS_Flag != radios[i].Client_R_GPS_Flag ||
                                                    previous_radio_Data[loop].Client_R_Switch_Off_Flag != radios[i].Client_R_Switch_Off_Flag ||
                                                    previous_radio_Data[loop].Client_R_Volum_Flag != radios[i].Client_R_Volum_Flag ||
                                                    previous_radio_Data[loop].Client_R_I_Call_Flag != radios[i].Client_R_I_Call_Flag)
                                                {
                                                    if (radios[i].Client_R_C_TG_Flag == 0 &&
                                                    radios[i].Client_R_GPS_Flag == 0 &&
                                                    radios[i].Client_R_Switch_Off_Flag == 0 &&
                                                    radios[i].Client_R_Volum_Flag == 0 &&
                                                    radios[i].Client_R_I_Call_Flag == 0)
                                                    {
                                                        trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].ForeColor = Color.Green;
                                                        if (radios[i].Client_R_GPS_Flag == 0 && previous_radio_Data[i].Client_R_GPS_Flag == 1)
                                                        {
                                                            if (radios[i].gpsPosition != null)
                                                            {
                                                                if (radios[i].gpsPosition.Latitude != 0 && radios[i].gpsPosition.Longitude != 0)
                                                                {
                                                                    string Pic_uri = Code.Singleton.radio_picture_uri;
                                                                    User_Picture_MapPoint radio_point = new User_Picture_MapPoint();
                                                                    radio_point.Uri = Pic_uri;
                                                                    MapPoint_Position radio_position = new MapPoint_Position();
                                                                    radio_position.Latitude = radios[i].gpsPosition.Latitude;
                                                                    radio_position.Longitude = radios[i].gpsPosition.Longitude;
                                                                    radio_point.User_Mappoint_Postion = radio_position;
                                                                    radio_point.User_Mappoint_Visibility = true;
                                                                    Esri_control._Esri_MAP_Control.Add_User_MapPoint("radios", radio_point, radios[i].ISSI);
                                                                    Esri_control.Fly_To(radios[i].gpsPosition.Latitude, radios[i].gpsPosition.Longitude, 1170);
                                                                }
                                                            }

                                                        }

                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    #endregion

                                    break;
                                }
                            }
                            if (Find_Flag == false)
                            {
                                //add
                                TreeNode new_radio_node = new TreeNode();
                                new_radio_node.Text = radios[i].Radio_Name;
                                new_radio_node.Tag = radios[i];
                                trv_TETRARadios_Radios_Tab.Nodes[0].Nodes.Add(new_radio_node);
                                if (radios[i].Client_R_C_TG_Flag == 1 || radios[i].Client_R_I_Call_Flag == 1 || radios[i].Client_R_GPS_Flag == 1 || radios[i].Client_R_Switch_Off_Flag == 1 || radios[i].Client_R_Volum_Flag == 1)
                                {
                                    new_radio_node.ForeColor = Color.Orange;
                                }

                            }
                        }
                        trv_TETRARadios_Radios_Tab.EndUpdate();
                    }

                    #endregion

                    #region Radio delete


                    if (Radios_length != 0 && trv_TETRARadios_Radios_Tab.Nodes[0].Nodes.Count > 0)
                    {
                        trv_TETRARadios_Radios_Tab.BeginUpdate();

                        for (int j = 0; j < trv_TETRARadios_Radios_Tab.Nodes[0].Nodes.Count; j++)
                        { // delete operation
                            bool check_flag = false;
                            Radios radio_obj = (Radios)trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].Tag;

                            for (int i = 0; i < radios.Length; i++)
                            {
                                if (radio_obj.Radio_ID == radios[i].Radio_ID)
                                {
                                    check_flag = true;
                                    break;
                                }
                            }
                            if (check_flag == false)
                            {
                                //delete
                                trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].Remove();
                            }
                        }
                        trv_TETRARadios_Radios_Tab.EndUpdate();

                    }
                    #endregion
                    if (radios.Length != 0)
                    {
                        previous_radio_Data = new Radios[radios.Length];
                        radios.CopyTo(previous_radio_Data, 0);

                    }


                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #region status label

        public delegate void Change_Status_Label_Radios_delegate(string message, Color color);
        public void update_Radios_Statues_label(string message, Color color)
        {
            try
            {
                if (lbl_txt_status_Radios_Tab.InvokeRequired)
                {
                    lbl_txt_status_Radios_Tab.Invoke(new Change_Status_Label_Radios_delegate(update_Radios_Statues_label), new object[] { message, color });
                }
                else
                {
                    lbl_txt_status_Radios_Tab.ForeColor = color;
                    lbl_txt_status_Radios_Tab.Text = message;
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #region loading bar

        public delegate void Change_radio_loading_bar_percentage(int value);
        public void change_radio_loadingbar_percentage(int value)
        {
            try
            {
                if (circularProgressBar_radio_panel.InvokeRequired)
                {
                    circularProgressBar_radio_panel.Invoke(new Change_radio_loading_bar_percentage(change_radio_loadingbar_percentage), new object[] { value });
                }
                else
                {
                    circularProgressBar_radio_panel.Value = value;
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #region radio type combobox
        public delegate void get_radio_type_from_combobox_type_delegate();
        public void get_Radio__type_from_Combobox()
        {
            if (comboBox_Radio_type_Radios_tab.InvokeRequired)
            {
                comboBox_Radio_type_Radios_tab.Invoke(new get_radio_type_from_combobox_type_delegate(get_Radio__type_from_Combobox));
            }
            else
            {
                if (comboBox_Radio_type_Radios_tab.Text != "")
                {
                    radio_type = comboBox_Radio_type_Radios_tab.Text.ToString();
                    workdone = true;
                }
                else
                {
                    radio_type = "";
                    workdone = true;
                }
            }
        }
        #endregion

        #endregion

        #region other functions

        public void Add_Radio(Radios new_radio)
        {
            try
            {
                loading_bar_radio_panel_thread();
                bool check = WS_Control_obj.Add_radio(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, new_radio);

                if (check)
                {
                    radios = WS_Control_obj.Select_all_radios(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Radios_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRARadios_Radios_Tab();
                }
                else
                {
                    update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Delete_radio(int radio_id)
        {
            try
            {
                loading_bar_radio_panel_thread();
                bool check = WS_Control_obj.Delete_radio(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, radio_id);
                if (check)
                {
                    radios = WS_Control_obj.Select_all_radios(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Radios_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRARadios_Radios_Tab();
                }
                else
                {
                    update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }

                workdone = true;
            }
            catch (Exception ex)
            {
                update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Edit_Radio(Radios Updated_Radio)
        {
            try
            {
                loading_bar_radio_panel_thread();
                bool check = WS_Control_obj.Edit_radio(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, Updated_Radio.Radio_ID, Updated_Radio);
                if (check)
                {
                    radios = WS_Control_obj.Select_all_radios(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Radios_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRARadios_Radios_Tab();
                }
                else
                {
                    update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Search_Radios()
        {
            int Issi_key;
            bool found = false;
            try
            {
                int int_check = 0;
                if (int.TryParse(txt_ISSI_Radios_Tab.Text, out int_check))
                {
                    Issi_key = Convert.ToInt32(txt_ISSI_Radios_Tab.Text);
                    #region searching the radio
                    foreach (TreeNode node in trv_TETRARadios_Radios_Tab.Nodes[0].Nodes)
                    {
                        Intentional = false;
                        Radios Radio = (Radios)node.Tag;
                        if (Radio.ISSI == Issi_key)
                        {

                            #region clear fields
                            lbl_txt_Radio_ID_Radios_Tab.Text = "";
                            txt_Radio_Name_Radios_Tab.Text = "";
                            txt_ISSI_Radios_Tab.Text = "";
                            lbl_txt_Date_Time_Radios_Tab.Text = "";
                            lbl_txt_lattitude_Radios_Tab.Text = "";
                            lbl_txt_Longtitude_Radios_Tab.Text = "";
                            txt_Site_ID_Radios_Tab.Text = "";
                            txt_Serial_Number_Radios_Tab.Text = "";
                            txt_TEI_Radios_Tab.Text = "";
                            txt_model_Radios_Tab.Text = "";
                            txt_INFO_Radios_Tab.Text = "";
                            comboBox_Radio_type_Radios_tab.SelectedItem = "";
                            CB_City_Radios_tab.Text = "none";
                            #endregion

                            Intentional = true;
                            trv_TETRAZones_Zones_Tab.SelectedNode = node;
                            Radios selected_radio = (Radios)node.Tag;
                            found = true;

                            lbl_txt_Radio_ID_Radios_Tab.Text = selected_radio.Radio_ID.ToString();
                            txt_Radio_Name_Radios_Tab.Text = selected_radio.Radio_Name.ToString();
                            txt_ISSI_Radios_Tab.Text = selected_radio.ISSI.ToString();
                            if (selected_radio.gpsPosition != null)
                            {
                                lbl_txt_Date_Time_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.gpsPosition.Date_Time.ToString()) ? selected_radio.gpsPosition.Date_Time.ToString() : "";
                                lbl_txt_lattitude_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.gpsPosition.Latitude.ToString()) ? selected_radio.gpsPosition.Latitude.ToString() : "";
                                lbl_txt_Longtitude_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.gpsPosition.Longitude.ToString()) ? selected_radio.gpsPosition.Longitude.ToString() : "";
                            }
                            txt_Site_ID_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.Site_ID.ToString()) ? selected_radio.Site_ID.ToString() : "";

                            txt_Serial_Number_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.SerialNum.ToString()) ? selected_radio.SerialNum.ToString() : "";
                            txt_TEI_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.TEI.ToString()) ? selected_radio.TEI.ToString() : "";
                            txt_model_Radios_Tab.Text = !string.IsNullOrWhiteSpace(selected_radio.Model.ToString()) ? selected_radio.Model.ToString() : "";
                            txt_INFO_Radios_Tab.Text = selected_radio.Info.ToString();

                            comboBox_Radio_type_Radios_tab.Text = selected_radio.Radio_Type;
                            CB_City_Radios_tab.Text = get_city_name(selected_radio.CityID);

                            show_current_Volume();
                            show_current_talkgroup();
                            update_Radios_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                            break;
                        }
                    }
                    #endregion
                    if (!found)
                    {
                        update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                    }
                    Intentional = false;
                }
                else
                {
                    update_Radios_Statues_label(Code.Singleton.Wrong_issi_input_message, Color.Red);
                }
            }
            catch (Exception ex)
            {
                update_Radios_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void radios_loading_bar_progress()
        {
            Thread.Sleep(1000);
            change_radio_loadingbar_percentage(100);
            Thread.Sleep(1000);
            change_radio_loadingbar_percentage(0);
        }
        public void loading_bar_radio_panel_thread()
        {
            try
            {
                workdone = false;
                Thread loading_radio_operation_thread = new Thread(radios_loading_bar_progress);
                loading_radio_operation_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }

        }
        #endregion

        #endregion

        #region Radio status bar

        #region Events
        private void SB_btn_Turnoff_Radios_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Thread turnoff_radio_thread = new Thread(turnoff_radio_request);
                turnoff_radio_thread.Start();
                workdone = false;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        private void SB_Volume_CB_Radios_Tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Thread change_volume_thread = new Thread(change_volume_request);
                change_volume_thread.Start();
                workdone = false;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        private void SB_TalkGroups_CB_Radios_Tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Thread choose_talkgroup_thread = new Thread(change_talkgroup_request);
                choose_talkgroup_thread.Start();
                workdone = false;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        private void SB_btn_Request_radio_location_Tab_Click(object sender, EventArgs e)
        {
            Thread request_location_thread = new Thread(request_location_request);
            request_location_thread.Start();
            workdone = false;
        }
        private void SB_btn_open_mic_Radios_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Thread open_mic_thread = new Thread(open_mic_radio_request);
                open_mic_thread.Start();
                workdone = false;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #endregion

        #region Delegates
        public delegate void Change_Volume_from_combobox_delegate();
        public void change_volume_request()
        {
            try
            {
                if (trv_TETRARadios_Radios_Tab.InvokeRequired)
                {
                    trv_TETRARadios_Radios_Tab.Invoke(new Change_Volume_from_combobox_delegate(change_volume_request));
                }
                else
                {
                    try
                    {
                        int selected_volume = Convert.ToInt32(SB_Volume_CB_Radios_Tab.SelectedItem.Text);

                        Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                        selected_radio.Client_R_Volum_Flag = 1;
                        selected_radio.Current_Volum_Level = selected_volume;
                        bool flag = WS_Control_obj.Edit_radio(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, selected_radio.Radio_ID, selected_radio);
                        workdone = true;
                        if (check_if_radio_flag_requests_are_off(selected_radio) == false && flag)
                        {
                            //change color to orange
                            trv_TETRARadios_Radios_Tab.SelectedNode.ForeColor = Color.DarkOrange;
                        }
                    }
                    catch (Exception ex)
                    {
                        Auditing.Error(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        public delegate void change_talkgroup_request_delegate();
        public void change_talkgroup_request()
        {
            try
            {
                if (trv_TETRARadios_Radios_Tab.InvokeRequired)
                {
                    trv_TETRARadios_Radios_Tab.Invoke(new change_talkgroup_request_delegate(change_talkgroup_request));
                }
                else
                {
                    Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                    selected_radio.Client_R_C_TG_Flag = 1;
                    foreach (Channels channel in selected_radio.channelsCollection)
                    {
                        string selected_talkgroup_from_combobox = SB_TalkGroups_CB_Radios_Tab.SelectedItem.Text;
                        if (channel.Channel_Name == selected_talkgroup_from_combobox)
                        {
                            Channels selected_channel = channel;
                            selected_radio.Client_R_C_TG_Flag = 1;
                            selected_radio.Selected_TG_ID = selected_channel.Channel_ID;
                            bool flag = WS_Control_obj.Edit_radio(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, selected_radio.Radio_ID, selected_radio);
                            workdone = true;

                            if (check_if_radio_flag_requests_are_off(selected_radio) == false && flag)
                            {
                                //change color to orange
                                trv_TETRARadios_Radios_Tab.SelectedNode.ForeColor = Color.DarkOrange;
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        public delegate void turn_off_radio_request_delegate();
        public void turnoff_radio_request()
        {
            try
            {
                if (trv_TETRARadios_Radios_Tab.InvokeRequired)
                {
                    trv_TETRARadios_Radios_Tab.Invoke(new turn_off_radio_request_delegate(turnoff_radio_request));
                }
                else
                {
                    Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                    selected_radio.Client_R_Switch_Off_Flag = 1;
                    bool check = WS_Control_obj.Edit_radio(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, selected_radio.Radio_ID, selected_radio);

                    if (check_if_radio_flag_requests_are_off(selected_radio) == false && check)
                    {
                        //change color to orange
                        trv_TETRARadios_Radios_Tab.SelectedNode.ForeColor = Color.DarkOrange;
                    }
                }
                workdone = true;

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        public delegate void open_mic_request_delegate();
        public void open_mic_radio_request()
        {
            try
            {
                if (trv_TETRARadios_Radios_Tab.InvokeRequired)
                {
                    trv_TETRARadios_Radios_Tab.Invoke(new open_mic_request_delegate(open_mic_radio_request));
                }
                else
                {
                    Radios selected_radio_position = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                    selected_radio_position.Client_R_I_Call_Flag = 1;
                    bool flag = WS_Control_obj.Edit_radio(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, selected_radio_position.Radio_ID, selected_radio_position);

                    if (check_if_radio_flag_requests_are_off(selected_radio_position) == false && flag)
                    {
                        //change color to orange
                        trv_TETRARadios_Radios_Tab.SelectedNode.ForeColor = Color.DarkOrange;
                    }
                }
                workdone = true;

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        public delegate void request_location_delegate();
        public void request_location_request()
        {
            try
            {
                if (trv_TETRARadios_Radios_Tab.InvokeRequired)
                {
                    trv_TETRARadios_Radios_Tab.Invoke(new request_location_delegate(request_location_request));
                }
                else
                {
                    if (trv_TETRARadios_Radios_Tab.SelectedNode != null)
                    {
                        Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                        selected_radio.Client_R_GPS_Flag = 1;
                        bool flag = WS_Control_obj.Edit_radio(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, selected_radio.Radio_ID, selected_radio);
                        //GPS_Position location = selected_radio.gpsPosition;

                        if (check_if_radio_flag_requests_are_off(selected_radio) == false && flag)
                        {
                            // flag =1 -> request -> change color to orange
                            trv_TETRARadios_Radios_Tab.SelectedNode.ForeColor = Color.DarkOrange;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }


        #endregion

        #region Other Functions
        public void show_current_Volume()
        {
            try
            {
                if (trv_TETRARadios_Radios_Tab.SelectedNode != null)
                {
                    Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                    SB_Volume_CB_Radios_Tab.Text = selected_radio.Current_Volum_Level.ToString();
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }

        }
        public void Load_combobox_talkgroups()
        {
            try
            {
                #region load in SB tg combobox
                try
                {
                    SB_TalkGroups_CB_Radios_Tab.Items.Clear();

                    if (trv_TETRARadios_Radios_Tab.SelectedNode != null)
                    {
                        Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                        if (selected_radio.channelsCollection != null && selected_radio.channelsCollection.Length != 0)
                        {
                            SB_TalkGroups_CB_Radios_Tab.Items.Clear();

                            foreach (Channels channel in selected_radio.channelsCollection)
                            {
                                // add to the combobox
                                string TG = channel.Channel_Name.ToString();
                                SB_TalkGroups_CB_Radios_Tab.Items.Add(TG);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Auditing.Error(ex.Message);

                }
                #endregion

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }

        }
        public void show_current_talkgroup()
        {
            try
            {
                SB_TalkGroups_CB_Radios_Tab.Items.Clear();
                SB_TalkGroups_CB_Radios_Tab.Text = "";

                Load_combobox_talkgroups();

                if (trv_TETRARadios_Radios_Tab.SelectedNode != null)
                {
                    Radios selected_radio = (Radios)trv_TETRARadios_Radios_Tab.SelectedNode.Tag;
                    string channel_name = "";
                    if (selected_radio.channelsCollection != null && selected_radio.channelsCollection.Length != 0)
                    {
                        foreach (Channels tg in selected_radio.channelsCollection)
                        {
                            if (tg.Channel_ID == selected_radio.Selected_TG_ID)
                            {
                                channel_name = tg.Channel_Name;
                            }
                        }
                        SB_TalkGroups_CB_Radios_Tab.Text = channel_name;

                    }

                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }

        }
        public bool check_if_radio_flag_requests_are_off(Radios radio)
        {
            if (radio.Client_R_C_TG_Flag == 0 && radio.Client_R_GPS_Flag == 0 && radio.Client_R_Switch_Off_Flag == 0 && radio.Client_R_Volum_Flag == 0 && radio.Client_R_I_Call_Flag == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region Map

        ElementHost ESRI_MAP_Host = new ElementHost();
        Esri_control Esri_control = new Esri_control();


        #region Events
        private void Main_MAP_Object_Click(object sender, SYSTEL_ESRI_Control.TappedReturnEventArgs args, int option)
        {
            try
            {

                #region spotting solution
                if (option == 0)
                {
                    //site 
                    update_Sites_Statues_label(args.Graphic_Name, Color.Green);

                    foreach (Sites site in sites)
                    {
                        if (site.Site_ID.ToString() == args.Graphic_Name)
                        {
                            Show_Sites_Fields(site);
                            pnl_Site_onMap.Visible = true;
                            pnl_Site_onMap.BringToFront();
                            show_Site_info_onMap_panel(site);

                            if (Tabview_TETRASites_Tab.Visible || Tabview_TETRARadios_Tab.Visible || Tabview_TETRA_Zones_Tab.Visible || Tabview_TETRACities_Tab.Visible)
                            {//tab pages opned
                                pnl_Site_onMap.Location = new Point((Int32)args.X + 20, (Int32)args.Y + 30);
                            }
                            else
                            {// tap pages docked
                                pnl_Site_onMap.Location = new Point((Int32)args.X - 3, (Int32)args.Y + 20);
                            }
                            break;
                        }
                    }
                }
                else if (option == 2)
                {
                    //radio or coverage point 
                    if (args.GraphicOverlay_Name == "radios")
                    {
                        //radio
                        foreach (Radios radio in radios)
                        {
                            if (radio.Radio_Name == args.Graphic_Name)
                            {
                                pnl_Radio_onMap.Visible = true;
                                pnl_Radio_onMap.BringToFront();
                                show_Radio_info_onMap_panel(radio);

                                if (Tabview_TETRASites_Tab.Visible || Tabview_TETRARadios_Tab.Visible || Tabview_TETRA_Zones_Tab.Visible || Tabview_TETRACities_Tab.Visible)
                                { //tab pages opned
                                    pnl_Radio_onMap.Location = new Point((Int32)args.X + 20, (Int32)args.Y + 30);
                                }
                                else
                                {// tab pages docked
                                    pnl_Radio_onMap.Location = new Point((Int32)args.X - 3, (Int32)args.Y + 20);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        //coverage point 
                        foreach (Coverage_info info in Coverage_points_info)
                        {
                            if (args.Graphic_Name == info.PosID.ToString())
                            {
                                pnl_Coverage_point_info.Visible = true;
                                pnl_Coverage_point_info.BringToFront();
                                show_Coverage_point_info_onmap_panel(info);

                                if (Tabview_TETRASites_Tab.Visible || Tabview_TETRARadios_Tab.Visible || Tabview_TETRA_Zones_Tab.Visible || Tabview_TETRACities_Tab.Visible)
                                {//tab pages opned
                                    pnl_Coverage_point_info.Location = new Point((Int32)args.X + 20, (Int32)args.Y + 30);
                                }
                                else
                                {// tab pages docked
                                    pnl_Coverage_point_info.Location = new Point((Int32)args.X - 3, (Int32)args.Y + 20);
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //close panels
                    pnl_Radio_onMap.Visible = false;
                    pnl_Site_onMap.Visible = false;
                    pnl_Coverage_point_info.Visible = false;

                }
                #endregion
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #region delegate
        public delegate void draw_sites_delegate();
        public void Draw_All_Sites()
        {
            if (pnl_Map.InvokeRequired)
            {
                pnl_Map.Invoke(new draw_sites_delegate(Draw_All_Sites));
            }
            else
            {
                List<User_Picture_MapPoint> map_points = new List<User_Picture_MapPoint>();
                List<User_Simple_MapPoint> mapp_points_V2 = new List<User_Simple_MapPoint>();
                List<double> IDs = new List<double>();
                List<double> IDS2 = new List<double>();

                if (sites != null)
                {
                    foreach (Sites selected_site in sites)
                    {
                        User_Picture_MapPoint point = new User_Picture_MapPoint();
                        point.Uri = (Code.Singleton.site_picture_uri);

                        point.User_Mappoint_Postion.Latitude = selected_site.Latitude;
                        point.User_Mappoint_Postion.Longitude = selected_site.Longitude;
                        point.User_Mappoint_Visibility = true;

                        User_Simple_MapPoint point_noPic = new User_Simple_MapPoint();
                        point_noPic.User_Mappoint_Text = selected_site.Site_Name;
                        point_noPic.Font_Size = 8;
                        point_noPic.User_Mappoint_Postion.Latitude = selected_site.Latitude;
                        point_noPic.User_Mappoint_Postion.Longitude = selected_site.Longitude;
                        point_noPic.User_Mappoint_Visibility = false;
                        point_noPic.Font_Color = System.Windows.Media.Colors.Blue;
                        point_noPic.Font_Family = "Arial";
                        point_noPic.MapPoint_Simple_Color = System.Windows.Media.Colors.Red;
                        point_noPic.MapPoint_Simple_Style = 0;
                        point_noPic.SimpleMarkerSymbol_Size = 0;
                        point_noPic.Text_XOffset = -1;
                        point_noPic.Text_YOffset = -20;

                        if (point.User_Mappoint_Postion.Latitude != 0 && point.User_Mappoint_Postion.Longitude != 0)
                        {
                            map_points.Add(point);

                            mapp_points_V2.Add(point_noPic);

                            IDs.Add(selected_site.Site_ID);
                            IDS2.Add(selected_site.Site_ID);
                        }
                    }

                    //text
                    Esri_control._Esri_MAP_Control.Add_Radios_GraphicOverlay_Layer(Code.Singleton.Radio_layer, mapp_points_V2, IDS2);
                    //pictures
                    Esri_control._Esri_MAP_Control.Add_Radios_GraphicOverlay_Layer(Code.Singleton.Site_layer, map_points, IDs);

                }
            }
        }
        #endregion

        #region Other Functions
        public void show_Site_info_onMap_panel(Sites site)
        {
            lbl_Sitename_pnl_Site_onMap.Text = site.Site_Name;
            lbl_SiteID_pnl_Site_onMap.Text = site.Site_ID.ToString();
            lbl_type_pnl_Site_onMap.Text = site.Site_Type;
            lbl_zoneID_pnl_Site_onMap.Text = site.Zone_ID.ToString();
            lbl_power_pnl_Site_onMap.Text = site.Site_Power.ToString();
            lbl_height_pnl_Site_onMap.Text = site.Site_Height.ToString();
            lbl_latt_pnl_Site_onMap.Text = site.Latitude.ToString();
            lbl_longt_pnl_Site_onMap.Text = site.Longitude.ToString();
            lbl_LA_pnl_Site_onMap.Text = site.LA;
        }
        public void show_Radio_info_onMap_panel(Radios radio)
        {
            lbl_txt_Radioname_pnl_Radio_onMap.Text = radio.Radio_Name;
            lbl_txt_ISSI_pnl_Radio_onMap.Text = radio.ISSI.ToString();
            lbl_txt_lattitude_pnl_Radio_onMap.Text = radio.gpsPosition.Latitude.ToString();
            lbl_txt_longtitude_pnl_Radio_onMap.Text = radio.gpsPosition.Longitude.ToString();
            lbl_txt_radioID_pnl_Radio_onMap.Text = radio.Radio_ID.ToString();
            lbl_txt_SiteID_pnl_Radio_onMap.Text = radio.Site_ID.ToString();
            lbl_txt_type_pnl_Radio_onMap.Text = radio.Radio_Type;
            foreach (Sites site in sites)
            {
                if (site.Site_ID == radio.Site_ID)
                {
                    lbl_txt_sitename_pnl_Radio_onMap.Text = site.Site_Name;
                    break;
                }
            }
        }
        public void show_Coverage_point_info_onmap_panel(Coverage_info info)
        {
            txt_lbl_sitename_coverage_pnl.Text = info.Site_Name;
            txt_llbl_issi_coverage_pnl.Text = info.ISSI.ToString();
            txt_lbl_dbm_coverage_pnl.Text = info.dbm_value;
        }
        #endregion

        #endregion

        #region Draw Covarage Panel
        public string Site_name;

        #region drawing data collection
        public struct Drawing_Data
        {
            public System.Drawing.Color picturecolour;
            public System.Windows.Media.Color color;
            public int RSSI;
            public string drawing_layername;
            public List<User_Simple_MapPoint> picture_points_collection;
            public List<MapPoint_Position> polygon_points;
            public List<double> placemarks_IDs;
            public string RSSI_to_DBM_value;
        }
        public Drawing_Data[] Drawing_data_collection = new Drawing_Data[32];
        public string convert_rssi_to_dbm_value(int rssi)
        {
            foreach (Drawing_Data Data_collection in Drawing_data_collection)
            {
                if (rssi == Data_collection.RSSI)
                {
                    return Data_collection.RSSI_to_DBM_value;
                }
            }
            return "";
        }
        public void initialize_drawing_data_collection_array()
        {
            #region rssi 0
            Drawing_data_collection[0].picturecolour = Color.DimGray;
            Drawing_data_collection[0].color = System.Windows.Media.Color.FromRgb(105, 105, 105);
            Drawing_data_collection[0].drawing_layername = Color.DimGray.ToString() + "_points";
            Drawing_data_collection[0].RSSI = 0;
            Drawing_data_collection[0].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[0].placemarks_IDs = new List<double>();
            Drawing_data_collection[0].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[0].RSSI_to_DBM_value = "-113 Dbm or less";
            #endregion
            #region rssi 1
            Drawing_data_collection[1].picturecolour = Color.White;
            Drawing_data_collection[1].color = /**/ System.Windows.Media.Color.FromRgb(255, 255, 255);
            Drawing_data_collection[1].drawing_layername = Color.White.ToString() + "_points";
            Drawing_data_collection[1].RSSI = 1;
            Drawing_data_collection[1].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[1].placemarks_IDs = new List<double>();
            Drawing_data_collection[1].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[1].RSSI_to_DBM_value = "-111 to -112 dBm";
            #endregion
            #region rssi 2   
            Drawing_data_collection[2].picturecolour = Color.LightGray;
            Drawing_data_collection[2].color = /**/ System.Windows.Media.Color.FromRgb(211, 211, 211);
            Drawing_data_collection[2].drawing_layername = Color.LightGray.ToString() + "_points";
            Drawing_data_collection[2].RSSI = 2;
            Drawing_data_collection[2].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[2].placemarks_IDs = new List<double>();
            Drawing_data_collection[2].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[2].RSSI_to_DBM_value = "-110 to -109 dBm";

            #endregion
            #region rssi 3           
            Drawing_data_collection[3].picturecolour = Color.LightPink;
            Drawing_data_collection[3].color = /*;*/ System.Windows.Media.Color.FromRgb(255, 182, 193);
            Drawing_data_collection[3].drawing_layername = Color.LightPink.ToString() + "_points";
            Drawing_data_collection[3].RSSI = 3;
            Drawing_data_collection[3].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[3].placemarks_IDs = new List<double>();
            Drawing_data_collection[3].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[3].RSSI_to_DBM_value = "-108 to -107 dBm";

            #endregion
            #region rssi 4           
            Drawing_data_collection[4].picturecolour = Color.Red;
            Drawing_data_collection[4].color = /**/ System.Windows.Media.Color.FromRgb(255, 0, 0);
            Drawing_data_collection[4].drawing_layername = Color.Red.ToString() + "_points";
            Drawing_data_collection[4].RSSI = 4;
            Drawing_data_collection[4].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[4].placemarks_IDs = new List<double>();
            Drawing_data_collection[4].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[4].RSSI_to_DBM_value = "-106 to -105 dBm";
            #endregion
            #region rssi 5           
            Drawing_data_collection[5].picturecolour = Color.LightSalmon;
            Drawing_data_collection[5].color = /**/System.Windows.Media.Color.FromRgb(255, 160, 122);
            Drawing_data_collection[5].drawing_layername = Color.LightSalmon.ToString() + "_points";
            Drawing_data_collection[5].RSSI = 5;
            Drawing_data_collection[5].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[5].placemarks_IDs = new List<double>();
            Drawing_data_collection[5].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[5].RSSI_to_DBM_value = "-104 to -103 dBm";
            #endregion
            #region rssi 6            
            Drawing_data_collection[6].picturecolour = Color.Orange;
            Drawing_data_collection[6].color = /**/System.Windows.Media.Color.FromRgb(255, 165, 0);
            Drawing_data_collection[6].drawing_layername = Color.Orange.ToString() + "_points";
            Drawing_data_collection[6].RSSI = 6;
            Drawing_data_collection[6].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[6].placemarks_IDs = new List<double>();
            Drawing_data_collection[6].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[6].RSSI_to_DBM_value = "-102 to -101 dBm";
            #endregion
            #region rssi 7        
            Drawing_data_collection[7].picturecolour = Color.DarkOrange;
            Drawing_data_collection[7].color = /**/System.Windows.Media.Color.FromRgb(255, 140, 0);
            Drawing_data_collection[7].drawing_layername = Color.DarkOrange.ToString() + "_points";
            Drawing_data_collection[7].RSSI = 7;
            Drawing_data_collection[7].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[7].placemarks_IDs = new List<double>();
            Drawing_data_collection[7].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[7].RSSI_to_DBM_value = "-100 to -99 dBm";
            #endregion
            #region rssi 8            
            Drawing_data_collection[8].picturecolour = Color.Tan;
            Drawing_data_collection[8].color = /*Color.Tan*/ System.Windows.Media.Color.FromRgb(210, 180, 140);
            Drawing_data_collection[8].drawing_layername = Color.Tan.ToString() + "_points";
            Drawing_data_collection[8].RSSI = 8;
            Drawing_data_collection[8].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[8].placemarks_IDs = new List<double>();
            Drawing_data_collection[8].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[8].RSSI_to_DBM_value = "-98 to -97 dBm";

            #endregion
            #region rssi 9            
            Drawing_data_collection[9].picturecolour = Color.Bisque;
            Drawing_data_collection[9].color = /**/System.Windows.Media.Color.FromRgb(255, 228, 196);
            Drawing_data_collection[9].drawing_layername = Color.Bisque.ToString() + "_points";
            Drawing_data_collection[9].RSSI = 9;
            Drawing_data_collection[9].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[9].placemarks_IDs = new List<double>();
            Drawing_data_collection[9].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[9].RSSI_to_DBM_value = "-96 to -95 dBm";

            #endregion
            #region rssi 10
            Drawing_data_collection[10].picturecolour = Color.Cornsilk;
            Drawing_data_collection[10].color = /**/ System.Windows.Media.Color.FromRgb(255, 248, 220);
            Drawing_data_collection[10].drawing_layername = Color.Cornsilk.ToString() + "_points";
            Drawing_data_collection[10].RSSI = 10;
            Drawing_data_collection[10].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[10].placemarks_IDs = new List<double>();
            Drawing_data_collection[10].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[10].RSSI_to_DBM_value = "-94 to -93 dBm";
            #endregion
            #region rssi 11           
            Drawing_data_collection[11].picturecolour = Color.DarkGoldenrod;
            Drawing_data_collection[11].color = /**/ System.Windows.Media.Color.FromRgb(184, 143, 11);
            Drawing_data_collection[11].drawing_layername = Color.DarkGoldenrod.ToString() + "_points";
            Drawing_data_collection[11].RSSI = 11;
            Drawing_data_collection[11].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[11].placemarks_IDs = new List<double>();
            Drawing_data_collection[11].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[11].RSSI_to_DBM_value = "-92 to -91 dBm";
            #endregion
            #region rssi 12  
            Drawing_data_collection[12].picturecolour = Color.LawnGreen;
            Drawing_data_collection[12].color = /**/System.Windows.Media.Color.FromRgb(124, 252, 0);
            Drawing_data_collection[12].drawing_layername = Color.LawnGreen.ToString() + "_points";
            Drawing_data_collection[12].RSSI = 12;
            Drawing_data_collection[12].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[12].placemarks_IDs = new List<double>();
            Drawing_data_collection[12].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[12].RSSI_to_DBM_value = "-90 to -89 dBm";

            #endregion
            #region rssi 13            
            Drawing_data_collection[13].picturecolour = Color.LimeGreen;
            Drawing_data_collection[13].color = /**/System.Windows.Media.Color.FromRgb(50, 205, 50);
            Drawing_data_collection[13].drawing_layername = Color.LimeGreen.ToString() + "_points";
            Drawing_data_collection[13].RSSI = 13;
            Drawing_data_collection[13].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[13].placemarks_IDs = new List<double>();
            Drawing_data_collection[13].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[13].RSSI_to_DBM_value = "-88 to -87 dBm";

            #endregion
            #region rssi 14           
            Drawing_data_collection[14].picturecolour = Color.Green;
            Drawing_data_collection[14].color = /*Color.Green*/ System.Windows.Media.Color.FromRgb(0, 255, 0);
            Drawing_data_collection[14].drawing_layername = Color.Green.ToString() + "_points";
            Drawing_data_collection[14].RSSI = 14;
            Drawing_data_collection[14].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[14].placemarks_IDs = new List<double>();
            Drawing_data_collection[14].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[14].RSSI_to_DBM_value = "-86 to -85 dBm";

            #endregion
            #region rssi 15            
            Drawing_data_collection[15].picturecolour = Color.MediumSeaGreen;
            Drawing_data_collection[15].color = /**/ System.Windows.Media.Color.FromRgb(60, 179, 113);
            Drawing_data_collection[15].drawing_layername = Color.MediumSeaGreen.ToString() + "_points";
            Drawing_data_collection[15].RSSI = 15;
            Drawing_data_collection[15].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[15].placemarks_IDs = new List<double>();
            Drawing_data_collection[15].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[15].RSSI_to_DBM_value = "-84 to -83 dBm";

            #endregion
            #region rssi 16            
            Drawing_data_collection[16].picturecolour = Color.Turquoise;
            Drawing_data_collection[16].color = /**/ System.Windows.Media.Color.FromRgb(64, 224, 208);
            Drawing_data_collection[16].drawing_layername = Color.Turquoise.ToString() + "_points";
            Drawing_data_collection[16].RSSI = 16;
            Drawing_data_collection[16].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[16].placemarks_IDs = new List<double>();
            Drawing_data_collection[16].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[16].RSSI_to_DBM_value = "-82 to -81 dBm";

            #endregion
            #region rssi 17        
            Drawing_data_collection[17].picturecolour = Color.DeepSkyBlue;
            Drawing_data_collection[17].color = /**/ System.Windows.Media.Color.FromRgb(0, 191, 255);
            Drawing_data_collection[17].drawing_layername = Color.DeepSkyBlue.ToString() + "_points";
            Drawing_data_collection[17].RSSI = 17;
            Drawing_data_collection[17].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[17].placemarks_IDs = new List<double>();
            Drawing_data_collection[17].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[17].RSSI_to_DBM_value = "-80 to -79 dBm";

            #endregion
            #region rssi 18            
            Drawing_data_collection[18].picturecolour = Color.BlueViolet;
            Drawing_data_collection[18].color = System.Windows.Media.Color.FromRgb(138, 43, 226); /**/
            Drawing_data_collection[18].drawing_layername = Color.BlueViolet.ToString() + "_points";
            Drawing_data_collection[18].RSSI = 18;
            Drawing_data_collection[18].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[18].placemarks_IDs = new List<double>();
            Drawing_data_collection[18].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[18].RSSI_to_DBM_value = "-78 to -77 dBm";

            #endregion
            #region rssi 19          
            Drawing_data_collection[19].picturecolour = Color.Plum;
            Drawing_data_collection[19].color =/**/ System.Windows.Media.Color.FromRgb(221, 160, 221);
            Drawing_data_collection[19].drawing_layername = Color.Plum.ToString() + "_points";
            Drawing_data_collection[19].RSSI = 19;
            Drawing_data_collection[19].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[19].placemarks_IDs = new List<double>();
            Drawing_data_collection[19].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[19].RSSI_to_DBM_value = "-76 to -75 dBm";

            #endregion
            #region rssi 20
            Drawing_data_collection[20].picturecolour = Color.Violet;
            Drawing_data_collection[20].color =/**/System.Windows.Media.Color.FromRgb(127, 0, 255);
            Drawing_data_collection[20].drawing_layername = Color.Violet.ToString() + "_points";
            Drawing_data_collection[20].RSSI = 20;
            Drawing_data_collection[20].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[20].placemarks_IDs = new List<double>();
            Drawing_data_collection[20].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[20].RSSI_to_DBM_value = "-74 to -73 dBm";

            #endregion
            #region rssi 21           
            Drawing_data_collection[21].picturecolour = Color.Pink;
            Drawing_data_collection[21].color =/**/System.Windows.Media.Color.FromRgb(255, 192, 203);
            Drawing_data_collection[21].drawing_layername = Color.Pink.ToString() + "_points";
            Drawing_data_collection[21].RSSI = 21;
            Drawing_data_collection[21].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[21].placemarks_IDs = new List<double>();
            Drawing_data_collection[21].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[21].RSSI_to_DBM_value = "-72 to -71 dBm";

            #endregion
            #region rssi 22     
            Drawing_data_collection[22].picturecolour = Color.Yellow;
            Drawing_data_collection[22].color =/**/System.Windows.Media.Color.FromRgb(255, 255, 0);
            Drawing_data_collection[22].drawing_layername = Color.Yellow.ToString() + "_points";
            Drawing_data_collection[22].RSSI = 22;
            Drawing_data_collection[22].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[22].placemarks_IDs = new List<double>();
            Drawing_data_collection[22].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[22].RSSI_to_DBM_value = "-70 to -69 dBm";

            #endregion
            #region rssi 23            
            Drawing_data_collection[23].picturecolour = Color.DarkSalmon;
            Drawing_data_collection[23].color =/**/System.Windows.Media.Color.FromRgb(233, 150, 122);
            Drawing_data_collection[23].drawing_layername = Color.DarkSalmon.ToString() + "_points";
            Drawing_data_collection[23].RSSI = 23;
            Drawing_data_collection[23].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[23].placemarks_IDs = new List<double>();
            Drawing_data_collection[23].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[23].RSSI_to_DBM_value = "-68 to -67 dBm";

            #endregion
            #region rssi 24           
            Drawing_data_collection[24].picturecolour = Color.Blue;
            Drawing_data_collection[24].color = /**/System.Windows.Media.Color.FromRgb(0, 0, 255);
            Drawing_data_collection[24].drawing_layername = Color.Blue.ToString() + "_points";
            Drawing_data_collection[24].RSSI = 24;
            Drawing_data_collection[24].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[24].placemarks_IDs = new List<double>();
            Drawing_data_collection[24].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[24].RSSI_to_DBM_value = "-66 to -65 dBm";

            #endregion
            #region rssi 25            
            Drawing_data_collection[25].picturecolour = Color.Orchid;
            Drawing_data_collection[25].color =/**/System.Windows.Media.Color.FromRgb(218, 112, 214);
            Drawing_data_collection[25].drawing_layername = Color.Orchid.ToString() + "_points";
            Drawing_data_collection[25].RSSI = 25;
            Drawing_data_collection[25].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[25].placemarks_IDs = new List<double>();
            Drawing_data_collection[25].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[25].RSSI_to_DBM_value = "-64 to -63 dBm";

            #endregion
            #region rssi 26          
            Drawing_data_collection[26].picturecolour = Color.PaleVioletRed;
            Drawing_data_collection[26].color =/**/System.Windows.Media.Color.FromRgb(219, 112, 147);
            Drawing_data_collection[26].drawing_layername = Color.PaleVioletRed.ToString() + "_points";
            Drawing_data_collection[26].RSSI = 26;
            Drawing_data_collection[26].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[26].placemarks_IDs = new List<double>();
            Drawing_data_collection[26].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[26].RSSI_to_DBM_value = "-62 to -61 dBm";

            #endregion
            #region rssi 27      
            Drawing_data_collection[27].picturecolour = Color.DarkViolet;
            Drawing_data_collection[27].color =/**/System.Windows.Media.Color.FromRgb(148, 0, 211);
            Drawing_data_collection[27].drawing_layername = Color.DarkViolet.ToString() + "_points";
            Drawing_data_collection[27].RSSI = 27;
            Drawing_data_collection[27].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[27].placemarks_IDs = new List<double>();
            Drawing_data_collection[27].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[27].RSSI_to_DBM_value = "-60 to -59 dBm";

            #endregion
            #region rssi 28          
            Drawing_data_collection[28].picturecolour = Color.Fuchsia;
            Drawing_data_collection[28].color =/**/System.Windows.Media.Color.FromRgb(255, 0, 255);
            Drawing_data_collection[28].drawing_layername = Color.Fuchsia.ToString() + "_points";
            Drawing_data_collection[28].RSSI = 28;
            Drawing_data_collection[28].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[28].placemarks_IDs = new List<double>();
            Drawing_data_collection[28].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[28].RSSI_to_DBM_value = "-58 to -57 dBm";

            #endregion
            #region rssi 29           
            Drawing_data_collection[29].picturecolour = Color.Crimson;
            Drawing_data_collection[29].color =/**/System.Windows.Media.Color.FromRgb(157, 34, 53);
            Drawing_data_collection[29].drawing_layername = Color.Crimson.ToString() + "_points";
            Drawing_data_collection[29].RSSI = 29;
            Drawing_data_collection[29].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[29].placemarks_IDs = new List<double>();
            Drawing_data_collection[29].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[29].RSSI_to_DBM_value = "-56 to -55 dBm";

            #endregion
            #region rssi 30            
            Drawing_data_collection[30].picturecolour = Color.MediumPurple;
            Drawing_data_collection[30].color =/**/System.Windows.Media.Color.FromRgb(147, 112, 219);
            Drawing_data_collection[30].drawing_layername = Color.MediumPurple.ToString() + "_points";
            Drawing_data_collection[30].RSSI = 30;
            Drawing_data_collection[30].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[30].placemarks_IDs = new List<double>();
            Drawing_data_collection[30].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[30].RSSI_to_DBM_value = "-54 to -53 dBm";

            #endregion
            #region rssi 31          
            Drawing_data_collection[31].picturecolour = Color.DarkSlateBlue;
            Drawing_data_collection[31].color =/**/System.Windows.Media.Color.FromRgb(72, 61, 139);
            Drawing_data_collection[31].drawing_layername = Color.DarkSlateBlue.ToString() + "_points";
            Drawing_data_collection[31].RSSI = 31;
            Drawing_data_collection[31].picture_points_collection = new List<User_Simple_MapPoint>();
            Drawing_data_collection[31].placemarks_IDs = new List<double>();
            Drawing_data_collection[31].polygon_points = new List<MapPoint_Position>();
            Drawing_data_collection[31].RSSI_to_DBM_value = "-52 dBm or more";
            #endregion
        }
        public void initialize_picture_boxes_colors()
        {

            pic_c_1.BackColor = Drawing_data_collection[0].picturecolour;
            pic_c_2.BackColor = Drawing_data_collection[1].picturecolour;
            pic_c_3.BackColor = Drawing_data_collection[2].picturecolour;
            pic_c_4.BackColor = Drawing_data_collection[3].picturecolour;
            pic_c_5.BackColor = Drawing_data_collection[4].picturecolour;
            pic_c_6.BackColor = Drawing_data_collection[5].picturecolour;
            pic_c_7.BackColor = Drawing_data_collection[6].picturecolour;
            pic_c_8.BackColor = Drawing_data_collection[7].picturecolour;
            pic_c_9.BackColor = Drawing_data_collection[8].picturecolour;
            pic_c_10.BackColor = Drawing_data_collection[9].picturecolour;
            pic_c_11.BackColor = Drawing_data_collection[10].picturecolour;
            pic_c_12.BackColor = Drawing_data_collection[11].picturecolour;
            pic_c_13.BackColor = Drawing_data_collection[12].picturecolour;
            pic_c_14.BackColor = Drawing_data_collection[13].picturecolour;
            pic_c_15.BackColor = Drawing_data_collection[14].picturecolour;
            pic_c_16.BackColor = Drawing_data_collection[15].picturecolour;
            pic_c_17.BackColor = Drawing_data_collection[16].picturecolour;
            pic_c_18.BackColor = Drawing_data_collection[17].picturecolour;
            pic_c_19.BackColor = Drawing_data_collection[18].picturecolour;
            pic_c_20.BackColor = Drawing_data_collection[19].picturecolour;
            pic_c_21.BackColor = Drawing_data_collection[20].picturecolour;
            pic_c_22.BackColor = Drawing_data_collection[21].picturecolour;
            pic_c_23.BackColor = Drawing_data_collection[22].picturecolour;
            pic_c_24.BackColor = Drawing_data_collection[23].picturecolour;
            pic_c_25.BackColor = Drawing_data_collection[24].picturecolour;
            pic_c_26.BackColor = Drawing_data_collection[25].picturecolour;
            pic_c_27.BackColor = Drawing_data_collection[26].picturecolour;
            pic_c_28.BackColor = Drawing_data_collection[27].picturecolour;
            pic_c_29.BackColor = Drawing_data_collection[28].picturecolour;
            pic_c_30.BackColor = Drawing_data_collection[29].picturecolour;
            pic_c_31.BackColor = Drawing_data_collection[30].picturecolour;
            pic_c_32.BackColor = Drawing_data_collection[31].picturecolour;


        }


        #endregion

        #region old drawing zone
        public struct Draw_zone
        {
            public Color zone_color;
            public int Rssi_value1;
            public int Rssi_value2;
            public string zone_layername;
            public string Picture_path;
            //for drawing pic points from logs
            public List<User_Picture_MapPoint> picture_points_collection;
            //for drawing polygons from logs
            public List<MapPoint_Position> map_points;
            public List<double> zone_placemark_Ids;
            public string rssi_to_dbm_value;
        };
        public Draw_zone[] Drawing_zones = new Draw_zone[19];
        public string[] Dbm_values = new string[19];
        public delegate void initialize_drawing_zones_delegate();
        public static string[] color_pics_path = new string[19];

        public void load_dbm_values()
        {
            Dbm_values[0] = "-113 or less dBm";
            Dbm_values[1] = "-111 to -112 dBm";
            Dbm_values[2] = "-109 to -110 dBm";
            Dbm_values[3] = "-107 to -108 dBm";
            Dbm_values[4] = "-104 to --106 dBm";
            Dbm_values[17] = "-100 to -102 dBm";
            Dbm_values[6] = "-96 to -98 dBm";
            Dbm_values[7] = "-92 to -94 dBm";
            Dbm_values[8] = "-88 to -90 dBm";
            Dbm_values[9] = "-84 to -86 dBm";
            Dbm_values[10] = "-80 to -82 dBm";
            Dbm_values[11] = "-76 to -78 dBm";
            Dbm_values[12] = "-72 to -74 dBm ";
            Dbm_values[13] = "-68 to -70 dBm";
            Dbm_values[14] = "-64 to -66 dBm";
            Dbm_values[117] = "-60 to -62 dBm";
            Dbm_values[16] = "-176 to -178 dBm";
            Dbm_values[17] = "-173 to -174 dBm";
            Dbm_values[18] = "-171 or more dBm";
        }
        public void load_color_pics()
        {
            color_pics_path[0] =
            color_pics_path[1] =
            color_pics_path[2] =
            color_pics_path[3] =
            color_pics_path[4] =
            color_pics_path[17] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_6.png";
            color_pics_path[6] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_7.png";
            color_pics_path[7] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_8.png";
            color_pics_path[8] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_9.png";
            color_pics_path[9] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_10.png";
            color_pics_path[10] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_11.png";
            color_pics_path[11] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_12.png";
            color_pics_path[12] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_13.png";
            color_pics_path[13] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_14.png";
            color_pics_path[14] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_15.png";
            color_pics_path[117] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_16.png";
            color_pics_path[16] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_17.png";
            color_pics_path[17] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_18.png";
            color_pics_path[18] = AppDomain.CurrentDomain.BaseDirectory + @"Files\Icons\C_19.png";
        }
        public void initialize_drawing_zones()
        {
            try
            {
                if (tableLayoutPanel1.InvokeRequired)
                {
                    initialize_drawing_zones_delegate del = new initialize_drawing_zones_delegate(initialize_drawing_zones);
                    tableLayoutPanel1.Invoke(del);
                }
                else
                {
                    foreach (System.Windows.Forms.Control pic_box in tableLayoutPanel1.Controls)
                    {
                        if (pic_box.Tag.ToString() != "lbl" && pic_box.Tag.ToString() != "table")
                        {
                            for (int loop = 0; loop < Drawing_zones.Length; loop++)
                            {
                                Drawing_zones[loop].map_points = new List<MapPoint_Position>();
                                Drawing_zones[loop].zone_placemark_Ids = new List<double>();
                                Drawing_zones[loop].picture_points_collection = new List<User_Picture_MapPoint>();


                                if (pic_box.AccessibleName != "over_color")
                                {
                                    if (loop == Convert.ToInt16(pic_box.AccessibleName))
                                    {
                                        int rssi = Convert.ToInt16(pic_box.Tag);
                                        if (rssi <= 4)
                                        {
                                            Drawing_zones[loop].Rssi_value1 = rssi;
                                            Drawing_zones[loop].Rssi_value2 = rssi;
                                        }
                                        else
                                        {
                                            Drawing_zones[loop].Rssi_value1 = rssi;
                                            if (loop == 18)
                                            {
                                                Drawing_zones[loop].Rssi_value2 = rssi + 2;
                                            }
                                            else
                                            {
                                                Drawing_zones[loop].Rssi_value2 = rssi + 1;
                                            }
                                        }
                                        Drawing_zones[loop].zone_color = pic_box.BackColor;
                                        Drawing_zones[loop].Picture_path = color_pics_path[loop];
                                        //
                                        //Drawing_zones[loop].rssi_to_dbm_value = Dbm_values[rssi_to_dbm(rssi)];
                                        Drawing_zones[loop].zone_layername = pic_box.BackColor.ToString() + "_points";

                                    }
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        #endregion

        public List<double> ids = new List<double>();

        public DateTime start_date;
        public DateTime end_date;
        public string radio_type;
        public int siteid = 0;
        public struct Coverage_info
        {
            public string Site_Name;
            public int ISSI;
            public float RSSI;
            public int PosID;
            public string dbm_value;
        }
        public List<Coverage_info> Coverage_points_info = new List<Coverage_info>();

        #region events
        private void btn_view_coverage_all_sites_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (comboBox_coverage_draw_method_Map_pnl.Text == "Polygons")
                {
                    if (comboBox_Choose_site_coverage.Text == "All Sites" || comboBox_Choose_site_coverage.SelectedIndex == 0)
                    {
                        #region clear previous drawing
                        Esri_control.Delete_All_Zones();
                        foreach (Draw_zone zone in Drawing_zones)
                        {
                            Esri_control.Invisable_GraphicOverlay_Layer(zone.zone_layername);
                            Esri_control.Clear_GraphicOverlay_Layer(zone.zone_layername);
                            zone.map_points.Clear();

                            zone.picture_points_collection.Clear();

                        }
                        #endregion
                        radio_type = comboBox_Radio_type_Map_pnl.Text;
                        start_date = start_Date.Value;
                        end_date = End_Date.Value;
                        Thread Draw_coverage_thread = new Thread(draw_coverage_for_all_sites_as_polygon);
                        Draw_coverage_thread.Start();
                    }
                    else if (!string.IsNullOrEmpty(comboBox_Choose_site_coverage.Text))
                    {
                        #region clear previous drawing
                        Esri_control.Delete_All_Zones();
                        foreach (Draw_zone zone in Drawing_zones)
                        {
                            Esri_control.Invisable_GraphicOverlay_Layer(zone.zone_layername);
                            Esri_control.Clear_GraphicOverlay_Layer(zone.zone_layername);
                            zone.map_points.Clear();
                            zone.zone_placemark_Ids.Clear();
                            zone.picture_points_collection.Clear();
                        }
                        #endregion
                        Site_name = comboBox_Choose_site_coverage.Text;
                        radio_type = comboBox_Radio_type_Map_pnl.Text;
                        start_date = start_Date.Value;
                        end_date = End_Date.Value;
                        foreach (Sites site in sites)
                        {
                            if (site.Site_Name == Site_name)
                            {
                                siteid = site.Site_ID;
                                break;
                            }
                        }
                        Thread Draw_coverage_thread = new Thread(coverage_draw_one_Site_as_polygon);
                        Draw_coverage_thread.Start();
                    }

                }
                else if (comboBox_coverage_draw_method_Map_pnl.Text == "Points")
                {
                    if (comboBox_Choose_site_coverage.Text == "All Sites" || comboBox_Choose_site_coverage.SelectedIndex == 0)
                    {

                        #region clear previous drawing 
                        Coverage_points_info.Clear();
                        Esri_control.Delete_All_Zones();
                        foreach (Draw_zone zone in Drawing_zones)
                        {
                            Esri_control.Invisable_GraphicOverlay_Layer(zone.zone_layername);
                            Esri_control.Clear_GraphicOverlay_Layer(zone.zone_layername);
                            zone.map_points.Clear();
                            zone.zone_placemark_Ids.Clear();
                            zone.picture_points_collection.Clear();
                        }
                        #endregion
                        radio_type = comboBox_Radio_type_Map_pnl.Text;
                        start_date = start_Date.Value;
                        end_date = End_Date.Value;
                        Thread Draw_coverage_thread = new Thread(draw_coverage_for_all_sites_as_points);

                        Draw_coverage_thread.Start();
                    }
                    else if (!string.IsNullOrEmpty(comboBox_Choose_site_coverage.Text) || comboBox_Choose_site_coverage.SelectedIndex != 0)
                    {

                        #region clear previous drawing
                        Esri_control.Delete_All_Zones();
                        foreach (Draw_zone zone in Drawing_zones)
                        {
                            Esri_control.Invisable_GraphicOverlay_Layer(zone.zone_layername);
                            Esri_control.Clear_GraphicOverlay_Layer(zone.zone_layername);
                            zone.map_points.Clear();
                            zone.zone_placemark_Ids.Clear();
                            zone.picture_points_collection.Clear();
                        }
                        #endregion

                        Site_name = comboBox_Choose_site_coverage.Text;
                        radio_type = comboBox_Radio_type_Map_pnl.Text;
                        start_date = start_Date.Value;
                        end_date = End_Date.Value;
                        foreach (Sites site in sites)
                        {
                            if (site.Site_Name == Site_name)
                            {
                                siteid = site.Site_ID;
                                break;
                            }
                        }
                        Thread Draw_coverage_thread = new Thread(coverage_draw_one_Site_as_points);
                        Draw_coverage_thread.Start();
                    }

                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        private void Btn_Clear_coverage_Click(object sender, EventArgs e)
        {
            Esri_control.Delete_All_Zones();
            foreach (Draw_zone zone in Drawing_zones)
            {
                Esri_control.Invisable_GraphicOverlay_Layer(zone.zone_layername);
                Esri_control.Clear_GraphicOverlay_Layer(zone.zone_layername);
                zone.map_points.Clear();
                zone.zone_placemark_Ids.Clear();
                zone.picture_points_collection.Clear();
            }

        }

        #endregion

        #region Delagate
        public delegate void coverage_draw_delegate();

        public delegate void coverage_draw_for_all_sites(List<Logs> logs);
        public delegate void coverage_draw_for_one_site(List<Logs> logs);

        public void coverage_draw_one_Site_as_polygon()
        {
            try
            {
                drawing_loading_bar_thread();
                if (siteid > 0)
                {
                    List<Logs> logs_list = WS_Control_obj.Get_logs_filtered_by_datetime_SiteID_radioType(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, start_date, end_date, radio_type, siteid).ToList<Logs>();
                    if (pnl_Map.InvokeRequired)
                    {
                        pnl_Map.Invoke(new coverage_draw_for_one_site(drawing_coverage_polygons), new object[] { logs_list });
                    }
                    else
                    {
                        drawing_coverage_polygons(logs_list);
                    }
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public void draw_coverage_for_all_sites_as_polygon()
        {
            drawing_loading_bar_thread();
            try
            {
                List<Logs> logs_list = WS_Control_obj.Get_logs_filtered_by_datetime_radioType(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, start_date, end_date, radio_type).ToList<Logs>();
                if (pnl_Map.InvokeRequired)
                {
                    pnl_Map.Invoke(new coverage_draw_for_all_sites(drawing_coverage_polygons), new object[] { logs_list });
                }
                else
                {
                    drawing_coverage_polygons(logs_list);
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public void coverage_draw_one_Site_as_points()
        {
            drawing_loading_bar_thread();
            try
            {
                if (siteid > 0)
                {
                    List<Logs>logs_list = WS_Control_obj.Get_logs_filtered_by_datetime_SiteID_radioType(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, start_date, end_date, radio_type, siteid).ToList<Logs>();
                    if (pnl_Map.InvokeRequired)
                    {
                        pnl_Map.Invoke(new coverage_draw_for_one_site(drawing_coverage_points), new object[] { logs_list });
                    }
                    else
                    {
                        drawing_coverage_points(logs_list);
                    }
                }
                workdone = true;

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public void draw_coverage_for_all_sites_as_points()
        {
            drawing_loading_bar_thread();
            try
            {
                #region get filtered logs
                List<Logs> logs_list = WS_Control_obj.Get_logs_filtered_by_datetime_radioType(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, start_date, end_date, radio_type).ToList<Logs>();
                List<Logs>filtered_logs_list = new List<Logs>();
                int current_pos_id = 0;
                foreach (Logs log in logs_list)
                {
                    if (current_pos_id != log.Pos_ID)
                    {
                        Logs filtered_log = WS_Control_obj.Get_one_log_filtered_by_Pos_ID_with_max_rssi(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, log.Pos_ID);
                        current_pos_id = log.Pos_ID;
                        filtered_logs_list.Add(filtered_log);
                    }
                }
                #endregion

                if (pnl_Map.InvokeRequired)
                {
                    pnl_Map.Invoke(new coverage_draw_for_all_sites(drawing_coverage_points), new object[] { filtered_logs_list });
                }
                else
                {
                    drawing_coverage_points(filtered_logs_list);
                }
                workdone = true;

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        public delegate void load_comboBox_Choose_site_coverage_delegate();
        public void load_comboBox_Choose_site_for_coverage()
        {
            if (comboBox_Choose_site_coverage.InvokeRequired)
            {
                load_comboBox_Choose_site_coverage_delegate dels = new load_comboBox_Choose_site_coverage_delegate(load_comboBox_Choose_site_for_coverage);
                comboBox_Choose_site_coverage.Invoke(dels);
            }
            else
            {
                try
                {
                    List<string> Sites_list = new List<string>();
                    Sites_list.Add("All Sites");
                    if (sites != null)
                    {
                        foreach (Sites site in sites)
                        {
                            Sites_list.Add(site.Site_Name);
                        }
                        comboBox_Choose_site_coverage.DataSource = Sites_list;

                    }
                }
                catch (Exception ex)
                {
                    Auditing.Error(ex.Message);
                }
            }
        }



        #region progress bar

        public int drawing_pbar_value = 0;

        public delegate void change_drawing_progress_bar_date_pnl_percentage_delegate(int value);

        public void change_drawing_progress_bar_date_pnl_percentage(int value)
        {
            try
            {
                if (drawing_progress_bar_date_pnl.InvokeRequired)
                {
                    drawing_progress_bar_date_pnl.Invoke(new change_drawing_progress_bar_date_pnl_percentage_delegate(change_drawing_progress_bar_date_pnl_percentage), new object[] { value });
                }
                else
                {
                    drawing_progress_bar_date_pnl.Value = value;
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }


        #endregion

        #endregion

        #region other functions
        public void drawing_coverage_points(List<Logs> filtered_logs)
        {
            try
            {
                if (filtered_logs != null && filtered_logs.Count > 0)
                {
                    foreach (Logs log in filtered_logs)
                    {
                        #region check all drawing layers
                        foreach (Drawing_Data drawing_layer in Drawing_data_collection)
                        {
                            if(drawing_layer.RSSI == log.RSSI)
                            {
                                #region add point to the drawing layer
                                MapPoint_Position mappoint = new MapPoint_Position();
                                User_Simple_MapPoint point = new User_Simple_MapPoint();
                                mappoint.Longitude = log.GPS_Position.Longitude;
                                mappoint.Latitude = log.GPS_Position.Latitude;

                                point.Font_Color = System.Windows.Media.Color.FromRgb(0, 0, 0);
                                point.Font_Size = 10;
                                point.Font_Family = "";
                                point.MapPoint_Simple_Color = drawing_layer.color;
                                point.SimpleMarkerSymbol_Size = 15;
                                point.User_Mappoint_Postion = mappoint;
                                point.User_Mappoint_Visibility = true;
                                point.User_Mappoint_Text = "";

                                drawing_layer.picture_points_collection.Add(point);
                                drawing_layer.placemarks_IDs.Add(log.Pos_ID);
                                #endregion

                                #region add coverage information
                                Coverage_info coverage_obj = new Coverage_info();
                                coverage_obj.RSSI = log.RSSI;
                                coverage_obj.dbm_value = drawing_layer.RSSI_to_DBM_value;
                                coverage_obj.PosID = log.Pos_ID;
                                foreach (Sites site in sites)
                                {
                                    if (log.Site_ID == site.Site_ID)
                                    {
                                        coverage_obj.Site_Name = site.Site_Name;
                                        break;
                                    }
                                }
                                foreach (Radios radio in radios)
                                {
                                    if (log.GPS_Position.Radio_ID == radio.Radio_ID)
                                    {
                                        coverage_obj.ISSI = radio.ISSI;
                                        break;
                                    }
                                }
                                Coverage_points_info.Add(coverage_obj);
                                #endregion
                            }
                        }
                        #endregion                     
                    }
                    #region draw the layers
                    foreach (Drawing_Data draw_layer in Drawing_data_collection)
                    {
                        if(draw_layer.picture_points_collection !=null && draw_layer.picture_points_collection.Count >0)
                        {
                            Esri_control.draw_multiple_color_points(draw_layer.drawing_layername, draw_layer.picture_points_collection, draw_layer.placemarks_IDs);
                        }
                    }
                    #endregion                    
                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }

        }
        public void drawing_coverage_polygons(List<Logs> filtered_logs)
        {
            if (filtered_logs != null)
            {
                foreach (Logs log in filtered_logs)
                {
                    #region check all drawing layers and add points to each layer
                    foreach(Drawing_Data drawing_layer in Drawing_data_collection)
                    {
                        if(drawing_layer.RSSI == log.RSSI)
                        {
                            MapPoint_Position point = new MapPoint_Position();
                            point.Longitude = log.GPS_Position.Longitude;
                            point.Latitude = log.GPS_Position.Latitude;
                            drawing_layer.polygon_points.Add(point);

                        }
                    }
                    #endregion
                }
                #region draw the layers

                foreach(Drawing_Data drawing_layer in Drawing_data_collection)
                {
                    if(drawing_layer.polygon_points != null && drawing_layer.polygon_points.Count > 2)
                    {
                        Esri_control.draw_polygon(drawing_layer.polygon_points, drawing_layer.color.R, drawing_layer.color.G, drawing_layer.color.B);
                    }
                }
                #endregion
            }
        }
        public void drawing_loading_bar_progress()
        {
            while (!workdone)
            {
                Thread.Sleep(500);
                change_drawing_progress_bar_date_pnl_percentage(drawing_pbar_value);
                drawing_pbar_value += 10;
                Thread.Sleep(500);
                if (drawing_pbar_value == 100)
                {
                    drawing_pbar_value = 0;
                }
            }
            if (workdone)
            {
                change_drawing_progress_bar_date_pnl_percentage(100);
            }

        }

        public void drawing_loading_bar_thread()
        {
            try
            {
                workdone = false;
                Thread loading_zone_operation_thread = new Thread(drawing_loading_bar_progress);
                loading_zone_operation_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }

        }
        #endregion

        #endregion

        #region Cities
        City[] Cities;

        #region Events
        #region buttons
        private void btn_Search_Cities_Cities_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Search_City();

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        private void btn_Add_City_Cities_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_CityName_Cities_Tab.Text != "")
                {
                    City new_City = new City();
                    new_City.Info = !string.IsNullOrEmpty(txt_INFO_Cities_Tab.Text) ? txt_INFO_Cities_Tab.Text : null;
                    new_City.CityName = !string.IsNullOrEmpty(txt_CityName_Cities_Tab.Text) ? txt_CityName_Cities_Tab.Text : null;
                    Thread add_City_thread = new Thread(() => Add_City(new_City));
                    add_City_thread.Start();
                }
                else
                {
                    update_Cities_Statues_label(Code.Singleton.Missing_Inserted_Data_Message, Color.Red);
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        private void btn_Edit_City_Cities_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_CityID_Cities_Tab.Text != "" && txt_CityName_Cities_Tab.Text != "")
                {
                    City Updated_City = new City();

                    Updated_City.Info = !string.IsNullOrEmpty(txt_INFO_Cities_Tab.Text) ? txt_INFO_Cities_Tab.Text : null;
                    Updated_City.CityName = !string.IsNullOrEmpty(txt_CityName_Cities_Tab.Text) ? txt_CityName_Cities_Tab.Text : txt_CityID_Cities_Tab.Text;

                    var id = Convert.ToInt32(txt_CityID_Cities_Tab.Text);
                    Updated_City.CityID = id;
                    Thread Edit_City_thread = new Thread(() => Edit_City(Updated_City));
                    Edit_City_thread.Start();
                    workdone = true;

                }
                else
                {
                    update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        private void btn_Delete_City_Cities_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                City city = (City)trv_TETRACities_Cities_Tab.SelectedNode.Tag;
                Thread delete_City_thread = new Thread(() => Delete_City(city.CityID));

                delete_City_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }

        #endregion

        #region treeview
        // show and treeview after select
        private void trv_TETRACities_Cities_Tab_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                #region clear fields
                txt_CityName_Cities_Tab.Text = "";
                txt_CityID_Cities_Tab.Text = "";
                txt_INFO_Cities_Tab.Text = "";
                #endregion
                if (trv_TETRACities_Cities_Tab.SelectedNode.Text != "Cities")
                {
                    if (trv_TETRACities_Cities_Tab.Parent.Text == "Cities")
                    {
                        City selected_City = (City)trv_TETRACities_Cities_Tab.SelectedNode.Tag;
                        txt_CityName_Cities_Tab.Text = selected_City.CityName.ToString();
                        txt_CityID_Cities_Tab.Text = selected_City.CityID.ToString();
                        txt_INFO_Cities_Tab.Text = selected_City.Info;
                    }
                    else
                    {
                        City selected_City = (City)trv_TETRACities_Cities_Tab.SelectedNode.Parent.Tag;
                        txt_CityName_Cities_Tab.Text = selected_City.CityName.ToString();
                        txt_CityID_Cities_Tab.Text = selected_City.CityID.ToString();
                        txt_INFO_Cities_Tab.Text = selected_City.Info;
                    }
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #endregion

        #endregion

        #region Delegates
        #region treeview
        public delegate void load_trv_TETRACities_Cities_Tab_delegate();
        public void load_trv_TETRACities_Cities_Tab()
        {
            try
            {
                if (trv_TETRACities_Cities_Tab.InvokeRequired)
                {
                    load_trv_TETRACities_Cities_Tab_delegate load_Cities_delegate = new load_trv_TETRACities_Cities_Tab_delegate(load_trv_TETRACities_Cities_Tab);
                    trv_TETRACities_Cities_Tab.Invoke(load_Cities_delegate);
                }
                else
                {

                    #region Cities
                    int Cities_length = Cities != null ? Cities.Length : 0;
                    #region Cities add, edit
                    if (Cities_length > 0)
                    {
                        trv_TETRACities_Cities_Tab.BeginUpdate();

                        for (int i = 0; i < Cities_length; i++)
                        {
                            bool Find_Flag = false;
                            for (int j = 0; j < trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Count; j++)
                            {
                                City selected_City = (City)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Tag;
                                if (Cities[i].CityID == selected_City.CityID)
                                {
                                    //edit 
                                    Find_Flag = true;
                                    trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Tag = Cities[i];
                                    trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Text = Cities[i].CityName;
                                    break;
                                }
                            }
                            if (Find_Flag == false)
                            {
                                //add
                                TreeNode new_City_node = new TreeNode();
                                new_City_node.Text = Cities[i].CityName;
                                new_City_node.Tag = Cities[i];
                                trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Add(new_City_node);
                            }
                        }
                        trv_TETRACities_Cities_Tab.EndUpdate();

                    }
                    #endregion

                    #region Cities delete

                    if (Cities_length != 0 && trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Count > 0)
                    {
                        trv_TETRACities_Cities_Tab.BeginUpdate();

                        for (int j = 0; j < trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Count; j++)
                        { // delete operation
                            bool Find_Flag = false;
                            City City_obj = (City)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Tag;
                            for (int i = 0; i < Cities.Length; i++)
                            {
                                if (City_obj.CityID == Cities[i].CityID)
                                {
                                    Find_Flag = true;
                                    break;
                                }
                            }
                            if (Find_Flag == false)
                            {
                                //delete
                                trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Remove();
                            }
                        }
                        trv_TETRACities_Cities_Tab.EndUpdate();

                    }
                    #endregion
                    #endregion

                    #region radios 

                    #region radios ADD
                    int radios_length = radios != null ? radios.Length : 0;

                    if (radios_length > 0)
                    {
                        for (int i = 0; i < radios_length; i++)
                        {
                            for (int j = 0; j < trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Count; j++)
                            {
                                City checked_city = (City)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Tag;

                                if (checked_city.CityID == radios[i].CityID)
                                {
                                    bool radios_detected = false;
                                    // if the radio belong to this site in db
                                    for (int loop = 0; loop < trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Nodes.Count; loop++)
                                    {
                                        Radios Radio_Obj = (Radios)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;

                                        if (Radio_Obj.Radio_ID == radios[i].Radio_ID)
                                        {// if the site is already a subnode of its site node

                                            trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Nodes[loop].Text = radios[i].Radio_Name;
                                            radios_detected = true;
                                            break;
                                        }
                                    }

                                    if (radios_detected == false)
                                    {
                                        // add the site as a subnode to that zone
                                        TreeNode radio_subnode = new TreeNode();
                                        radio_subnode.Text = radios[i].Radio_Name;
                                        radio_subnode.Tag = radios[i];
                                        trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Nodes.Add(radio_subnode);
                                    }
                                    break;
                                }

                            }
                        }
                    }
                    #endregion

                    #region radios Delete
                    if (radios.Length > 0)
                    {

                        for (int j = 0; j < trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Count; j++)
                        {
                            City city_Obj = (City)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Tag;

                            //foreach (TreeNode subnode in trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes)
                            for (int loop = 0; loop < trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Nodes.Count; loop++)
                            {
                                bool radios_detected = false;
                                Radios Radio_obj = (Radios)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;
                                //check if the subnode is in radios db   
                                for (int i = 0; i < radios_length; i++)
                                {
                                    if (Radio_obj.Radio_ID == radios[i].Radio_ID && radios[i].CityID == Radio_obj.CityID)
                                    {
                                        radios_detected = true;
                                        break;
                                    }
                                }
                                if (radios_detected == false)
                                {
                                    //remove this subnode
                                    trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Nodes.RemoveAt(loop);
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion

                    if (Cities.Length != 0)
                    {
                        load_comboBox_City_ID_Radios_Tab();
                    }
                    if (Cities_length == 0)
                    {
                        foreach (TreeNode node in trv_TETRACities_Cities_Tab.Nodes[0].Nodes)
                        {
                            trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Remove(node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #region status label
        public delegate void Change_Label_delegate_Cities(string message, Color color);
        public void update_Cities_Statues_label(string message, Color color)
        {
            try
            {
                if (lbl_txt_status_Cities_Tab.InvokeRequired)
                {
                    lbl_txt_status_Cities_Tab.Invoke(new Change_Label_delegate_Cities(update_Cities_Statues_label), new object[] { message, color });
                }
                else
                {
                    lbl_txt_status_Cities_Tab.ForeColor = color;
                    lbl_txt_status_Cities_Tab.Text = message;
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        //loading bar function and delegate
        #region loading bar
        public delegate void Change_City_loading_bar_percentage(int value);
        public void change_City_loadingbar_percentage(int value)
        {
            try
            {
                if (circularProgressBar_Cities_panel.InvokeRequired)
                {
                    circularProgressBar_Cities_panel.Invoke(new Change_City_loading_bar_percentage(change_City_loadingbar_percentage), new object[] { value });
                }
                else
                {
                    circularProgressBar_Cities_panel.Value = value;
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #endregion

        #region combobox
        public delegate void load_comboBox_City_ID_Radios_Tab_delegate();
        public void load_comboBox_City_ID_Radios_Tab()
        {
            if (CB_City_Radios_tab.InvokeRequired)
            {
                load_comboBox_City_ID_Radios_Tab_delegate del = new load_comboBox_City_ID_Radios_Tab_delegate(load_comboBox_City_ID_Radios_Tab);
                CB_City_Radios_tab.Invoke(del);
            }
            else
            {
                try
                {
                    List<string> City_name_List = new List<string>();
                    string none = "none";
                    City_name_List.Add(none);
                    foreach (City city in Cities)
                    {
                        City_name_List.Add(city.CityName);
                    }
                    CB_City_Radios_tab.DataSource = City_name_List;
                }
                catch (Exception ex)
                {
                    Auditing.Error(ex.Message);
                }
            }
        }
        #endregion


        #endregion

        #region other functions
        // add edit delete search functions
        public void Add_City(City new_city)
        {
            try
            {
                loading_bar_City_panel_thread();

                bool check = WS_Control_obj.Add_city(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, new_city);
                if (check)
                {

                    Cities = WS_Control_obj.Select_All_cities(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Cities_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRACities_Cities_Tab();
                }
                else
                {
                    update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }

                workdone = true;
            }
            catch (Exception ex)
            {
                update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Delete_City(int city_id)
        {
            try
            {
                loading_bar_City_panel_thread();

                if (WS_Control_obj.Delete_city(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, city_id))
                {
                    Cities = WS_Control_obj.Select_All_cities(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Cities_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRACities_Cities_Tab();
                }
                else
                {
                    update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }

                workdone = true;
            }
            catch (Exception ex)
            {
                update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Edit_City(City Updated_City)
        {
            try
            {
                loading_bar_City_panel_thread();

                bool check = WS_Control_obj.Edit_city(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, Updated_City.CityID, Updated_City);
                if (check)
                {
                    Cities = WS_Control_obj.Select_All_cities(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Cities_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRACities_Cities_Tab();
                }
                else
                {
                    update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
                workdone = true;
            }
            catch (Exception ex)
            {
                update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }
        public void Search_City()
        {
            bool found = false;
            try
            {
                string searched_city_name = txt_CityName_Cities_Tab.Text;

                #region searching the radio
                foreach (TreeNode node in trv_TETRACities_Cities_Tab.Nodes[0].Nodes)
                {
                    Intentional = false;
                    City city = (City)node.Tag;
                    if (city.CityName == searched_city_name)
                    {
                        Intentional = true;
                        trv_TETRACities_Cities_Tab.SelectedNode = node;
                        City selected_city = (City)node.Tag;
                        found = true;
                        #region clear fields
                        txt_CityID_Cities_Tab.Text = "";
                        txt_CityName_Cities_Tab.Text = "";
                        txt_INFO_Cities_Tab.Text = "";

                        #endregion
                        txt_CityID_Cities_Tab.Text = selected_city.CityID.ToString();
                        txt_CityName_Cities_Tab.Text = selected_city.CityName;
                        txt_INFO_Cities_Tab.Text = selected_city.Info;
                        update_Cities_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                        break;

                    }
                }
                #endregion
                if (!found)
                {
                    update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                    Intentional = false;
                }
            }
            catch (Exception ex)
            {
                update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);

            }
        }
        //public int get_city_id(string cityname)
        //{
        //    foreach(City city in Cities)
        //    {
        //        //if(cityname == "none")
        //        //{
        //        //    return 0;
        //        //}
        //        if(city.CityName == cityname )
        //        {
        //            return city.CityID;
        //        }
        //    }
        //    return null;
        //}
        public string get_city_name(int id)
        {
            foreach (City city in Cities)
            {
                if (id == city.CityID)
                {
                    return city.CityName;
                }
            }
            return "none";
        }

        public void loading_bar_City_panel_thread()
        {
            try
            {
                workdone = false;
                Thread loading_City_operation_thread = new Thread(Cities_loading_bar_progress);
                loading_City_operation_thread.Start();
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        public void Cities_loading_bar_progress()
        {
            Thread.Sleep(1000);
            change_City_loadingbar_percentage(100);
            Thread.Sleep(1000);
            change_City_loadingbar_percentage(0);

        }


        #endregion

        #endregion

    }
}


