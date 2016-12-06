using tessnet2;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace QwopStyle.Source
{
    public enum FinalState { CRASHED, STOPPED }

    class QwopOcr
    {
        [DllImport("User32.dll")]
        private static extern Int32 SetForegroundWindow(int hWnd);
        
        private string imagePath;
        private Tesseract ocr;

        public string Score { get; set; }
        public double Confidence { get; set; }
        public QwopOcr()
        {
            imagePath = @".\Test.png";
            ocr = new Tesseract();
            ocr.Init(@"C:\Users\Brendan\Desktop\Tesseract-OCR\tessdata", "eng", false);
        }

        public QwopOcr(string filePath)
        {
            imagePath = filePath;
            ocr = new Tesseract();
            ocr.Init(@"C:\Users\Brendan\Desktop\Tesseract-OCR\tessdata", "eng", false);
        }

        public void UpdateScore()
        {
            // move the flashplayer window to the foreground
            var proc = Process.GetProcessesByName("Flashplayer")[0];
            SetForegroundWindow(proc.MainWindowHandle.ToInt32());

            // take a screenshot
            Bitmap bmpScreenshot = new Bitmap(180, 53);
            Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            gfxScreenshot.CopyFromScreen(218, 65, 0, 0, bmpScreenshot.Size);
            bmpScreenshot.Save(imagePath, ImageFormat.Png); // save to this location

            try
            {
                var result = ocr.DoOCR(bmpScreenshot, Rectangle.Empty);
                
                this.Confidence = result[0].Confidence;
                this.Score = result[0].Text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.Confidence = -1;
                this.Score = "";
            }
        }

        public FinalState GetFinalState()
        {
            // move the flashplayer window to the foreground
            var proc = Process.GetProcessesByName("Flashplayer")[0];
            SetForegroundWindow(proc.MainWindowHandle.ToInt32());

            // take a screenshot
            Bitmap bmpScreenshot = new Bitmap(250, 47);
            Graphics gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            gfxScreenshot.CopyFromScreen(200, 155, 0, 0, bmpScreenshot.Size);
            bmpScreenshot.Save(imagePath, ImageFormat.Png); // save to this location

            try
            {
                var result = ocr.DoOCR(bmpScreenshot, Rectangle.Empty);
                //Console.WriteLine(result[0].Text + result[1].Text);

                if (!result[0].Text.Contains("PART"))
                {
                    return FinalState.STOPPED;
                }
                else
                {
                    return FinalState.CRASHED;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                this.Confidence = -1;
                this.Score = "";

                return FinalState.CRASHED;
            }
        }
    }
}
