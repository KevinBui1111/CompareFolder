using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using WindowsApplication1;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CompareFolder
{
    public partial class frmMain : Form
    {
        HashCalculator hcal = new HashCalculator();
        string rootfolder;
        bool checkDate = false;
        Dictionary<string, string> dicSrcChecksum = new Dictionary<string, string>(),
            dicDesChecksum = new Dictionary<string, string>();

        CancellationTokenSource cancellationTokenSource;
        Task task;

        List<ResultFile> list_result = new List<ResultFile>();
        const string record_file_pattern = @"^(.+) (\w{8})$";

        public frmMain()
        {
            InitializeComponent();
        }

        private void txtSrc_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void txtSrc_DragDrop(object sender, DragEventArgs e)
        {
            TextBox txt = sender as TextBox;
            string path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

            txt.Text = Directory.Exists(path) ? path : null;
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (task != null && !task.IsCompleted)
            {
                hcal.Stop();
                cancellationTokenSource.Cancel();
            }
            else
            {
                Cursor = Cursors.WaitCursor;
                rootfolder = txtSource.Text;
                list_result.Clear();
                olvResult.ClearObjects();

                dicSrcChecksum.Clear();
                dicDesChecksum.Clear();

                string checksumfile = txtSource.Text + ".sfv";
                if (File.Exists(checksumfile)) ReadSFVChecksum(checksumfile, dicSrcChecksum);
                
                checksumfile = txtDes.Text + ".sfv";
                if (File.Exists(checksumfile)) ReadSFVChecksum(checksumfile, dicDesChecksum);

                cancellationTokenSource = new CancellationTokenSource();
                task = Task.Factory.StartNew(() => CompareFolder(txtSource.Text, txtDes.Text))
                    .ContinueWith((_) => CompleteCompare(), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        void CompareFolder(string srcFolder, string desFolder)
        {
            string foldergroup = GetRelativePath(srcFolder, rootfolder);
            ResultFolder resf = new ResultFolder { folder = foldergroup };

            String[] srcSubFolders = Directory.GetDirectories(srcFolder);
            String[] desSubFolders = Directory.GetDirectories(desFolder);
            Array.Sort(srcSubFolders);
            Array.Sort(desSubFolders);

            #region Compare Folders.

            int iSrc = 0, iDes = 0, compare;
            while (iSrc < srcSubFolders.Length && iDes < desSubFolders.Length)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Compare Files break;");
                    return;
                }

                string srcFolderName = Path.GetFileName(srcSubFolders[iSrc]);
                string desFolderName = Path.GetFileName(desSubFolders[iDes]);
                compare = srcFolderName.ToLower().CompareTo(desFolderName.ToLower());
                if (compare < 0) //src be add new.
                {
                    resf.diff_files.Add(new ResultFile
                    {
                        folder = foldergroup,
                        filename = srcFolderName,
                        fullname = srcSubFolders[iSrc],
                        isFile = false,
                        operation = Operation.NEW
                    });
                    ++iSrc;
                }
                else if (compare > 0) // des be delete.
                {
                    resf.diff_files.Add(new ResultFile
                    {
                        folder = foldergroup,
                        filename = desFolderName,
                        fullname = desSubFolders[iDes],
                        isFile = false,
                        operation = Operation.DELETE
                    });
                    ++iDes;
                }
                else
                {
                    CompareFolder(srcSubFolders[iSrc], desSubFolders[iDes]);

                    ++iSrc;
                    ++iDes;
                }
            }

            if (iSrc == srcSubFolders.Length) // add remain folder in snap2 to newfolders
            {
                for (int i = iDes; i < desSubFolders.Length; ++i)
                    resf.diff_files.Add(new ResultFile
                    {
                        folder = foldergroup,
                        filename = Path.GetFileName(desSubFolders[i]),
                        fullname = desSubFolders[i],
                        isFile = false,
                        operation = Operation.DELETE
                    });

            }
            else // add remain folders in snap1 to deletefolders.
            {
                for (int i = iSrc; i < srcSubFolders.Length; ++i)
                    resf.diff_files.Add(new ResultFile
                    {
                        folder = foldergroup,
                        filename = Path.GetFileName(srcSubFolders[i]),
                        fullname = srcSubFolders[i],
                        isFile = false,
                        operation = Operation.NEW
                    });
            }

            #endregion

            #region Compare Files.

            srcSubFolders = Directory.GetFiles(srcFolder);
            desSubFolders = Directory.GetFiles(desFolder);
            Array.Sort(srcSubFolders);
            Array.Sort(desSubFolders);

            iSrc = 0; iDes = 0;
            while (iSrc < srcSubFolders.Length && iDes < desSubFolders.Length)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Compare Files break;");
                    return;
                }

                string srcFolderName = Path.GetFileName(srcSubFolders[iSrc]);
                string desFolderName = Path.GetFileName(desSubFolders[iDes]);
                compare = srcFolderName.ToLower().CompareTo(desFolderName.ToLower());
                if (compare < 0) //src be add new.
                {
                    resf.diff_files.Add(new ResultFile
                    {
                        folder = foldergroup,
                        filename = srcFolderName,
                        fullname = srcSubFolders[iSrc],
                        isFile = true,
                        operation = Operation.NEW
                    });
                    ++iSrc;
                }
                else if (compare > 0) // des be delete.
                {
                    resf.diff_files.Add(new ResultFile
                    {
                        folder = foldergroup,
                        filename = desFolderName,
                        fullname = desSubFolders[iDes],
                        isFile = true,
                        operation = Operation.DELETE
                    });
                    ++iDes;
                }
                else
                {
                    if (CheckFileChanged(srcSubFolders[iSrc], desSubFolders[iDes]))
                        resf.diff_files.Add(new ResultFile
                        {
                            folder = foldergroup,
                            filename = srcFolderName,
                            fullname = srcSubFolders[iSrc],
                            isFile = true,
                            operation = Operation.CHANGED
                        });

                    ++iSrc;
                    ++iDes;
                }
            }

            if (iSrc == srcSubFolders.Length) // add remain folder in snap2 to newfolders
            {
                for (int i = iDes; i < desSubFolders.Length; ++i)
                    resf.diff_files.Add(new ResultFile
                    {
                        folder = foldergroup,
                        filename = Path.GetFileName(desSubFolders[i]),
                        fullname = desSubFolders[i],
                        isFile = true,
                        operation = Operation.DELETE
                    });
            }
            else // add remain folders in snap1 to deletefolders.
            {
                for (int i = iSrc; i < srcSubFolders.Length; ++i)
                    resf.diff_files.Add(new ResultFile
                    {
                        folder = foldergroup,
                        filename = Path.GetFileName(srcSubFolders[i]),
                        fullname = srcSubFolders[i],
                        isFile = true,
                        operation = Operation.NEW
                    });
            }

            #endregion

            if (resf.diff_files.Count > 0) list_result.AddRange(resf.diff_files);
        }

        private bool CheckFileChanged(string srcFile, string desFile)
        {
            FileInfo fSrc = new FileInfo(srcFile);
            FileInfo fDes = new FileInfo(desFile);

            if (fSrc.Length != fDes.Length) return true;
            if (checkDate && fSrc.LastWriteTime != fDes.LastWriteTime) return true;

            string srcCRC32 = dicSrcChecksum.ContainsKey(srcFile) ? dicSrcChecksum[srcFile] : null;
            string desCRC32 = dicDesChecksum.ContainsKey(desFile) ? dicDesChecksum[desFile] : null;

            hcal.Reset();
            if (string.IsNullOrEmpty(srcCRC32))
            {
                hcal.files.Add(srcFile);
                hcal.ThreadCalcCRC32File();
                srcCRC32 = hcal.result[0];
            }
            hcal.Reset();
            if (string.IsNullOrEmpty(desCRC32) && !cancellationTokenSource.IsCancellationRequested)
            {
                hcal.files.Add(desFile);
                hcal.ThreadCalcCRC32File();
                desCRC32 = hcal.result[0];
            }

            return srcCRC32 != desCRC32;
        }
        string GetRelativePath(string filespec, string folder)
        {
            if (filespec.StartsWith(folder))
                return filespec.Substring(folder.Length);
            else return "error";
        }

        void CompleteCompare()
        {
            Cursor = Cursors.Default;
            if (!cancellationTokenSource.IsCancellationRequested)
                olvResult.SetObjects(list_result);

            olvResult.Sort(olvColumn3);
        }
        void ReadSFVChecksum(string file, Dictionary<string, string> dic)
        {
            string folder = Path.GetDirectoryName(file);
            foreach (string line in File.ReadLines(file))
            {
                Match m = Regex.Match(line, record_file_pattern);
                if (m.Success)
                {
                    dic[folder + (folder.EndsWith("\\") ? "" : "\\") + m.Groups[1].Value] = m.Groups[2].Value;
                }
            }
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            SysImageListHelper helper = new SysImageListHelper(olvResult);
            olvColumn1.ImageGetter = delegate(object x) { return helper.GetImageIndex(((ResultFile)x).fullname); };

            txtSource.Text = @"C:\Users\khanh.buidang\Downloads\bin";
            txtDes.Text = @"c:\Users\khanh.buidang\Documents\Visual Studio 2010\Projects\WindowsFormsApplication1\WindowsFormsApplication1\bin\";
        }

        private void olvResult_BeforeCreatingGroups(object sender, BrightIdeasSoftware.CreateGroupsEventArgs e)
        {
            e.Parameters.PrimarySortOrder = SortOrder.None;
        }

        private void olvResult_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            ResultFile f = (ResultFile)e.Model;
            switch (f.operation)
            {
                case Operation.CHANGED:
                    e.Item.ForeColor = Color.DarkMagenta;
                    break;
                case Operation.NEW:
                    e.Item.ForeColor = Color.RoyalBlue;
                    break;
                case Operation.DELETE:
                    e.Item.ForeColor = Color.Crimson;
                    break;
            }

            if (!f.isFile) e.Item.Font = new Font(e.Item.Font, FontStyle.Bold);
        }
    }

    public enum Operation
    {
        NEW,
        DELETE,
        CHANGED
    }
    public class ResultFolder
    {
        public string folder { get; set; }
        public List<ResultFile> diff_files { get; set; }

        public ResultFolder()
        {
            diff_files = new List<ResultFile>();
        }
    }
    public class ResultFile
    {
        public string folder { get; set; }
        public string filename { get; set; }
        public string fullname { get; set; }
        public bool isFile { get; set; }
        public Operation operation { get; set; }
    }
}
