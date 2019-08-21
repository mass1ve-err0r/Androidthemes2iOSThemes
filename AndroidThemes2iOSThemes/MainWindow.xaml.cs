using System;
using System.IO;
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
using System.Windows.Forms;
using static System.Windows.Forms.FlatStyle;
using System.Drawing;


namespace AndroidThemes2iOSThemes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // global inits
        private System.Windows.Forms.FolderBrowserDialog folderDlg;
        public String t_path;
        public Boolean path_is_set = false;
        public Boolean dict_is_loaded = false;
        SortedDictionary<string, string> ios_dict = new SortedDictionary<string, string>();

        // stylization because why not
        public String type_info = "[INFO]: ";
        public String type_debug = "[DEBUG]: ";
        public String type_error = "[ERROR]: ";

        // Le Window
        public MainWindow()
        {
            InitializeComponent();
            // inits
            logform.IsReadOnly = true;
            pickerx.Click += pickerx_action;
            pickerx.BorderThickness = new Thickness(1);
            B1.Click += b1_action;
            B1.BorderThickness = new Thickness(1);
            B2.Click += b2_action;
            B2.BorderThickness = new Thickness(1);
        }


        // LOGIC: FolderPicker
        void pickerx_action (object sender, RoutedEventArgs e)
        {
            // Assign the FolderDialog now
            this.folderDlg = new System.Windows.Forms.FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // set TextBox content to filepath
                T2.Text = folderDlg.SelectedPath;
                t_path = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
                // set flag path has been set
                path_is_set = true;
            }
        }

        // LOGIC: Grabbing FileNames, matching with dict & recusively renaming
        void b1_action (object sender, RoutedEventArgs e)
        {
            // check if any path is selected
            if (path_is_set)
            {
                // filter for pngs because thats how droids store
                string[] files_arr = Directory.GetFiles(t_path, "*.png");
                int files_count = files_arr.Length;
                // check if path contains any images to work with
                if (files_count > 0)
                {
                    if (dict_is_loaded)
                    {
                        // log because we can
                        log_t(type_info, "Amount of files (With Alts): ", files_count);
                        log_t(type_info, "Reading from dict & rebranding...", 0);

                        // invoke renamer with this shit lmao
                        renamer_t(files_arr);
                    }
                    else
                    {
                        log_t(type_error, "No dictionary loaded! Please load dictionary first.", 0);
                    }
                }
                else
                {
                    log_t(type_error, "No pngs inside / Theme is empty!", 0);
                }
            }
            else
            {
                log_t(type_error, "Error loading theme dir!", 0);
            }

        }

        // LOGIC: Load/ Set a dictionary
        void b2_action(object sender, RoutedEventArgs e)
        {
            // try loading a dict
            string dict_t = Directory.GetCurrentDirectory() + "\\dict.txt";
            // check if dict exists & proceed
            if (File.Exists(dict_t))
            {
                generate_dict(dict_t);
                dict_is_loaded = true;
            }
            else
            {
                log_t(type_error, "No dictionary found (dict.txt) !", 0);
            }
        }

        // LOGIC_HELPER: logger because cleaner code maybe and dat style
        void log_t(String type, String message_str, int message_int)
        {
            if (message_int == 0)
            {
                logform.Text += type + message_str + Environment.NewLine;
            }
            else
            {
                logform.Text += type + message_str + message_int + Environment.NewLine;
            }
        }

        // LOGIC_HELPER: renamer
        void renamer_t(string[] input_arr)
        {
            // init counter
            int counter = input_arr.Length;
            // create renaming loop
            foreach (string bundleID in input_arr)
            {
                // trim the string to just the BundleID
                string bid = bundleID.Replace(t_path + "\\", "");
                // debug
                // log_t(type_debug, bid, 0);
                if (ios_dict.TryGetValue(bid, out string keyval))
                {
                    // rename & replace the files
                    File.Move(t_path + "\\" + bid, t_path + "\\" + keyval);
                    // iterate the counter
                    counter--;
                    if(counter == 0)
                    {
                        log_t(type_info, "DONE", 0);
                    }
                    else
                    {
                        log_t(type_info, "Files left: ", counter);
                    }
                }
                else
                {
                    log_t(type_error, "Entry not found in Dict! Skipping icon...", 0);
                }

            }
        }

        // LOGIC_HELPER: Generate dictionary
        void generate_dict(String path_to_dictfile)
        {
            // stop unecessary reloads
            if (dict_is_loaded)
            {
                log_t(type_info, "dict already loaded!", 0);
            }
            else
            {
                // inits
                string[] tokens;
                string[] lines = System.IO.File.ReadAllLines(path_to_dictfile);
                int local_amount = lines.Length;
                // tokenizer to feed dict
                for (int i = 0; i < lines.Length; i++)
                {
                    tokens = lines[i].Split('=');
                    ios_dict.Add(tokens[0], tokens[1]);
                }

                log_t(type_info, "Dictionary loaded! Entries: ", local_amount);

                // debug
                /*
                foreach (KeyValuePair<String, String> entry in ios_dict)
                {
                    logform.Text += string.Format("Key = {0}, Value = {1}\n", entry.Key, entry.Value);
                }
                */
            }
        }
    }
}
