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
using System.Data.SQLite;
using System.Net;
using System.Data;
using System.IO;
using System.Management;
using System.Management.Instrumentation;

namespace GetInfo
{
 
    //https://docs.microsoft.com/pl-pl/windows/desktop/CIMWin32Prov/win32-bios
    //CREATE TABLE iba_inw (id INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT, user VARCHAR(50), localization VARCHAR(50), iba_nbr VARCHAR(50), company VARCHAR(50), serial_nbr VARCHAR(50), iba_inw VARCHAR(50), operating_sys VARCHAR(100), address_ip VARCHAR(50), proc VARCHAR(50), ram VARCHAR(50), disk_space VARCHAR(50));
    public partial class MainWindow : Window
    {
        Database db = new Database();

        public MainWindow()
        {
            InitializeComponent();
            LoadAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.InsertDB(usrTxt.Text.ToString(), locTxt.Text.ToString(), nibaTxt.Text.ToString(), cmpTxt.Text.ToString(), serialTxt.Text.ToString(), inwIbaTxt.Text.ToString(), osTxt.Text.ToString(), ipTxt.Text.ToString(), procTxt.Text.ToString(), ramTxt.Text.ToString(), storTxt.Text.ToString());

            }catch(Exception xe)
            {
                MessageBox.Show(xe.Message);
            }
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            inwGrid.ItemsSource = db.data().DefaultView;
        }

      

        private void InwGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataRowView dgr = inwGrid.SelectedItem as DataRowView;

            //if(dgr != null)
            //{
            //    usrTxt.Text = dgr.Row["user"].ToString();
            //    locTxt.Text = dgr.Row["localization"].ToString();
            //    cmpTxt.Text = dgr.Row["company"].ToString();
            //    serialTxt.Text = dgr.Row["serial_nbr"].ToString();
            //    inwIbaTxt.Text = dgr.Row["iba_inw"].ToString();
            //    osTxt.Text = dgr.Row["operating_sys"].ToString();
            //    ipTxt.Text = dgr.Row["address_ip"].ToString();
            //    procTxt.Text = dgr.Row["proc"].ToString();
            //    ramTxt.Text = dgr.Row["ram"].ToString();
            //    storTxt.Text = dgr.Row["disk_space"].ToString();
            //}
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadAll();
            }catch(Exception xe)
            {
                MessageBox.Show(xe.Message);
            }

        }

        private void LoadAll()
        {
            usrTxt.Text = Environment.UserName.ToString();
            nibaTxt.Text = Environment.MachineName.ToString();
            string hname = Dns.GetHostName();
            ipTxt.Text = Dns.GetHostAddresses(hname)[1].ToString();

            ManagementObjectSearcher ComSerial = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            foreach (ManagementObject wmi in ComSerial.Get())
            {
                try
                {
                    serialTxt.Text = wmi.GetPropertyValue("SerialNumber").ToString();
                    cmpTxt.Text = wmi.GetPropertyValue("Manufacturer").ToString();


                }
                catch (Exception xe)
                {
                    MessageBox.Show(xe.Message);
                }
            }

            ManagementObjectSearcher ComSerial_2 = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject wmi in ComSerial_2.Get())
            {

                try
                {
                    int ram = Convert.ToInt32(wmi.GetPropertyValue("TotalVisibleMemorySize")) / 1000000;
                    ramTxt.Text = (ram + " GB").ToString();
                    osTxt.Text = wmi.GetPropertyValue("Caption").ToString();

                }
                catch (Exception xe)
                {
                    MessageBox.Show(xe.Message);
                }
            }

            ManagementObjectSearcher ComSerial_3 = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject wmi in ComSerial_3.Get())
            {

                try
                {
                    long diskSpace = Convert.ToInt64(wmi.GetPropertyValue("Size")) / 1024 / 1024 / 1024;
                    storTxt.Text = (diskSpace + " GB").ToString();
                }
                catch (Exception xe)
                {
                    MessageBox.Show(xe.Message);
                }
            }

            ManagementObjectSearcher ComSerial_1 = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject wmi in ComSerial_1.Get())
            {
                try
                {
                    procTxt.Text = wmi.GetPropertyValue("Name").ToString();

                }
                catch (Exception xe)
                {
                    MessageBox.Show(xe.Message);
                }
            }

        }
    }

    class Database
    {
        SQLiteConnection db_connection = new SQLiteConnection("Data Source=IBA.s3db");
        

        public DataTable data()
        {
            string query = "select * from iba_inw";
            SQLiteCommand command = new SQLiteCommand(query, db_connection);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            db_connection.Open();
           
            db_connection.Close();

            return dt;
        }

        public void InsertDB(string user, string localization, string iba_nbr, string company, string serial_nbr, string iba_inw, string operating_sys, string address_ip, string proc, string ram, string disk_space)
        {
            string query = "insert into iba_inw (user, localization, iba_nbr, company, serial_nbr, iba_inw, operating_sys, address_ip, proc, ram, disk_space) VALUES ('" + user + "','" + localization + "','" + iba_nbr + "','" + company + "','" + serial_nbr + "','" + iba_inw + "','" + operating_sys + "','" + address_ip + "','" + proc + "','" + ram + "','" + disk_space + "')";

            SQLiteCommand command = new SQLiteCommand(query, db_connection);
            db_connection.Open();

            try
            {
                if (MessageBox.Show("Czy dane wprowadzono poprawnie?", "Poprawność danych", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    command.ExecuteNonQuery();
                }

            }
            catch(Exception xe)
            {
                MessageBox.Show(xe.Message);
            }

            db_connection.Close();
        }

    }

}
