using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Threading;

public static class DateHelper
{
    public static string ToHumanDate(this DateTime? date)
    {
        if (!date.HasValue) return null;

        DateTime d = date.Value;
        DateTime now = DateTime.Now;
        // 1.
        // Get time span elapsed since the date.
        TimeSpan s = now.Subtract(d);

        // 2.
        // Get total number of days elapsed.
        int dayDiff = (int)s.TotalDays;

        // 3.
        // Get total number of seconds elapsed.
        int secDiff = (int)s.TotalSeconds;

        // 5.
        // Handle same-day times.
        if (dayDiff == 0)
        {
            // A.
            // Less than one minute ago.
            if (secDiff < 60)
            {
                return "just now";
            }
            // B.
            // Less than 2 minutes ago.
            if (secDiff < 120)
            {
                return "1 minute ago";
            }
            // C.
            // Less than one hour ago.
            if (secDiff < 3600)
            {
                return string.Format("{0} minutes ago",
                    Math.Floor((double)secDiff / 60));
            }
            // D.
            // Less than 2 hours ago.
            if (secDiff < 7200)
            {
                return "1 hour ago";
            }
            // E.
            // Less than one day ago.
            if (secDiff < 86400)
            {
                return string.Format("{0} hours ago",
                    Math.Floor((double)secDiff / 3600));
            }
        }
        // 6.
        // Handle previous days.
        if (dayDiff == 1)
        {
            return "yesterday";
        }
        if (dayDiff < 7)
        {
            return string.Format("{0} days ago",
            dayDiff);
        }
        if (dayDiff < 31)
        {
            return string.Format("{0} weeks ago",
            Math.Ceiling((double)dayDiff / 7));
        }

        if (now.AddMonths(-2) < d)
            return "a month ago";

        if (now.AddYears(-1) < d)
            return string.Format("{0} months ago", now.Month - d.Month + 12 * (now.Year - d.Year));

        if (now.AddYears(-2) < d)
            return "1 year ago";

        return string.Format("{0} years ago", now.Year - d.Year);
    }
    public static long ToUnixTime(this DateTime date)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
    }
    public static long? ToUnixTime(this DateTime? date)
    {
        if (date == null) return null;
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((date.Value.ToUniversalTime() - epoch).TotalSeconds);
    }
    public static DateTime FromUnixTime(this long unixTime)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddSeconds(unixTime).ToLocalTime();
    }
    public static string ToReadableSize(this long? size)
    {
        if (size == null) return null;

        const long ONE_GB = 1024 * 1024 * 1024;
        const long ONE_MB = 1024 * 1024;
        const long ONE_KB = 1024;

        if (size < ONE_KB)
            return string.Format("{0} Bytes", size);

        if (size < ONE_MB)
            return string.Format("{0} KB", size / ONE_KB);

        if (size < ONE_GB)
            return string.Format("{0} MB", size / ONE_MB);

        return string.Format("{0:0.##} GB", 1f * size / ONE_GB);
    }
}

public class JobWalkDirectories
{
    public List<string> exceptionFol { get; set; }
    public List<SearchPath> searchFols { get; set; }
    public bool m_bSystem { get; set; }
    public string[] m_ext { get; set; }

    public event ProgressChangedEventHandler ProgressChanged;
    public event RunWorkerCompletedEventHandler RunWorkerCompleted;

    public delegate void GotFileEventHandler(object sender, FileInfo f);
    public event GotFileEventHandler GotAFile;

    private BackgroundWorker bgw;
    private ManualResetEvent _pauseEvent = new ManualResetEvent(true);

    public JobWalkDirectories()
    {
        exceptionFol = new List<string>();
        m_ext = new string[] { ".*" };
        m_bSystem = true;

        bgw = new BackgroundWorker();
        bgw.WorkerSupportsCancellation = true;
        bgw.WorkerReportsProgress = true;
        bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
        bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
        bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
    }

    void bgw_DoWork(object sender, DoWorkEventArgs e)
    {
        foreach (SearchPath item in searchFols)
        {
            if (item.m_subFolder)
                WalkDirectories(item.folder, 0);
            else
                WalkDirectories(item.folder);
        }

        //e.Result = resListFile;
    }
    void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        if (ProgressChanged != null) ProgressChanged(sender, e);
    }
    void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (RunWorkerCompleted != null) RunWorkerCompleted(sender, e);
    }

    public void GetFilesAsync()
    {
        bgw.RunWorkerAsync();
    }
    public void Cancel()
    {
        bgw.CancelAsync();
        Resume();
    }
    public void Pause()
    {
        _pauseEvent.Reset();
    }
    public void Resume()
    {
        _pauseEvent.Set();
    }

    /// <summary>
    /// Get all file in dir.
    /// </summary>
    /// <param name="subFolder">Specify if subfolder can be gotten</param>
    /// <param name="exceptionFol"> list of folders that can be omitted</param>
    private void WalkDirectories(string sPath, int level)
    {
        _pauseEvent.WaitOne();
        if (bgw.CancellationPending) return;

        bool valid = true;

        // Get Sub Folders.
        try
        {
            if (level < 3) bgw.ReportProgress(0, sPath);

            String[] subFolders = Directory.GetDirectories(sPath);
            foreach (string dir in subFolders)
            {
                valid = (File.GetAttributes(dir) & FileAttributes.System) == 0 || this.m_bSystem;
                if (!valid) continue;

                foreach (string item in exceptionFol)
                    if (item == dir)
                    {
                        valid = false;
                        break;
                    }

                if (valid) WalkDirectories(dir, level + 1);
            }
            // Get files in  folder.
            WalkDirectories(sPath);
        }
        catch (UnauthorizedAccessException) { }
    }

    /// <summary>
    /// Get all file in dir, only top-level.
    /// </summary>
    private void WalkDirectories(string sPath)
    {
        _pauseEvent.WaitOne();
        if (bgw.CancellationPending) return;

        DirectoryInfo dir = new DirectoryInfo(sPath);
        bool matchfiletype = false;

        foreach (FileInfo file in dir.GetFiles())
        {
            _pauseEvent.WaitOne();
            if (bgw.CancellationPending) return;

            if ((file.Attributes & FileAttributes.System) != 0 && !this.m_bSystem) continue;

            matchfiletype = false;
            // Check where file is matched with pattern.
            foreach (string ext in this.m_ext)
            {
                if (ext == ".*" || ext.Equals(file.Extension, StringComparison.OrdinalIgnoreCase))
                {
                    matchfiletype = true;
                    break;
                }
            }

            if (matchfiletype && GotAFile != null)
                GotAFile(this, file);
        }
    }

    public class SearchPath
    {
        public string folder { get; set; }
        public bool m_subFolder { get; set; }
    }
}

