using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using DualMonitor.Win32;
using DualMonitor.Entities;

namespace DualMonitor
{
    public class RuleManager
    {
        private string _userDataPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName, "IconsCache");
        private List<Rule> _rules;
        private ProcessLogic _processLogic;

        public RuleManager(List<Rule> rules, ProcessLogic processLogic) 
        {
            _rules = rules;
            _processLogic = processLogic;
        }

        internal void RemoveRule(Rule rule)
        {
            int index = _rules.FindIndex(r => RuleComparer(r, rule));

            if (index >= 0)
            {
                _rules.RemoveAt(index);
            }
        }

        private bool RuleComparer(Rule r1, Rule r2)
        {
            return r1.Id == r2.Id;
        }

        internal void AddRule(Rule r1)
        {
            _rules.Add(r1);
        }

        internal IEnumerable<Rule> GetRules()
        {
            return _rules;
        }

        internal string SaveIcon(Icon icon)
        {
            Bitmap bmp = icon.ToBitmap();
            if (!Directory.Exists(_userDataPath)) Directory.CreateDirectory(_userDataPath);

            string filename = _userDataPath + "\\" + Guid.NewGuid().ToString() + ".png";
            if (!saveJpeg(filename, bmp, 100))
            {
                return null;
            }

            return filename;
        }

        private bool saveJpeg(string path, Bitmap img, long quality)
        {
            // Encoder parameter for image quality
            EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            // Jpeg image codec
            ImageCodecInfo jpegCodec = this.GetEncoderInfo(ImageFormat.Png);

            if (jpegCodec == null)
                return false;

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);

            return true;
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].FormatID == format.Guid)
                    return encoders[j];
            }
            return null;
        }

        internal void MatchRule(IntPtr hwnd)
        {
            string program = ProcessUtil.GetProcessPathByWindowHandle(hwnd);
            string className = Native.GetClassName(hwnd);
            string caption = Native.GetWindowText(hwnd);

            if (_rules != null)
            {
                foreach (var rule in _rules)
                {
                    if (rule.UseClass && className.IndexOf(rule.Class, StringComparison.CurrentCultureIgnoreCase) < 0)
                    {
                        continue;
                    }

                    if (rule.UseCaption && caption.IndexOf(rule.Caption, StringComparison.CurrentCultureIgnoreCase) < 0)
                    {
                        continue;
                    }

                    if (rule.UseProgram && (program == null || !program.Equals(rule.Program, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        continue;
                    }

                    RuleActions.HandleMoveAction(rule.MoveAction, hwnd, _processLogic);
                }
            }
        }

        internal void Init()
        {
            var di = new DirectoryInfo(_userDataPath);
            if (!di.Exists) return;

            foreach (var item in di.GetFiles("*.png"))
            {
                if (!_rules.Any(r => r.Icon != null && r.Icon.Equals(item.FullName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    try
                    {
                        item.Delete();
                    }
                    catch
                    {
                        // what to do, what to do ?
                    }
                }
            }
        }

        internal static List<Rule> Clone(IEnumerable<Rule> list)
        {
            // use a copy of the existing rules and work with this list            
            var rules = new List<DualMonitor.Entities.Rule>();
            foreach (var rule in list)
            {
                rules.Add(rule.Clone());
            }
            return rules;
        }
    }
}
