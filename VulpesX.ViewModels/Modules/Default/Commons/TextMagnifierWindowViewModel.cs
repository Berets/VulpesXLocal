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
    public class TextMagnifierWindowViewModel : Base
    {
        public required int MaxSize { get; set; }
        public required string SourceText { get; set; }

        public TextMagnifierWindowViewModel()
        {
        }

        public string? SelectedText { get; set; }


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
            get => MaxSize > 0 ? $"{currentSize} / {MaxSize}" : $"{currentSize} / ∞";
        }
        public string CurrentSizeColor
        {
            get
            {
                if (MaxSize == 0)
                    return "G";
                if (currentSize > MaxSize)
                    return "O";
                else
                    return "G";
            }
        }
    }
}
