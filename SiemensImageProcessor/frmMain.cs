using ClosedXML.Excel;
using DevExpress.XtraEditors;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml.Linq;
using ToastNotifications;

namespace SiemensImageProcessor
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        private string errorLogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Trace.log");
        private string config = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config.xml");
        private string templateExcelFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Template.xlsx");
        private string tesseractFolderPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Tesseract-OCR");

        private void WriteLog(LogLevel logLevel, string logMessage)
        {
            string strLevel = string.Empty;

            switch (logLevel)
            {
                case LogLevel.Debug:
                    strLevel = "[DEBUG]:";
                    break;
                case LogLevel.Error:
                    strLevel = "[ERROR]:";
                    break;
                case LogLevel.Fatal:
                    strLevel = "[FATAL]:";
                    break;
                case LogLevel.Info:
                    strLevel = "[INFO]:";
                    break;
                case LogLevel.Warn:
                    strLevel = "[WARNING]:";
                    break;
                default:
                    break;
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(this.errorLogFilePath, true, Encoding.Default))
                {
                    string message = string.Format("{0} {1} {2}", DateTime.Now.ToString(), strLevel, logMessage);

                    sw.WriteLine(message);
                    this.meLog.Text += message + Environment.NewLine;
                }
            }
            catch
            {

            }
        }

        private void WaitForMe(int ms)
        {
            new System.Threading.ManualResetEvent(false).WaitOne(ms);
        }

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Skin;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Caramel");

            this.LoadUserSettings();

            this.ActiveControl = this.teJpgFolderPath;
        }

        private void sbtnRun_Click(object sender, EventArgs e)
        {
            if (!this.bwCommon.IsBusy)
            {
                this.sbtnRun.Enabled = false;
                this.sbtnRun.Text = "ОБРАБОТКА...";

                this.bwCommon.RunWorkerAsync();
            }
            else
            {
                this.WriteLog(LogLevel.Info, "I am busy!");
            }
        }

        private void SetSelection()
        {
            this.meLog.SelectionStart = this.meLog.Text.Length;
            this.meLog.ScrollToCaret();
            this.meLog.Refresh();
        }

        private void ShowAlert(string message)
        {
            try
            {
                var toastNotification = new Notification
                (
                    "SiemensImageProcessor",
                    message,
                    60,
                    FormAnimator.AnimationMethod.Slide,
                    FormAnimator.AnimationDirection.Up
                );

                PlayNotificationSound("normal");
                toastNotification.Show();
            }
            catch (Exception ex)
            {
                this.WriteLog(LogLevel.Error, "Ошибка при отправке всплывающего сообщения: " + ex.Message);
            }
        }

        private void PlayNotificationSound(string sound)
        {
            var soundsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");
            var soundFile = Path.Combine(soundsFolder, sound + ".wav");

            using (var player = new System.Media.SoundPlayer(soundFile))
            {
                player.Play();
            }
        }

        private List<string> ExtractFromText(string text, string start, string end)
        {
            List<string> matched = new List<string>();

            text = text.ToLower();
            start = start.ToLower();
            end = end.ToLower();

            int indexStart = 0;
            int indexEnd = 0;

            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(start);
                if (indexStart != -1)
                {
                    indexEnd = indexStart + text.Substring(indexStart).IndexOf(end);

                    matched.Add(text.Substring(indexStart + start.Length, indexEnd - indexStart - start.Length));

                    text = text.Substring(indexEnd + end.Length);

                    break;
                }
                else
                {
                    exit = true;
                }
            }

            return matched;
        }

        private bool IsForFast(string name)
        {
            bool yes = false;

            List<int> yess = new List<int>();

            for (int i = 0; i < 2019; i++)
            {
                if (i < 7)
                {
                    yess.Add(i);
                    continue;
                }

                if (i % 7 == 0)
                {
                    i += 7;
                    yess.Add(i);

                    continue;
                }

                yess.Add(i);
            }

            if (name == "000")
            {
                name = "0";
            }
            else
            {
                name = name.TrimStart('0');
            }

            if (yess.Contains(int.Parse(name)))
            {
                yes = true;
            }

            return yes;
        }

        private string GetSingleCoordinate1(string jpgFilePath, string tesseractFolderPath, bool best = false, bool usePsmForBest = true, bool usePsmForFast = true)
        {
            string tempTxtFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, new FileInfo(jpgFilePath).Name.Replace(".jpg", string.Empty));

            string tesseract = Path.Combine(tesseractFolderPath, "tesseract.exe");
            string fastTessDataDir = Path.Combine(tesseractFolderPath, "tessdata");
            string bestTessDataDir = Path.Combine(tesseractFolderPath, "tessdata_best");

            string args1 = string.Empty;
            if (best == true)
            {
                if (usePsmForBest == true)
                {
                    args1 = string.Format("\"{0}\" \"{1}\" -l lat+eng --tessdata-dir \"{2}\" --psm 6", jpgFilePath, tempTxtFilePath, bestTessDataDir);
                }
                else
                {
                    args1 = string.Format("\"{0}\" \"{1}\" -l lat+eng --tessdata-dir \"{2}\"", jpgFilePath, tempTxtFilePath, bestTessDataDir);
                }
            }
            else
            {
                if (usePsmForFast == true)
                {
                    args1 = string.Format("\"{0}\" \"{1}\" -l eng --tessdata-dir \"{2}\" --psm 6", jpgFilePath, tempTxtFilePath, fastTessDataDir);
                }
                else
                {
                    args1 = string.Format("\"{0}\" \"{1}\" -l eng --tessdata-dir \"{2}\"", jpgFilePath, tempTxtFilePath, fastTessDataDir);
                }
            }

            try
            {
                string batchFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConvertJpgToTxt.bat");

                File.Delete(batchFilePath);

                StringBuilder sb = new StringBuilder();

                sb.AppendLine(string.Format("\"{0}\" {1}", tesseract, args1));

                File.WriteAllText(batchFilePath, sb.ToString(), Encoding.GetEncoding(866));

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.Arguments = " /c " + "\"" + batchFilePath + "\"";
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                this.WriteLog(LogLevel.Error, "Ошибка при конвертации JPG файла: " + ex.Message);
                throw new Exception();
            }

            tempTxtFilePath += ".txt";
            if (!File.Exists(tempTxtFilePath))
            {
                this.WriteLog(LogLevel.Error, "Не найден текстовый файл для обработки!");
                throw new Exception();
            }

            string wholeText = File.ReadAllText(tempTxtFilePath);
            if (string.IsNullOrWhiteSpace(wholeText))
            {
                this.WriteLog(LogLevel.Error, "Пустой текстовый файл для обработки!");
                throw new Exception();
            }

            string coordinate = string.Empty;

            List<string> rows = File.ReadAllLines(tempTxtFilePath).ToList();
            foreach (string row in rows)
            {
                if (row.IndexOf("ROI") != -1)
                {
                    coordinate = row.Split(':')[1].Trim();
                    break;
                }
            }

            return coordinate;
        }

        private string GetSingleCoordinate2(string jpgFilePath)
        {
            string coordinate = string.Empty;

            try
            {
                var client = new RestClient("https://api8.ocr.space/Parse/Image");

                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                request.AddHeader("Content-Type", "multipart/form-data");
                request.AddParameter("apikey", "3639381c4688957");
                request.AddParameter("language", "eng");
                request.AddParameter("isOverlayRequired", "true");
                request.AddParameter("OCREngine", "2");
                request.AddParameter("scale", "true");
                request.AddFile("image", jpgFilePath);

                this.WaitForMe(5000);

                var response = client.Execute(request);

                var obj = JObject.Parse(response.Content);

                coordinate = obj["ParsedResults"][0]["TextOverlay"]["Lines"].Where(x => x["LineText"].ToString().IndexOf("ROI") != -1).FirstOrDefault()["LineText"].ToString();
                coordinate = coordinate.Split(':')[1].Trim();
            }
            catch (Exception ex)
            {
                this.WriteLog(LogLevel.Error, "Ошибка при получении данных с веб-сервиса: " + ex.Message);
            }

            return coordinate;
        }

        private void CreateExcelFile(string excelFilePath, List<CoordinateItem> coordinates)
        {
            using (XLWorkbook wb = new XLWorkbook(excelFilePath))
            {
                if (wb != null)
                {
                    IXLWorksheet ws = wb.Worksheet(1);
                    if (ws != null)
                    {
                        int allRows = coordinates.Max(x => x.RowNumberInExcel);
                        for (int i = 0; i < allRows; i++)
                        {
                            List<CoordinateItem> coordinateForRow = coordinates.Where(x => x.RowNumberInExcel == i + 1).ToList();

                            ws.Cell(i + 2, 9).Value = coordinateForRow[0].Coordinate;
                            ws.Cell(i + 2, 10).Value = coordinateForRow[1].Coordinate + " " + coordinateForRow[2].Coordinate;
                            ws.Cell(i + 2, 11).Value = coordinateForRow[3].Coordinate + " " + coordinateForRow[4].Coordinate;
                            ws.Cell(i + 2, 12).Value = coordinateForRow[5].Coordinate + " " + coordinateForRow[6].Coordinate;

                            ws.Cell(i + 2, 14).Value = coordinateForRow[7].Coordinate;
                            ws.Cell(i + 2, 15).Value = coordinateForRow[8].Coordinate + " " + coordinateForRow[9].Coordinate;
                            ws.Cell(i + 2, 16).Value = coordinateForRow[10].Coordinate + " " + coordinateForRow[11].Coordinate;
                            ws.Cell(i + 2, 17).Value = coordinateForRow[12].Coordinate + " " + coordinateForRow[13].Coordinate;
                        }
                    }
                }

                this.WriteLog(LogLevel.Info, "Сохраняем файл...");
                wb.Save();
            }
        }

        private string GetClearSingleCoordinate(string coordinate)
        {
            string clearCoordinate = string.Empty;

            clearCoordinate = coordinate.Replace("$", "S");

            if (clearCoordinate.StartsWith("1"))
            {
                clearCoordinate = "I" + clearCoordinate.Substring(1);
            }

            clearCoordinate = clearCoordinate.Trim();

            clearCoordinate = clearCoordinate.Replace(":", string.Empty);
            clearCoordinate = clearCoordinate.Replace("(", string.Empty);
            clearCoordinate = clearCoordinate.Replace(")", string.Empty);
            clearCoordinate = clearCoordinate.TrimEnd('.');

            return clearCoordinate;
        }

        private void bwCommon_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.WriteLog(LogLevel.Info, "Начало работы программы");

            try
            {
                string jpgFolderPath = this.teJpgFolderPath.Text.Trim();

                if (string.IsNullOrWhiteSpace(jpgFolderPath))
                {
                    this.WriteLog(LogLevel.Error, "Укажите директорию с JPG файлами!");
                    return;
                }

                if (!Directory.Exists(jpgFolderPath))
                {
                    this.WriteLog(LogLevel.Error, "Указанная директория с JPG файлами не существует!");
                    return;
                }

                List<string> jpgFiles = Directory.GetFiles(jpgFolderPath, "*.jpg", SearchOption.TopDirectoryOnly).ToList();
                if (jpgFiles.Count < 1)
                {
                    this.WriteLog(LogLevel.Error, "В указанной директории с JPG файлами не найдено ни одного файла!");
                    return;
                }

                if (jpgFiles.Count % 14 != 0)
                {
                    this.WriteLog(LogLevel.Error, "В указанной директории с JPG файлами количество файлов не кратно 14!");
                    return;
                }

                if (!File.Exists(this.templateExcelFilePath))
                {
                    this.WriteLog(LogLevel.Error, "Шаблон Excel \"Template.xlsx\" файла отсутствует в директории с программой!");
                    return;
                }

                if (!Directory.Exists(this.tesseractFolderPath))
                {
                    this.WriteLog(LogLevel.Error, "Директория Tesseract-OCR отсутствует в директории с программой!");
                    return;
                }

                this.WriteLog(LogLevel.Info, "Обработка JPG файлов через Tesseract-OCR...");

                List<CoordinateItem> coordinates = new List<CoordinateItem>();

                int row = 1;
                for (int i = 0; i < jpgFiles.Count; i++)
                {
                    try
                    {
                        this.WriteLog(LogLevel.Info, "Обрабатывается JPG файл: " + jpgFiles[i]);

                        string fileName = Regex.Match(new FileInfo(jpgFiles[i]).Name.Split('.')[0], @"\d+").Value;

                        bool isForFast = this.IsForFast(fileName);

                        coordinates.Add(new CoordinateItem
                        {
                            JpgFilePath = jpgFiles[i],
                            RowNumberInExcel = row,
                            Coordinate = this.GetSingleCoordinate1(jpgFiles[i], this.tesseractFolderPath, !isForFast),
                            IsForFastTestBase = isForFast
                        });

                        if ((i + 1) % 14 == 0)
                        {
                            row++;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(LogLevel.Error, "Ошибка при обработке JPG файла: " + ex.Message);
                    }
                }

                if (coordinates.Count < 1)
                {
                    this.WriteLog(LogLevel.Error, "Нет данных из JPG файлов!");
                    return;
                }

                this.WriteLog(LogLevel.Info, "Обработка и унификация полученных координат...");

                foreach (CoordinateItem coordinate in coordinates)
                {
                    try
                    {
                        string value = coordinate.Coordinate;

                        this.WriteLog(LogLevel.Info, "Обрабатывается координата: " + value);

                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex("[ ]{2,}", options);
                        value = regex.Replace(value, " ");

                        value = value.Trim();

                        string first = value.Split(' ')[0];
                        string second = value.Split(' ')[1];
                        string third = value.Split(' ')[2];

                        first = this.GetClearSingleCoordinate(first);
                        second = this.GetClearSingleCoordinate(second);
                        third = this.GetClearSingleCoordinate(third);

                        coordinate.Coordinate = first + " " + second + " " + third;
                        coordinate.Success = true;
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog(LogLevel.Error, "Ошибка при попытке обработать координату: " + ex.Message);
                        coordinate.Success = false;
                    }
                }

                List<CoordinateItem> coordinatesFailure = new List<CoordinateItem>();
                coordinatesFailure = coordinates.Where(x => x.Success == false).ToList();

                this.WriteLog(LogLevel.Info, "Файлов на повторную обработку: " + coordinatesFailure.Count);

                if (coordinatesFailure.Count > 0)
                {
                    this.WriteLog(LogLevel.Info, "Повторная обработка JPG файлов...");

                    foreach (CoordinateItem coordinate in coordinates)
                    {
                        if (coordinate.Success == false)
                        {
                            this.WriteLog(LogLevel.Info, "Обрабатывается JPG файл: " + coordinate.JpgFilePath);

                            try
                            {
                                this.WriteLog(LogLevel.Info, "Попытка обработать JPG файл через Tesseract-OCR с другими параметрами...");

                                string value = string.Empty;

                                value = this.GetSingleCoordinate1(coordinate.JpgFilePath, this.tesseractFolderPath, !coordinate.IsForFastTestBase, false, false);

                                this.WriteLog(LogLevel.Info, "Получена координата через Tesseract-OCR: " + value);

                                if (string.IsNullOrWhiteSpace(value))
                                {
                                    this.WriteLog(LogLevel.Info, "Попытка обработать JPG файл через веб-сервис OCR.Space...");

                                    value = this.GetSingleCoordinate2(coordinate.JpgFilePath);

                                    this.WriteLog(LogLevel.Info, "Получена координата через веб-сервис OCR.Space: " + value);
                                }

                                this.WriteLog(LogLevel.Info, "Обрабатывается координата: " + value);

                                RegexOptions options = RegexOptions.None;
                                Regex regex = new Regex("[ ]{2,}", options);
                                value = regex.Replace(value, " ");

                                value = value.Trim();

                                string first = value.Split(' ')[0];
                                string second = value.Split(' ')[1];
                                string third = value.Split(' ')[2];

                                first = this.GetClearSingleCoordinate(first);
                                second = this.GetClearSingleCoordinate(second);
                                third = this.GetClearSingleCoordinate(third);

                                coordinate.Coordinate = first + " " + second + " " + third;
                                coordinate.Success = true;
                            }
                            catch (Exception ex)
                            {
                                this.WriteLog(LogLevel.Error, "Ошибка при попытке обработать координату повторно: " + ex.Message);
                                coordinate.Success = false;
                            }
                        }
                    }
                }

                if (coordinates.Where(x => x.Success == false).Count() > 0)
                {
                    this.WriteLog(LogLevel.Warn, "Требуется обработать вручную следующие файлы: " + string.Join(", ", coordinates.Where(x => x.Success == false).Select(y => y.JpgFilePath).ToList()));
                }

                this.WriteLog(LogLevel.Info, "Сохранение отчета в Excel...");

                string newExcelFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Report_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
                File.Copy(this.templateExcelFilePath, newExcelFilePath);

                this.CreateExcelFile(newExcelFilePath, coordinates);

                this.WriteLog(LogLevel.Info, "Отчет Excel сохранен в файле: " + newExcelFilePath);

                BeginInvoke(new MethodInvoker(delegate
                {
                    this.ShowAlert("Программа завершена!");
                }));
            }
            catch (Exception ex)
            {
                this.WriteLog(LogLevel.Error, "Ошибка при работе программы: " + ex.Message);
            }
        }

        private void bwCommon_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {

        }

        private void bwCommon_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            this.sbtnRun.Enabled = true;
            this.sbtnRun.Text = "СТАРТ";

            this.sbtnClose.Enabled = true;
            this.sbtnClose.Text = "ЗАКРЫТЬ";

            if (e.Error != null)
            {
                this.WriteLog(LogLevel.Info, "Программа завершена с ошибками!");
            }
            else
            {
                this.WriteLog(LogLevel.Info, "Программа завершена!");
            }
        }

        private void sbtnClose_Click(object sender, EventArgs e)
        {
            this.WriteLog(LogLevel.Info, "Нажатие кнопки ЗАКРЫТЬ...");
            Application.Exit();
        }

        private void SaveUserSettings()
        {
            XElement rootItem = new XElement("settings");

            XElement jpg = new XElement("param", new XAttribute("name", "jpgFolderPath"), new XAttribute("value", this.teJpgFolderPath.Text));
            rootItem.Add(jpg);

            try
            {
                rootItem.Save(this.config);
            }
            catch
            {
                MessageBox.Show("Не удалось сохранить XML файл с настройками элементов на форме.", this.Text);
            }
        }

        private void LoadUserSettings()
        {
            if (!File.Exists(this.config))
            {
                return;
            }

            try
            {
                XDocument doc = XDocument.Load(this.config);
                foreach (XElement el in doc.Descendants("param"))
                {
                    if (el.Attribute("name").Value == "jpgFolderPath")
                    {
                        this.teJpgFolderPath.Text = el.Attribute("value").Value;
                    }
                }

                doc = null;
            }
            catch
            {
                return;
            }

            return;
        }

        private void sbtnSelectJpgFolderPath_Click(object sender, EventArgs e)
        {
            if (this.fbdJpg.ShowDialog() == DialogResult.OK)
            {
                this.teJpgFolderPath.Text = this.fbdJpg.SelectedPath;
            }
        }

        private void meLog_EditValueChanged(object sender, EventArgs e)
        {
            this.SetSelection();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveUserSettings();
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
