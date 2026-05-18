using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models
{
    public class MenuModel : Base
    {
        public required string Name { get; set; }
        public string? Uri { get; set; }

        private bool isEnabled;
        public bool IsEnabled { get { return isEnabled; } set { isEnabled = value; NotifyPropertyChanged(); } }
        public bool IsEnabledChanging { get { return !string.IsNullOrEmpty(Uri); } }
        public bool IsExpanded { get; set; }
        public object[]? Parameters { get; set; }

        public string? Username { get; set; }

        public List<MenuModel>? SubItems { get; set; }
    }
}
