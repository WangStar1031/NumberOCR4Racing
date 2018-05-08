using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Net;
using Newtonsoft.Json;

namespace OCR
{
    public partial class FrmMain : Form
    {
        public int selected_track;
        public TrackGroup[] tracks;

        ImageHandler imageHandler = new ImageHandler();
        NumberRecognition imgRecog = new NumberRecognition();
        Bitmap memoryImage;
        public static Bitmap patternImage = new Bitmap(20, 20);
        Size s;
        public static Size pattern_s = new Size(20, 20);
        Graphics memoryGraphics;
        public static Graphics patternMemoryGraphics = Graphics.FromImage(patternImage);
        public bool check_window_pos = true;
        public int window_pos_left = 0;
        public int window_pos_top = 0;

        public FrmMain()
        {
            InitializeComponent();

            String[] track_names = new String[100];
            track_names[3] = "Obihiro";
            track_names[10] = "Morioka";
            track_names[11] = "Mizusawa";
            track_names[18] = "Urawa";
            track_names[19] = "Funabashi";
            track_names[20] = "Oi";
            track_names[21] = "Kawasaki";
            track_names[22] = "Kanazawa";
            track_names[22] = "Kanazawa";
            track_names[23] = "Kasamatsu";
            track_names[24] = "Nagoya";
            track_names[27] = "Sonoda";
            track_names[28] = "Himeji";
            track_names[30] = "Fukuyama";
            track_names[31] = "Kochi";
            track_names[32] = "Saga";
            track_names[33] = "Arao";
            track_names[36] = "Monbetsu";

            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("http://54.95.106.5/NAR/live_video.php");
                RootObject r = JsonConvert.DeserializeObject<RootObject>(json);
                tracks = r.result.trackGroup.ToArray();
                for (int i = 0; i < tracks.Length; i++)
                {
                    cmd_track.Items.Add(new Item(track_names[Convert.ToInt16(tracks[i].trackCode)], Convert.ToInt16(tracks[i].trackCode)));
                }
                if (cmd_track.Items.Count > 0)
                {
                    cmd_track.SelectedIndex = 0;
                    selected_track = Convert.ToInt16(tracks[cmd_track.SelectedIndex].trackCode);
                    GetTrackPosition();
                }
            }

            IntPtr chrome = GetHandelByTitle("Chrome");
            if (chrome != IntPtr.Zero)
            {
                User32.RECT rct = new User32.RECT();
                User32.GetWindowRect(chrome, ref rct);
                window_pos_left = rct.left + 15;
                window_pos_top = rct.top + 79;
                check_window_pos = false;
            }

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            System.Timers.Timer cTimer = new System.Timers.Timer();
            cTimer.Elapsed += new ElapsedEventHandler(OnClearTimedEvent);
            cTimer.Interval = 10000;
            cTimer.Enabled = true;
        }

        public void GetTrackPosition()
        {
            using (WebClient wc = new WebClient())
            {
                String pos_data = wc.DownloadString("http://54.95.106.5/NAR/api_keiba_notice.php?c=18&track_id=" + selected_track.ToString());
                String[] pos_datas = pos_data.Split('-');
                if (pos_datas.Length >= 4)
                {
                    if (txt_left.InvokeRequired)
                        txt_left.Invoke(new Action(() => txt_left.Text = pos_datas[0]));
                    else
                        txt_left.Text = pos_datas[0];

                    if (txt_top.InvokeRequired)
                        txt_top.Invoke(new Action(() => txt_top.Text = pos_datas[1]));
                    else
                        txt_top.Text = pos_datas[1];

                    if (txt_width.InvokeRequired)
                        txt_width.Invoke(new Action(() => txt_width.Text = pos_datas[2]));
                    else
                        txt_width.Text = pos_datas[2];

                    if (txt_height.InvokeRequired)
                        txt_height.Invoke(new Action(() => txt_height.Text = pos_datas[3]));
                    else
                        txt_height.Text = pos_datas[3];
                }
            }
        }

        private void OnClearTimedEvent(object source, ElapsedEventArgs e)
        {
            IntPtr chrome = GetHandelByTitle("Google");
            if (chrome != IntPtr.Zero)
            {
                User32.RECT rct = new User32.RECT();
                User32.GetWindowRect(chrome, ref rct);

                window_pos_left = rct.left + 15;
                window_pos_top = rct.top + 79;

                check_window_pos = false;
            }
            else
            {
                check_window_pos = true;
            }

            GC.Collect();
        }
        public Boolean CheckOddsType(Bitmap check_image, int min_check, int max_check)
        {
            Color c;
            int left = (int)(check_image.Width / 2);
            int top = (int)(check_image.Height / 2);
            c = check_image.GetPixel(left, top);
            if ((c.R > min_check) && (c.R < max_check)) return true;
            return false;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (check_window_pos) return;

            int video_width = Convert.ToInt16(txt_width.Text);
            int video_height = Convert.ToInt16(txt_height.Text);

            memoryImage = new Bitmap(video_width, video_height);
            s = new Size(memoryImage.Width, memoryImage.Height);
            memoryGraphics = Graphics.FromImage(memoryImage);


            int video_left = Convert.ToInt16(txt_left.Text);
            int video_top = Convert.ToInt16(txt_top.Text);

            bool check_ocr = false;
            if (txt_origin.Text == "1") check_ocr = true;

            memoryGraphics.CopyFromScreen(video_left + window_pos_left, video_top + window_pos_top, 0, 0, s);
            imageHandler._currentBitmap = memoryImage;

            if (selected_track == 24)
            {
                // Nagoya Used Pattern Image
                patternMemoryGraphics.CopyFromScreen(video_left + window_pos_left + 250, window_pos_top + 84, 0, 0, pattern_s);
                pic_pattern.Image = patternImage;
            }
            else if (selected_track == 32)
            {
                // Saga Used Pattern Image
                patternMemoryGraphics.CopyFromScreen(video_left + window_pos_left + 270, video_top + window_pos_top - 330, 0, 0, pattern_s);
                pic_pattern.Image = patternImage;
            }
            else if (selected_track == 3)
            {
                // Obihiro Used Pattern Image
                patternMemoryGraphics.CopyFromScreen(window_pos_left + 410, window_pos_top + 100, 0, 0, pattern_s);
                pic_pattern.Image = patternImage;
            }
            else
            {
                patternMemoryGraphics.CopyFromScreen(window_pos_left + 250, window_pos_left + 60, 0, 0, pattern_s);
                //patternMemoryGraphics.CopyFromScreen(window_pos_left + 410, window_pos_top + 100, 0, 0, pattern_s);
                pic_pattern.Image = patternImage;
            }

            int odds_type = 0;

            if (check_ocr)
            {
                if ((selected_track == 18) || (selected_track == 20))
                {
                    odds_type = imageHandler.PreProcess_for_Urawa();
                }
                else if (selected_track == 24)
                {
                    if (imageHandler.CheckPixelColor(patternImage, 161, 209, 255)) odds_type = 1; // TRI
                    else if (imageHandler.CheckPixelColor(patternImage, 211, 158, 255)) odds_type = 2; // TRO
                    if (odds_type > 0) imageHandler.PreProcess_for_Nagoya();
                    else imageHandler._currentBitmap = new Bitmap(1, 1);
                }
                else if (selected_track == 27)
                {
                    imageHandler.PreProcess_for_Sonoda();
                    //if (imageHandler._currentBitmap.Width > 2) imageHandler.Resize(imageHandler._currentBitmap.Width * 2, imageHandler._currentBitmap.Height * 2);
                }
                else if (selected_track == 19)
                {
                    imageHandler.PreProcess_for_OOi();
                    //if (imageHandler._currentBitmap.Width > 2) imageHandler.Resize(imageHandler._currentBitmap.Width * 2, imageHandler._currentBitmap.Height * 2);
                }
                else if (selected_track == 21)
                {
                    if (imageHandler.CheckPixelColor(1, 60, 230, 226, 14)) odds_type = 1; // WIN, BQNL, BEXA
                    else if (imageHandler.CheckPixelColor(1, 45, 141, 231, 73) && imageHandler.CheckPixelColor(1, 67, 234, 137, 26)) odds_type = 2; // TRO, TRI
                    else if (imageHandler.CheckPixelColor(1, 45, 27, 28, 240) && imageHandler.CheckPixelColor(1, 67, 81, 156, 52)) odds_type = 3; // WIN, EXA
                    else if (imageHandler.CheckPixelColor(1, 45, 29, 33, 237) && imageHandler.CheckPixelColor(1, 67, 194, 73, 23)) odds_type = 4; // WIN, PLC
                    else if (imageHandler.CheckPixelColor(1, 67, 18, 21, 139)) odds_type = 5; // QNL

                    if (odds_type > 0) imageHandler.PreProcess_for_Kawasaki(odds_type);
                    else imageHandler._currentBitmap = new Bitmap(1, 1);
                }
                else if (selected_track == 22)
                {
                    imageHandler.PreProcess_for_Kanazawa();
                    //if (imageHandler._currentBitmap.Width > 2) imageHandler.Resize(imageHandler._currentBitmap.Width * 2, imageHandler._currentBitmap.Height * 2);
                }
                else if (selected_track == 32)
                {

                    if (imageHandler.CheckPixelColor2(patternImage, 244, 157, 81)) odds_type = 1; // TRI
                    else if (imageHandler.CheckPixelColor(patternImage, 54, 99, 34)) odds_type = 2; // TRO
                    //if (imageHandler.CheckPixelColor2(patternImage, 202, 142, 95)) odds_type = 1; // TRI
                    //else if (imageHandler.CheckPixelColor(patternImage, 65, 118, 42)) odds_type = 2; // TRO
                    if (odds_type > 0) imageHandler.PreProcess_for_Saga();
                    else imageHandler._currentBitmap = new Bitmap(1, 1);
                    //if (imageHandler._currentBitmap.Width > 2) imageHandler.Resize(imageHandler._currentBitmap.Width * 3, imageHandler._currentBitmap.Height * 3);
                }
                else if (selected_track == 3)
                {

                    if (imageHandler.CheckPixelColor(patternImage, 210, 18, 2) && imageHandler.CheckPixelColor(patternImage, 210, 18, 2, 8)) odds_type = 1; // TRI
                    else if (imageHandler.CheckPixelColor(patternImage, 87, 2, 85) && imageHandler.CheckPixelColor(patternImage, 255, 255, 255, 8)) odds_type = 2; // EXA
                    else if (imageHandler.CheckPixelColor(patternImage, 205, 200, 0) && imageHandler.CheckPixelColor(patternImage, 205, 200, 0, 8)) odds_type = 3; // QNL
                    else if (imageHandler.CheckPixelColor(patternImage, 219, 207, 202) && imageHandler.CheckPixelColor(patternImage, 22, 39, 39, 8)) odds_type = 4; // WIN
                    else if (imageHandler.CheckPixelColor(patternImage, 218, 166, 0) && imageHandler.CheckPixelColor(patternImage, 218, 166, 0, 8)) odds_type = 5; // TRO
                    else if (imageHandler.CheckPixelColor(patternImage, 0, 206, 207) && imageHandler.CheckPixelColor(patternImage, 0, 206, 207, 8)) odds_type = 6; // WIDE

                    if (odds_type > 0) imageHandler.PreProcess_for_Obihiro(odds_type);
                    else imageHandler._currentBitmap = new Bitmap(1, 1);

                }
                else
                {
                    if (CheckOddsType(patternImage, 220, 256)) odds_type = 1;
                    imageHandler.SetContrast();
                    if (imageHandler._currentBitmap.Width > 2) imageHandler.Resize(imageHandler._currentBitmap.Width * 2, imageHandler._currentBitmap.Height * 2);
                }
            }
            txt_oddsType.Invoke(new Action(() => txt_oddsType.Text = odds_type.ToString()));

            Bitmap temp = (Bitmap)imageHandler._currentBitmap.Clone();
            pic_obj.Image = imageHandler._currentBitmap;

            if (!check_ocr) return;

            if (imageHandler._currentBitmap.Width < 2)
            {
                lbl_result.Invoke(new Action(() => lbl_result.Text = "Rest ... "));
                return;
            }

            if (selected_track == 32)
            {
             
                string output = imgRecog.Processing(imageHandler.test_color, imageHandler._currentBitmap.Width, imageHandler._currentBitmap.Height);
                
                if (output == "<Error>\r\n") output = "Rest ... ";
                lbl_result.Invoke(new Action(() => lbl_result.Text = output));
                
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadString("http://54.95.106.5/NAR/api_keiba_notice.php?c=16&track_id=" + selected_track.ToString() + "&odds=" + output + "&odds_type=" + odds_type.ToString());
                }
            }
            else
            {
                int pos_left = this.Left + 5;
                int pos_top = this.Top + 27;
                int pos_width = pos_left + pic_obj.Width - 100;
                int pos_height = pos_top + pic_obj.Height - 5;

                Process process = new Process
                {
                    StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    Arguments = "/C C:\\Capture\\Capture2Text_CLI.exe -s \"" + pos_left.ToString() + " "+ pos_top.ToString() +" " + pos_width.ToString() + " " + pos_height.ToString() + "\""
                }
                };
                process.Start();
                process.WaitForExit();
                if (process.HasExited)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    if (output == "<Error>\r\n") output = "Rest ... ";
                    lbl_result.Invoke(new Action(() => lbl_result.Text = output));

                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadString("http://54.95.106.5/NAR/api_keiba_notice.php?c=16&track_id=" + selected_track.ToString() + "&odds=" + output + "&odds_type=" + odds_type.ToString());
                    }
                }
            }
        }

        public IntPtr GetHandelByTitle(String wName)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(wName))
                {
                    //  Debug.Print(pList.MainWindowTitle);
                    hWnd = pList.MainWindowHandle;
                }
            }
            return hWnd;
        }

        public Image CaptureWindow(IntPtr handle, int gap_left, int gap_top)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, gap_left, gap_top, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up 
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        }

        private void cmd_track_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            int selectedIndex = cmb.SelectedIndex;
            selected_track = Convert.ToInt16(tracks[cmd_track.SelectedIndex].trackCode);
            GetTrackPosition();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            String pos_data = txt_left.Text + "-" + txt_top.Text + "-" + txt_width.Text + "-" + txt_height.Text;
            using (WebClient wc = new WebClient())
            {
                wc.DownloadString("http://54.95.106.5/NAR/api_keiba_notice.php?c=17&track_id=" + selected_track.ToString() + "&pos=" + pos_data);
            }
        }
    }

    public class Live
    {
        public string status { get; set; }
        public string hdsAbr { get; set; }
        public string hds1 { get; set; }
        public string hds3 { get; set; }
        public string hds5 { get; set; }
        public string hds2 { get; set; }
        public string hds4 { get; set; }
        public string hlsAbr { get; set; }
        public string hls1 { get; set; }
        public string hls3 { get; set; }
        public string hls5 { get; set; }
        public string hls2 { get; set; }
        public string hls4 { get; set; }
        public string thumbnail { get; set; }
    }

    public class TrackGroup
    {
        public string trackCode { get; set; }
        public string trackName { get; set; }
        public string raceNumber { get; set; }
        public string raceType { get; set; }
        public string breedAge { get; set; }
        public string raceSeq { get; set; }
        public string raceName { get; set; }
        public string raceAddName { get; set; }
        public string raceGrade { get; set; }
        public string nighttimeType { get; set; }
        public string horseCount { get; set; }
        public string startTime { get; set; }
        public string changeStartTime { get; set; }
        public string distance { get; set; }
        public string turfDart { get; set; }
        public string inoutCourse { get; set; }
        public string clockwise { get; set; }
        public string weather { get; set; }
        public string trackState { get; set; }
        public string trackMoisture { get; set; }
        public string statusFlag { get; set; }
        public string movieFlag { get; set; }
        public string movieUrl { get; set; }
        public Live live { get; set; }
    }

    public class Result
    {
        public string raceDate { get; set; }
        public string updateTime { get; set; }
        public List<TrackGroup> trackGroup { get; set; }
    }

    public class RootObject
    {
        public string version { get; set; }
        public string time { get; set; }
        public string response { get; set; }
        public string status { get; set; }
        public Result result { get; set; }
    }

    public class Item
    {
        public string Name;
        public int Value;
        public Item(string name, int value)
        {
            Name = name; Value = value;
        }
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Name;
        }
    }

}
