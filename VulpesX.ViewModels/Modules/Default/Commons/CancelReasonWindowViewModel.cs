using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Commons
{
    public class CancelReasonWindowViewModel : Base
    {
        public int MinSize { get; set; } = 15;

        public CancelReasonWindowViewModel()
        {
        }

        private int currentSize;
        public int CurrentSize
        {
            get { return currentSize; }
            set
            {
                currentSize = value;
                NotifyPropertyChanged("CurrenSize");
                NotifyPropertyChanged("CurrentSizeText");
                NotifyPropertyChanged("CurrentSizeColor");
            }
        }
        public string CurrentSizeText
        {
            get => $"{currentSize} / 255";
        }

        public string CurrentSizeColor
        {
            get
            {
                if (currentSize < MinSize)
                    return "O";
                else
                    return "G";
            }
        }

        public string? SelectedReason { get; set; }
    }
}
