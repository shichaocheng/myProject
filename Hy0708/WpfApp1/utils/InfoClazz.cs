using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.utils
{
    class InfoClazz
    {
    }
    public partial class Information
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public string Unit { get; set; }

        public string DataMin { get; set; }

        public string DataMax { get; set; }
    }
    public partial class SettingInfo : PropertyChangedBase
    {
        private bool selected;
        public bool Selected
        {
            get { return (bool)selected; }
            set { selected = value; Notify("Selected"); }
        }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Scope { get; set; }
        public string DataDecimal { get; set; }
        public string datalen { get; set; }

    }
    public partial class RealInfo
    {
        public string RegNum { get; set; }
        public string StartAddress { get; set; }
        public string[] Address { get; set; }
        public string[] DataLen { get; set; }
        public string[] DataSign { get; set; }
        public string[] DataDecimal { get; set; }
    }
    public partial class SetInfo
    {
        public string RegNum { get; set; }
        public string StartAddress { get; set; }
        public string[] Address { get; set; }
        public string[] DataLen { get; set; }
        public string[] DataSign { get; set; }
        public string[] DataDecimal { get; set; }
    }
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class CorrugatedInfo : PropertyChangedBase
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; Notify("Selected"); }
        }
        public string Name { get; set; }
        public int AllPoints { get; set; }
        public int FaultPoint { get; set; }
        public int AfterTime { get; set; }
        public int BehindPoints { get; set; }
        public Color LineColor { get; set; }
        public CorrugatedInfo()
        {

        }
        public CorrugatedInfo(string name, int allpoints, int faultpoint, int afterTime, int behindpoints, Color lineColor)
        {
            Name = name;
            AllPoints = allpoints;
            FaultPoint = faultpoint;
            AfterTime = afterTime;
            BehindPoints = behindpoints;
            LineColor = lineColor;
        }

    }

    public class IsState
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public partial class upgradeInfo : PropertyChangedBase
    {
        private bool selected;
        public bool Selected
        {
            get { return (bool)selected; }
            set { selected = value; Notify("Selected"); }
        }
        public string Name { get; set; }
        public string FirmwareCode { get; set; }
        public string FirmwareVersion { get; set; }
       // public int Progress { get; set; }
        public string ProgressValue { get; set; }
        public string Result { get; set; }
    }

}
