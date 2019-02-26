using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using winForm = System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace EffectiveFix
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 檔案路徑選取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPath_Click(object sender, RoutedEventArgs e)
        {
            winForm.FolderBrowserDialog path = new winForm.FolderBrowserDialog();
            path.ShowDialog();
            txtPath.Text = path.SelectedPath;
        }

        /// <summary>
        /// 更新生效日
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            string newEftDate, oldEftDate, extension, strLine1, content, fileName;
            string backPath, folderPath;

            folderPath = txtPath.Text;

            if (!String.IsNullOrEmpty(folderPath))
            {
                //擷取字串，只留日期的數字
                DateTime date = DateTime.Parse(datepicker1.SelectedDate.ToString());
                newEftDate = date.ToString("yyyyMMdd");

                //資料夾全部檔案
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    //副檔名
                    extension = System.IO.Path.GetExtension(filePath);
                    //檔案名稱（含副檔名）
                    fileName = System.IO.Path.GetFileName(filePath);

                    //xml檔不更改日期
                    if (extension != ".xml")
                    {
                        //依照檔案編碼方式，讀取檔案
                        StreamReader sr = new StreamReader(filePath, Encoding.Default);

                        //讀取一行
                        strLine1 = sr.ReadLine();

                        if (!String.IsNullOrEmpty(strLine1))
                        {
                            //判斷表頭
                            if (strLine1.Substring(0, 2) == "H0")
                            {
                                //讀取表身
                                content = sr.ReadToEnd();
                                sr.Close();

                                //擷取舊的生效日
                                oldEftDate = strLine1.Substring(2, 8);
                                //新的生效日取代舊的生效日
                                strLine1 = strLine1.Replace(oldEftDate, newEftDate);

                                //備份檔案
                                backPath = folderPath + "\\backup";
                                Directory.CreateDirectory(backPath);
                                File.Copy(filePath, backPath + "\\" + fileName, true);

                                //刪除舊資料
                                File.Delete(filePath);

                                //依照資料編碼方式寫入檔案
                                StreamWriter sw = new StreamWriter(filePath, true, Encoding.Default);
                                //寫入資料表頭
                                sw.WriteLine(strLine1);
                                //寫入表身
                                sw.Write(content);
                                sw.Close();
                            }
                        }
                    }
                }
                MessageBox.Show("設定完成");
            }
            else
            {
                MessageBox.Show("請輸入路徑");
            }           
        }

        /// <summary>
        /// 執行DataUnput.bat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Process[] process = Process.GetProcessesByName("DataInputNet");

            //判斷背景是否有執行"DataInputNet"
            if (process.Length > 0)
            {
                MessageBox.Show("程式執行中, 請稍後再試");
            }
            else
            {
                //執行程式
                Process.Start(@"D:\Bakofice\DataInputNet.bat");
            }           
        }

        /// <summary>
        /// 將主檔搬移至更新資料夾
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            string sourceFile, fileName;

            sourceFile = txtPath.Text;

            if (!String.IsNullOrEmpty(sourceFile))
            {
                //檢查路徑是否存在
                if (Directory.Exists(@"C:\\ADDM\\VAN\\RCV") == false)
                {
                    Directory.CreateDirectory(@"C:\ADDM\VAN\RCV");
                }
                else
                {
                    //複製檔案,不包含資料夾
                    foreach (string filePath in Directory.GetFiles(sourceFile))
                    {
                        fileName = System.IO.Path.GetFileName(filePath);
                        File.Copy(filePath, System.IO.Path.Combine(@"C:\ADDM\VAN\RCV", fileName), true);
                    }
                    MessageBox.Show("檔案搬移完成");
                }
            } 
            else
            {
                MessageBox.Show("請輸入路徑");
            }
        }
    }
}
