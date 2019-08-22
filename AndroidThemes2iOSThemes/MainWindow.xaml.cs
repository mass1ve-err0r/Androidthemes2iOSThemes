using System;
using System.IO;
using System.Collections.Generic;
using System.Security;
using System.IO.Compression;
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
        public String t_path;
        public Boolean path_is_set = false;
        public Boolean dict_is_loaded = false;
        SortedDictionary<string, string> ios_dict = new SortedDictionary<string, string>();
        // stylization because why not
        public String type_info = "[INFO]: ";
        public String type_debug = "[DEBUG]: ";
        public String type_error = "[ERROR]: ";

        // Newer logic
        private System.Windows.Forms.OpenFileDialog fileopenerdialog;
        public String path_file;
        public String outdir = Directory.GetCurrentDirectory() + "\\_output\\";
        public String inpdir = Directory.GetCurrentDirectory() + "\\_input";
        public String info_dir = Directory.GetCurrentDirectory() + "\\_xresources";
        public String iconsfolder = "\\res\\drawable-nodpi-v4";
        public String extensionT = ".theme";
        public Boolean props_set = false;
        public Boolean file_chosen = false;
        public String innerPath, rec_tname, rec_pkgname;

        // Le Window
        public MainWindow()
        {
            InitializeComponent();
            // inits
            logform.IsReadOnly = true;
            pickerx.Click += filepicker_action;
            pickerx.BorderThickness = new Thickness(1);
            B1.Click += b1x_action; // alt action
            B1.BorderThickness = new Thickness(1);
            B2.Click += b2_action;
            B2.BorderThickness = new Thickness(1);
            ios_props.Click += ios_props_action;
            ios_props.BorderThickness = new Thickness(1);
        }


        // LOGIC: ios_props hanlder
        void ios_props_action(object sender, RoutedEventArgs e)
        {
            // open new window to set props
            Window1 props = new Window1();
            props.ShowDialog();
            // retrieve values from TextBoxes
            // Package name
            rec_pkgname = props.pname_tb.Text;
            //debug-purposes: log_t(type_debug, rec_pkgname, 0);
            // Theme name
            rec_tname = props.thame_tb.Text;
            //debug-purposes: log_t(type_debug, rec_tname, 0);

            // set flag props have been set
            props_set = true;
        }

        // LOGIC: new process handler
        void b1x_action(object sender, RoutedEventArgs e)
        {
            // make sure we picked our file & set props
            if (file_chosen && props_set)
            {
                // launch prep
                prepare_x();
            }
        }

        // LOGIC: FilePicker
        // need that regular obj sender shit due to button
        void filepicker_action(object sender, RoutedEventArgs e)
        {
            this.fileopenerdialog = new OpenFileDialog();
            System.Windows.Forms.DialogResult result = fileopenerdialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // save full filepath & show
                var filePath = fileopenerdialog.FileName;
                T2.Text = filePath;
                path_file = filePath;
                file_chosen = true;
            }
        }

        // LOGIC: import APK, extract as ZIP and export the icons folder:
        // <ZIP>\res\drawable-nodpi-v4
        void prepare_x()
        {
            // straight extract to working dir _input
            ZipFile.ExtractToDirectory(path_file, inpdir);
            log_t(type_info, "Successfully extracted theme; Exporting...", 0);
            // struct the outdir/ theme
            innerPath = outdir + rec_tname + extensionT + "\\IconBundles";
            //log_t(type_debug, innerPath, 0);
            Directory.CreateDirectory(innerPath);
            // copy icon_dir to outdir
            DirectoryInfo dirInfo = new DirectoryInfo(innerPath);
            if (dirInfo.Exists == false)
            {
                Directory.CreateDirectory(innerPath);
            }
            List<String> Iconz = Directory.GetFiles(inpdir + iconsfolder, "*.png", SearchOption.AllDirectories).ToList();
            foreach (string file in Iconz)
            {
                FileInfo mFile = new FileInfo(file);
                // to remove name collisions
                if (new FileInfo(dirInfo + "\\" + mFile.Name).Exists == false)
                {
                    mFile.MoveTo(dirInfo + "\\" + mFile.Name);
                }
            }
            log_t(type_info, "Successfully exported theme! Setting up final stuff...", 0);
            filterX();
        }


        // LOGIC: search & rename
        void filterX ()
        {
            // filter for pngs because thats how droids store
            string[] files_arr = Directory.GetFiles(innerPath, "*.png");
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

        // LOGIC: kinda like sed-i
        void injectInfo()
        {
            // copy Info to theme dir
            string themedir = outdir + rec_tname + extensionT;
            System.IO.File.Copy(info_dir + "\\Info.txt", themedir + "\\Info.txt", true);
            // replace Package & Theme Name
            string text = File.ReadAllText(themedir + "\\Info.txt");
            text = text.Replace("$var_themename", rec_tname);
            File.WriteAllText(themedir + "\\Info.txt", text);
            // change extension
            File.Move(themedir + "\\Info.txt", System.IO.Path.ChangeExtension(themedir + "\\Info.txt", ".plist"));
            log_t(type_info, "All tasks done. Theme can now be exported!\nTheme is located in the _output folder!\n", 0);
        }



        // old edited code

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
                string bid = bundleID.Replace(innerPath + "\\", "");
                // debug
                // log_t(type_debug, bid, 0);
                if (ios_dict.TryGetValue(bid, out string keyval))
                {
                    // rename & replace the files
                    File.Move(innerPath + "\\" + bid, innerPath + "\\" + keyval);
                    // iterate the counter
                    counter--;
                    if (counter == 0)
                    {
                        log_t(type_info, "DONE; Inserting customized Info.plist", 0);
                        injectInfo();
                    }
                }
                else
                {
                    // also iterate in this case lmao
                    counter--;
                    if (counter == 0)
                    {
                        log_t(type_info, "DONE; Inserting customized Info.plist", 0);
                        injectInfo();
                    }
                    //log_t(type_error, "Key-Value not found!", 0);
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
