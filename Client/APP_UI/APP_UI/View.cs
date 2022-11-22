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
using TETRA_Coverage_Monitor.Code;
using System.IO;

namespace TETRA_Coverage_Monitor
{
    public partial class View : Form
    {
        Web_service_Control WS_Control_obj = new Web_service_Control();
        public bool End_loading_bar_thread { get; set; }

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
                Esri_control.initialize_drawing_data_collection_array();
                List<Color> colorlist = Esri_control.get_colors();
                initialize_picture_boxes_colors();
                #endregion               
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #region Main Functions

        #region Form Events
        private void View_Load(object sender, EventArgs e)
        {
            try
            {
                End_loading_bar_thread = false;

                #region Initialize esri map
                //initialize map 
                elementHost_Mapview.Child = Esri_control.Load_Esri_Control();
                Esri_control.ev_Tapped_Building_AlarmUnit += Main_MAP_Object_Click;
                string path = "http://192.168.1.100:6080/arcgis/rest/services/BaseMap/MOI_Vector2020/MapServer";
                Esri_control.Add_Map_Layer(true, Code.Singleton.Radio_layer, path);
                #endregion
                //main load loop
                Main_Thread = new Thread(load_all_treeviews_cycle_and_Draw_Sites);
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
            //24bits
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
            if (privilages[5] != "1")
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
            if (privilages[15] != "1")
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
                btn_view_coverage_all_sites.Visible = false;
                Btn_Clear_coverage.Visible = false;
            }
            #endregion
        }
        public void load_all_treeviews_cycle_and_Draw_Sites()
        {
            try
            {
                while (true)
                {
                    Zones = WS_Control_obj.Select_all_zones(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    Sites = WS_Control_obj.Select_all_sites(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    radios = WS_Control_obj.Select_all_radios(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    Cities = WS_Control_obj.Select_All_cities(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);

                    load_trv_TETRAZones_Zones_Tab();
                    load_trv_TETRASites_Sites_Tab();
                    load_trv_TETRARadios_Radios_Tab();
                    load_trv_TETRACities_Cities_Tab();

                    Draw_All_Sites();

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

        Zone[] Zones;

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
                    update_Zones_Statues_label(Code.Singleton.Missing_Inserted_Data_Message, Color.Red);
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
                else
                {
                    update_Zones_Statues_label(Code.Singleton.Missing_Inserted_Data_Message, Color.Red);
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
                if(trv_TETRAZones_Zones_Tab.SelectedNode.Text != "Zones")
                {
                    if (trv_TETRAZones_Zones_Tab.SelectedNode.Parent.Text != "Zones")
                    {
                        Show_Zone_Fields((Zone)trv_TETRAZones_Zones_Tab.SelectedNode.Parent.Tag);
                    }
                    else
                    {
                        Show_Zone_Fields((Zone)trv_TETRAZones_Zones_Tab.SelectedNode.Tag);
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
        public delegate void Change_zone_loading_bar_percentage_delegate(int value);
        public void change_Zone_loadingbar_percentage_async(int value)
        {
            try
            {
                if (circularProgressBar_zone_panel.InvokeRequired)
                {
                    circularProgressBar_zone_panel.Invoke(new Change_zone_loading_bar_percentage_delegate(change_Zone_loadingbar_percentage_async), new object[] { value });
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
                    trv_TETRAZones_Zones_Tab.Invoke(new load_trv_TETRAZones_Zones_Tab_delegate(load_trv_TETRAZones_Zones_Tab));
                }
                else
                {

                    #region Zones
                    int Zones_length = Zones != null ? Zones.Length : 0;
                    load_comboBox_Zone_ID_Sites_Tab(Zones);

                    trv_TETRAZones_Zones_Tab.BeginUpdate();


                    #region Zones Add, Edit

                    if (Zones_length != 0)
                    {
                        for (int i = 0; i < Zones_length; i++)
                        {
                            bool Find_Flag = false;
                            //add , edit operations
                            for (int j = 0; j < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count; j++)
                            {
                                Zone selected_zone = (Zone)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag;
                                if (Zones[i].Zone_ID == selected_zone.Zone_ID)
                                {
                                    Find_Flag = true;
                                    trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag = Zones[i];
                                    trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Text = Zones[i].Zone_Name;
                                    break;
                                }
                            }

                            if (Find_Flag == false)
                            {
                                TreeNode new_zone_node = new TreeNode();
                                new_zone_node.Text = Zones[i].Zone_Name;
                                new_zone_node.Tag = Zones[i];
                                trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Add(new_zone_node);
                            }
                        }
                    }
                    #endregion

                    #region Zones Delete
                    if (Zones_length != 0 && trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count > 0)
                    {
                        for (int j = 0; j < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count; j++)
                        {
                            bool Find_Flag = false;

                            Zone current_Zone_Obj = (Zone)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag;

                            for (int i = 0; i < Zones.Length; i++)
                            {
                                if (current_Zone_Obj.Zone_ID == Zones[i].Zone_ID)
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
                    int Sites_length = Sites != null ? Sites.Length : 0;

                    trv_TETRAZones_Zones_Tab.BeginUpdate();
                    #region Sites Add, Edit
                    for (int i = 0; i < Sites_length; i++)
                    {
                        for (int j = 0; j < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes.Count; j++)
                        {
                            Zone checked_zone = (Zone)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Tag;

                            if (checked_zone.Zone_ID == Sites[i].Zone_ID)
                            {
                                bool Find_Flag = false;
                                // if the site belong to this zone in db
                                for (int loop = 0; loop < trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes.Count; loop++)
                                {
                                    Sites current_Sites_Obj = (Sites)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;

                                    if (current_Sites_Obj.Site_ID == Sites[i].Site_ID)
                                    {
                                        // if the site is a subnode of its zone node
                                        trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes[loop].Text = Sites[i].Site_Name;
                                        Find_Flag = true;
                                        break;
                                    }
                                }

                                if (Find_Flag == false)
                                {// add the site as a subnode to that zone
                                    TreeNode new_site_subnode = new TreeNode();
                                    new_site_subnode.Text = Sites[i].Site_Name;
                                    new_site_subnode.Tag = Sites[i];

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
                            Sites current_Site_obj = (Sites)trv_TETRAZones_Zones_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;

                            //check if the subnode is in sites db   
                            for (int i = 0; i < Sites_length; i++)
                            {
                                if (Sites[i].Site_ID == current_Site_obj.Site_ID && Sites[i].Zone_ID == Zone_Obj.Zone_ID)
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
                    Zones = WS_Control_obj.Select_all_zones(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Zones_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRAZones_Zones_Tab();
                }
                else
                {
                    update_Zones_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
                End_loading_bar_thread = true;
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
                    Zones = WS_Control_obj.Select_all_zones(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    load_trv_TETRAZones_Zones_Tab();
                    update_Zones_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                }
                End_loading_bar_thread = true;
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
                    Zones = WS_Control_obj.Select_all_zones(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Zones_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRAZones_Zones_Tab();
                }
                End_loading_bar_thread = true;
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
                    loading_bar_zone_panel_thread();

                    if (node.Text == txt_Zone_Name_Zones_Tab.Text.ToString())
                    {
                        found = true;
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
                        End_loading_bar_thread = true;
                        break;
                    }
                }
                if (!found)
                {
                    update_Zones_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                    End_loading_bar_thread = true;
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }
        }
        public void zones_loading_bar_progress_cycle()
        {
            while (!End_loading_bar_thread)
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
                End_loading_bar_thread = false;
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
        Sites[] Sites;

        #region Events

        #region buttons
        private void btn_Add_Site_Sites_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_Latitude_Sites_Tab.Text != "" &&
                       txt_Longtitude_Sites_Tab.Text != "" &&
                       txt_SiteName_Sites_Tab.Text != "" &&
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
                       txt_SiteName_Sites_Tab.Text != "" &&
                       txt_Longtitude_Sites_Tab.Text != "" &&
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
                if(trv_TETRASites_Sites_Tab.SelectedNode.Text.ToString() != "Sites")
                {
                    if (trv_TETRASites_Sites_Tab.SelectedNode.Parent.Text.ToString() != "Sites")
                    {
                        Sites Parent_site = (Sites)trv_TETRASites_Sites_Tab.SelectedNode.Parent.Tag;
                        Show_Sites_Fields(Parent_site);
                    }
                    else
                    {
                        Show_Sites_Fields((Sites)trv_TETRASites_Sites_Tab.SelectedNode.Tag);
                    }

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
                    Esri_control.Fly_To(selected_site.Latitude, selected_site.Longitude, 150);
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
        public delegate void Change_site_loading_bar_percentage_delegate(int value);
        public void change_Site_loadingbar_percentage_async(int value)
        {
            try
            {
                if (circularProgressBar_site_panel.InvokeRequired)
                {
                    circularProgressBar_site_panel.Invoke(new Change_site_loading_bar_percentage_delegate(change_Site_loadingbar_percentage_async), new object[] { value });
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
                    trv_TETRASites_Sites_Tab.Invoke(new load_trv_TETRASites_Sites_Tab_delegate(load_trv_TETRASites_Sites_Tab));
                }
                else
                {
                    trv_TETRASites_Sites_Tab.BeginUpdate();
                    #region Sites
                    int sites_length = Sites != null ? Sites.Length : 0;
                    if (sites_length != 0)
                    {
                        load_comboBox_Choose_site_for_coverage(Sites);
                        #region Sites add, edit

                        for (int i = 0; i < sites_length; i++)

                        {
                            bool Find_Flag = false;
                            for (int j = 0; j < trv_TETRASites_Sites_Tab.Nodes[0].Nodes.Count; j++)
                            {
                                Sites selected_site = (Sites)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Tag;
                                if (Sites[i].Site_ID == selected_site.Site_ID)
                                {
                                    //edit 
                                    Find_Flag = true;
                                    trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Tag = Sites[i];
                                    trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Text = Sites[i].Site_Name;

                                    break;
                                }
                            }
                            if (Find_Flag == false)
                            {
                                //add
                                TreeNode new_site_node = new TreeNode();
                                new_site_node.Text = Sites[i].Site_Name;
                                new_site_node.Tag = Sites[i];
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

                                Sites current_Site_obj = (Sites)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Tag;
                                for (int i = 0; i < Sites.Length; i++)
                                {
                                    if (current_Site_obj.Site_ID == Sites[i].Site_ID)
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
                    }else
                    {
                        trv_TETRASites_Sites_Tab.Nodes[0].Nodes.Clear();
                        load_comboBox_Choose_site_for_coverage(Sites);
                        return;
                    }

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
                                    Radios current_Radio_Obj = (Radios)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;

                                    if (current_Radio_Obj.Radio_ID == radios[i].Radio_ID)
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
                            Radios current_Radio_obj = (Radios)trv_TETRASites_Sites_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;
                            //check if the subnode is in radios db   
                            for (int i = 0; i < len2; i++)
                            {
                                if (current_Radio_obj.Radio_ID == radios[i].Radio_ID && radios[i].Site_ID == current_Radio_obj.Site_ID)
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

                    Sites = WS_Control_obj.Select_all_sites(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    load_trv_TETRASites_Sites_Tab();
                    update_Sites_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);

                }
                End_loading_bar_thread = true;
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

                    Sites = WS_Control_obj.Select_all_sites(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Sites_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRASites_Sites_Tab();
                }
                else
                {
                    update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);

                }
                End_loading_bar_thread = true;

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
                    Sites = WS_Control_obj.Select_all_sites(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password);
                    update_Sites_Statues_label(Code.Singleton.Opertation_Successful_Message, Color.Green);
                    load_trv_TETRASites_Sites_Tab();

                }
                else
                {
                    update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
                End_loading_bar_thread = true;
            }
            catch (Exception ex)
            {
                update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
                End_loading_bar_thread = true;
            }
        }
        public void Search_Sites()
        {
            try
            {
                bool found = false;

                foreach (TreeNode node in trv_TETRASites_Sites_Tab.Nodes[0].Nodes)
                {

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
                        End_loading_bar_thread = true;
                        break;
                    }
                }
                if (!found)
                {
                    update_Sites_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                    End_loading_bar_thread = true;

                }

            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public string get_site_name(int id)
        {
            foreach (Sites site in Sites)
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
                foreach (Sites site in Sites)
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
            while (!End_loading_bar_thread)
            {
                Thread.Sleep(1000);
                change_Site_loadingbar_percentage_async(100);
                Thread.Sleep(1000);
                change_Site_loadingbar_percentage_async(0);
            }

        }
        public void loading_bar_site_panel_thread()
        {
            try
            {
                End_loading_bar_thread = false;
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
                int int_check = 0;
                if (txt_ISSI_Radios_Tab.Text != "" && int.TryParse(txt_ISSI_Radios_Tab.Text, out int_check))
                {
                    Radios new_Radio = new Radios();

                    int issi = int.Parse(txt_ISSI_Radios_Tab.Text);
                    new_Radio.ISSI = issi;
                    new_Radio.Radio_Name = !string.IsNullOrEmpty(txt_Radio_Name_Radios_Tab.Text) ? txt_Radio_Name_Radios_Tab.Text : issi.ToString();

                    new_Radio.Info = !string.IsNullOrEmpty(txt_INFO_Radios_Tab.Text) ? txt_INFO_Radios_Tab.Text : null;
                    new_Radio.Model = !string.IsNullOrEmpty(txt_model_Radios_Tab.Text) ? txt_model_Radios_Tab.Text : null;
                    //new_Radio.RCPIN = !string.IsNullOrEmpty(txt_.Text) ? txt_Serial_Number_Radios_Tab.Text : null
                    new_Radio.SerialNum = !string.IsNullOrEmpty(txt_Serial_Number_Radios_Tab.Text) ? txt_Serial_Number_Radios_Tab.Text : null;
                    new_Radio.Site_ID = 0;
                    new_Radio.TEI = !string.IsNullOrEmpty(txt_TEI_Radios_Tab.Text) ? txt_TEI_Radios_Tab.Text : null;

                    new_Radio.Radio_Type = comboBox_Radio_type_Radios_tab.Text ;
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
                    End_loading_bar_thread = true;
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
                        foreach (Sites site in Sites)
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
                        Esri_control.add_single_Picture_point("radios", Esri_control.create_radio_pic_point(selected_radio.gpsPosition.Latitude, selected_radio.gpsPosition.Longitude), selected_radio.ISSI);
                        Esri_control.Fly_To(selected_radio.gpsPosition.Latitude, selected_radio.gpsPosition.Longitude, 150);
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
                    trv_TETRARadios_Radios_Tab.Invoke(new load_trv_TETRARadios_Radios_Tab_delegate(load_trv_TETRARadios_Radios_Tab));
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
                            {   //edit
                                Radios current_Radio_Obj = (Radios)trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].Tag;
                                if (radios[i].Radio_ID == current_Radio_Obj.Radio_ID)
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
                                                                    Esri_control.add_single_Picture_point("radios", Esri_control.create_radio_pic_point(radios[i].gpsPosition.Latitude, radios[i].gpsPosition.Longitude), radios[i].ISSI);
                                                                    Esri_control.Fly_To(radios[i].gpsPosition.Latitude, radios[i].gpsPosition.Longitude, 150);
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
                    else
                    {
                        trv_TETRARadios_Radios_Tab.Nodes[0].Nodes.Clear();
                        return;
                    }

                    #endregion

                    #region Radio delete


                    if (Radios_length != 0 && trv_TETRARadios_Radios_Tab.Nodes[0].Nodes.Count > 0)
                    {
                        trv_TETRARadios_Radios_Tab.BeginUpdate();

                        for (int j = 0; j < trv_TETRARadios_Radios_Tab.Nodes[0].Nodes.Count; j++)
                        { // delete operation
                            bool check_flag = false;
                            Radios current_radio_obj = (Radios)trv_TETRARadios_Radios_Tab.Nodes[0].Nodes[j].Tag;

                            for (int i = 0; i < radios.Length; i++)
                            {
                                if (current_radio_obj.Radio_ID == radios[i].Radio_ID)
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

        public delegate void get_site_neightbours_delegate(Radios radio);
        public void get_neightbour_Sites(Radios radio)
        {
            try
            {
                if (txt_neighbours_radios_tab.InvokeRequired)
                {
                    txt_neighbours_radios_tab.Invoke(new get_site_neightbours_delegate(get_neightbour_Sites));
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
                                foreach (Sites site in Sites)
                                {

                                    if (site.Site_ID == log.Site_ID)
                                    {
                                        if (selected_radio.Site_ID == site.Site_ID)
                                        {
                                            int site_rssi = Convert.ToInt32(log.RSSI);
                                            string dbm_value = Esri_control.convert_rssi_to_dbm_value(site_rssi);
                                            lbl_txt_site_signal_strength_radios_tab.Text = " " + dbm_value;
                                            break;
                                        }

                                        int rssi = Convert.ToInt32(log.RSSI);
                                        string RSSI_to_dbm_value = Esri_control.convert_rssi_to_dbm_value(rssi);
                                        txt_neighbours_radios_tab.Text += "Zone " + site.Zone_ID + " ID " + site.Cell_ID + " " + site.Site_Name + " " + " ( " + " " + RSSI_to_dbm_value + " )" + "\r\n"/*" \t " + " \t " + " \t " + " \t "*/;
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

        public delegate void Change_radio_loading_bar_percentage_delegate(int value);
        public void change_radio_loadingbar_percentage(int value)
        {
            try
            {
                if (circularProgressBar_radio_panel.InvokeRequired)
                {
                    circularProgressBar_radio_panel.Invoke(new Change_radio_loading_bar_percentage_delegate(change_radio_loadingbar_percentage), new object[] { value });
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
                    End_loading_bar_thread = true;
                }
                else
                {
                    radio_type = "";
                    End_loading_bar_thread = true;
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
                End_loading_bar_thread = true;
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

                End_loading_bar_thread = true;
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
                End_loading_bar_thread = true;
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
                End_loading_bar_thread = false;
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
                End_loading_bar_thread = false;
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
                End_loading_bar_thread = false;
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
                End_loading_bar_thread = false;
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
            End_loading_bar_thread = false;
        }
        private void SB_btn_open_mic_Radios_Tab_Click(object sender, EventArgs e)
        {
            try
            {
                Thread open_mic_thread = new Thread(open_mic_radio_request);
                open_mic_thread.Start();
                End_loading_bar_thread = false;
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
                        End_loading_bar_thread = true;
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
                            End_loading_bar_thread = true;

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
                End_loading_bar_thread = true;

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
                End_loading_bar_thread = true;

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

                    foreach (Sites site in Sites)
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
            try
            {
                if (pnl_Map.InvokeRequired)
                {
                    pnl_Map.Invoke(new draw_sites_delegate(Draw_All_Sites));
                }
                else
                {
                    List<float> lattitude_list = new List<float>();
                    List<float> longtitude_list = new List<float>();
                    List<string> sitename_list = new List<string>();
                    List<int> site_id_list = new List<int>();
                    if (Sites != null)
                    {
                        foreach (Sites site in Sites)
                        {
                            lattitude_list.Add(site.Latitude);
                            longtitude_list.Add(site.Longitude);
                            sitename_list.Add(site.Site_Name);
                            site_id_list.Add(site.Site_ID);
                        }
                    }
                    Esri_control.draw_all_Sites(lattitude_list, longtitude_list, sitename_list, site_id_list);
                }

            }catch(Exception ex)
            {
                Auditing.Error(ex.Message);
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
            foreach (Sites site in Sites)
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

        public void initialize_picture_boxes_colors()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Files\colors.txt";
            StreamReader stream = new StreamReader(path);
            string file_data = stream.ReadToEnd();
            int counter = 0;
            System.Drawing.Color[] list = new System.Drawing.Color[32];
            foreach (string line in System.IO.File.ReadLines(path))
            {
                string[] line_no_space = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                list[counter] = System.Drawing.Color.FromName(line_no_space[0]);
                counter++;
            }
            pic_c_1.BackColor = list[0];
            pic_c_2.BackColor = list[1];
            pic_c_3.BackColor = list[2];
            pic_c_4.BackColor = list[3];
            pic_c_5.BackColor = list[4];
            pic_c_6.BackColor = list[5];
            pic_c_7.BackColor = list[6];
            pic_c_8.BackColor = list[7];
            pic_c_9.BackColor = list[8];
            pic_c_10.BackColor = list[9];
            pic_c_11.BackColor = list[10];
            pic_c_12.BackColor = list[11];
            pic_c_13.BackColor = list[12];
            pic_c_14.BackColor = list[13];
            pic_c_15.BackColor = list[14];
            pic_c_16.BackColor = list[15];
            pic_c_17.BackColor = list[16];
            pic_c_18.BackColor = list[17];
            pic_c_19.BackColor = list[18];
            pic_c_20.BackColor = list[19];
            pic_c_21.BackColor = list[20];
            pic_c_22.BackColor = list[21];
            pic_c_23.BackColor = list[22];
            pic_c_24.BackColor = list[23];
            pic_c_25.BackColor = list[24];
            pic_c_26.BackColor = list[25];
            pic_c_27.BackColor = list[26];
            pic_c_28.BackColor = list[27];
            pic_c_29.BackColor = list[28];
            pic_c_30.BackColor = list[29];
            pic_c_31.BackColor = list[30];
            pic_c_32.BackColor = list[31];
        }
        public List<double> ids = new List<double>();


        #endregion


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

        #region buttons
        private void btn_view_coverage_all_sites_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (comboBox_coverage_draw_method_Map_pnl.Text == "Polygons")
                {
                    if (comboBox_Choose_site_coverage.Text == "All Sites" || comboBox_Choose_site_coverage.SelectedIndex == 0)
                    {
                        Esri_control.clear_all_drawing_data_collection_layers();
                            radio_type = comboBox_Radio_type_Map_pnl.Text;
                        start_date = start_Date.Value;
                        end_date = End_Date.Value;
                        Thread Draw_coverage_thread = new Thread(draw_coverage_for_all_sites_as_polygon);
                        Draw_coverage_thread.Start();
                    }
                    else if (!string.IsNullOrEmpty(comboBox_Choose_site_coverage.Text))
                    {
                        Esri_control.clear_all_drawing_data_collection_layers();

                        Site_name = comboBox_Choose_site_coverage.Text;
                        radio_type = comboBox_Radio_type_Map_pnl.Text;
                        start_date = start_Date.Value;
                        end_date = End_Date.Value;
                        foreach (Sites site in Sites)
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

                        Esri_control.clear_all_drawing_data_collection_layers();

                        radio_type = comboBox_Radio_type_Map_pnl.Text;
                        start_date = start_Date.Value;
                        end_date = End_Date.Value;
                        Thread Draw_coverage_thread = new Thread(draw_coverage_for_all_sites_as_points);

                        Draw_coverage_thread.Start();
                    }
                    else if (!string.IsNullOrEmpty(comboBox_Choose_site_coverage.Text) || comboBox_Choose_site_coverage.SelectedIndex != 0)
                    {
                        Esri_control.clear_all_drawing_data_collection_layers();


                        Site_name = comboBox_Choose_site_coverage.Text;
                        radio_type = comboBox_Radio_type_Map_pnl.Text;
                        start_date = start_Date.Value;
                        end_date = End_Date.Value;
                        foreach (Sites site in Sites)
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
            Esri_control.clear_all_drawing_data_collection_layers();
        }
        #endregion

        #endregion

        #region Delagate

        public delegate void coverage_draw_polygons_for_one_site_delegate(List<Logs> logs_list);
        public void draw_coverage_polygons_for_one_site(List<Logs> logs_list)
        {
            try
            {
                if (pnl_Map.InvokeRequired)
                {
                    pnl_Map.Invoke(new coverage_draw_polygons_for_one_site_delegate(draw_coverage_polygons_for_one_site), new object[] { logs_list });
                }
                else
                {
                    List<float> RSSI_list = new List<float>();
                    List<double> Longitude_list = new List<double>();
                    List<double> Latitude_list = new List<double>();

                    if (logs_list != null)
                    {
                        foreach (Logs log in logs_list)
                        {
                            if (log.GPS_Position != null)
                            {
                                RSSI_list.Add(log.RSSI);
                                Longitude_list.Add(log.GPS_Position.Longitude);
                                Latitude_list.Add(log.GPS_Position.Latitude);
                            }
                        }                                            
                    }
                    Esri_control.draw_Polygons_for_one_site(RSSI_list, Latitude_list, Longitude_list);
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        public delegate void coverage_draw_points_delegate(List<Logs> logs_list);
        //for one site and for all sites
        public void draw_coverage_points(List<Logs> logs_list)
        {
            try
            {
                if (pnl_Map.InvokeRequired)
                {
                    pnl_Map.Invoke(new coverage_draw_points_delegate(draw_coverage_points), new object[] { logs_list });
                }
                else
                {
                    List<int> Pos_id_list = new List<int>();
                    List<float> RSSI_list = new List<float>();
                    List<double> lattitude_list = new List<double>();
                    List<double> longtitude_list = new List<double>();
                    List<int> site_id_list = new List<int>();
                    List<int> ISSI_list = new List<int>();
                    List<string> Sitename_list = new List<string>();

                    if (logs_list != null && logs_list.Count > 0)
                    {
                        foreach (Logs log in logs_list)
                        {
                            if (log.GPS_Position != null)
                            {
                                Pos_id_list.Add(log.Pos_ID);
                                RSSI_list.Add(log.RSSI);
                                lattitude_list.Add(log.GPS_Position.Latitude);
                                longtitude_list.Add(log.GPS_Position.Longitude);
                                site_id_list.Add(log.Site_ID);
                                int issi = get_issi_from_radio_id(log.GPS_Position.Radio_ID);
                                ISSI_list.Add(issi);
                                string sitename = get_sitename_from_site_id(log.Site_ID);
                                Sitename_list.Add(sitename);                                
                            }
                        }

                    }
                    Esri_control.draw_points_for_one_site(Pos_id_list, RSSI_list, lattitude_list, longtitude_list, site_id_list, ISSI_list, Sitename_list);
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);

            }

        }

        public delegate void coverage_draw_Polygons_for_all_sites_delegate(List<List<Logs>> logs_list_collection);
        public void draw_coverage_polygons_for_all_sites(List<List<Logs>> logs_list_collection)
        {
            try
            {
                if (pnl_Map.InvokeRequired)
                {
                    pnl_Map.Invoke(new coverage_draw_Polygons_for_all_sites_delegate(draw_coverage_polygons_for_all_sites), new object[] { logs_list_collection });
                }
                else
                {
                    foreach (List<Logs> logs_list in logs_list_collection)
                    {
                        Esri_control.reset_drawing_data_collection_layers();
                        draw_coverage_polygons_for_one_site(logs_list);
                    }
                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }

        #region combobox
        public delegate void load_comboBox_Choose_site_coverage_panel_delegate(Sites[] sites_array);
        public void load_comboBox_Choose_site_for_coverage(Sites[] sites_Array)
        {
            if (comboBox_Choose_site_coverage.InvokeRequired)
            {
                comboBox_Choose_site_coverage.Invoke(new load_comboBox_Choose_site_coverage_panel_delegate(load_comboBox_Choose_site_for_coverage), new object[] { Sites });
            }
            else
            {
                try
                {
                    List<string> Sites_list = new List<string>();
                    Sites_list.Add("All Sites");
                    if (Sites != null)
                    {
                        foreach (Sites site in sites_Array)
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
        #endregion


        #region loading bar

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
        public void draw_coverage_for_all_sites_as_polygon()
        {
            drawing_loading_bar_thread();
            try
            {

                List<List<Logs>> logs_list_collection = new List<List<Logs>>();

                #region get logs from the database
                for (int i = 0; i < Sites.Length; i++)
                {

                    if (Sites[i].logsCollection != null)
                    {
                        List<Logs> logs_list = WS_Control_obj.Get_logs_filtered_by_datetime_SiteID_radioType(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, start_date, end_date, radio_type, Sites[i].Site_ID).ToList<Logs>();
                        if (logs_list != null)
                        {
                            logs_list_collection.Add(logs_list);
                        }
                    }
                }
                #endregion

                draw_coverage_polygons_for_all_sites(logs_list_collection);
                End_loading_bar_thread = true;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public void coverage_draw_one_Site_as_polygon()
        {
            try
            {
                drawing_loading_bar_thread();
                if (siteid > 0)
                {
                    List<Logs> logs_list = WS_Control_obj.Get_logs_filtered_by_datetime_SiteID_radioType(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, start_date, end_date, radio_type, siteid).ToList<Logs>();
                    draw_coverage_polygons_for_one_site(logs_list);
                }
                End_loading_bar_thread = true;
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
                    List<Logs> logs_list = WS_Control_obj.Get_logs_filtered_by_datetime_SiteID_radioType(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, start_date, end_date, radio_type, siteid).ToList<Logs>();
                    draw_coverage_points(logs_list);
                }
                End_loading_bar_thread = true;
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
                List<Logs> logs_list = WS_Control_obj.Get_logs_filtered_by_datetime_radioType(Code.Singleton.user_obj.Username, Code.Singleton.user_obj.Password, start_date, end_date, radio_type).ToList<Logs>();
                draw_coverage_points(logs_list);
                End_loading_bar_thread = true;
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        public int get_issi_from_radio_id(int radio_id)
        {
            foreach(Radios radio in radios)
            {
                if (radio_id == radio.Radio_ID)
                {
                    return radio.ISSI;
                }

            }
            return 0;
        }
        public string get_sitename_from_site_id(int site_id)
            {
                foreach(Sites site in Sites)
                {
                    if (site_id == site.Site_ID)
                    {
                        return site.Site_Name;
                    }
                }
                return "";
            }

        #region drawing loading bar
        public void drawing_loading_bar_progress()
        {
            while (!End_loading_bar_thread)
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
            if (End_loading_bar_thread)
            {
                change_drawing_progress_bar_date_pnl_percentage(100);
            }

        }

        public void drawing_loading_bar_thread()
        {
            try
            {
                End_loading_bar_thread = false;
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
                    new_City.CityName =  txt_CityName_Cities_Tab.Text;
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
                if ( txt_CityName_Cities_Tab.Text != "")
                {
                    City Updated_City = new City();

                    Updated_City.Info = !string.IsNullOrEmpty(txt_INFO_Cities_Tab.Text) ? txt_INFO_Cities_Tab.Text : null;
                    Updated_City.CityName =  txt_CityName_Cities_Tab.Text ;
                    int id = Convert.ToInt32(txt_CityID_Cities_Tab.Text);
                    Updated_City.CityID = id;

                    Thread Edit_City_thread = new Thread(() => Edit_City(Updated_City));
                    Edit_City_thread.Start();
                    End_loading_bar_thread = true;

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
                    trv_TETRACities_Cities_Tab.Invoke(new load_trv_TETRACities_Cities_Tab_delegate(load_trv_TETRACities_Cities_Tab));
                }
                else
                {

                    #region Cities
                    int Cities_length = Cities != null ? Cities.Length : 0;
                    #region Cities add, edit
                    if (Cities_length != 0)
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

                            load_comboBox_City_Names_in_Radios_Tab();

                    }
                    else
                    {
                        trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Clear();

                        load_comboBox_City_Names_in_Radios_Tab();
                        return;

                    }
                    #endregion

                    #region Cities delete

                    if (Cities_length != 0 && trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Count > 0)
                    {
                        trv_TETRACities_Cities_Tab.BeginUpdate();

                        for (int j = 0; j < trv_TETRACities_Cities_Tab.Nodes[0].Nodes.Count; j++)
                        { // delete operation
                            bool Find_Flag = false;
                            City current_City_obj = (City)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Tag;
                            for (int i = 0; i < Cities.Length; i++)
                            {
                                if (current_City_obj.CityID == Cities[i].CityID)
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
                                        Radios current_Radio_Obj = (Radios)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Nodes[loop].Tag;

                                        if (current_Radio_Obj.Radio_ID == radios[i].Radio_ID)
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
                            City current_City_Obj = (City)trv_TETRACities_Cities_Tab.Nodes[0].Nodes[j].Tag;
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

                }
            }
            catch (Exception ex)
            {
                Auditing.Error(ex.Message);
            }
        }
        #endregion

        #region status label
        public delegate void Change_Status_Label_Cities_delegate(string message, Color color);
        public void update_Cities_Statues_label(string message, Color color)
        {
            try
            {
                if (lbl_txt_status_Cities_Tab.InvokeRequired)
                {
                    lbl_txt_status_Cities_Tab.Invoke(new Change_Status_Label_Cities_delegate(update_Cities_Statues_label), new object[] { message, color });
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
        public void load_comboBox_City_Names_in_Radios_Tab()
        {
            if (CB_City_Radios_tab.InvokeRequired)
            {
                CB_City_Radios_tab.Invoke(new load_comboBox_City_ID_Radios_Tab_delegate(load_comboBox_City_Names_in_Radios_Tab));
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

                End_loading_bar_thread = true;
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

                End_loading_bar_thread = true;
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
                End_loading_bar_thread = true;
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
                foreach (TreeNode node in trv_TETRACities_Cities_Tab.Nodes[0].Nodes)
                {
                    City city = (City)node.Tag;
                    if (city.CityName == searched_city_name)
                    {
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
                if (!found)
                {
                    update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                }
            }
            catch (Exception ex)
            {
                update_Cities_Statues_label(Code.Singleton.Operation_failed_Message, Color.Red);
                Auditing.Error(ex.Message);
            }
        }       
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

        #region loading bar
        public void loading_bar_City_panel_thread()
        {
            try
            {
                End_loading_bar_thread = false;
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

        #endregion

    }
}


