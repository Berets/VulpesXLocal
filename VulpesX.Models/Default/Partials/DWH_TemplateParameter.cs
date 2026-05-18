using System.Data;

namespace VulpesX.Models.Default
{
    public partial class DWH_TemplateParameter
    {
        public int Tipo { get; set; }
        public object? ParameterValue { get; set; }

        private DateTime _ParameterDate;
        public DateTime ParameterDate
        {
            get
            {
                if (Tipo == (int)SqlDbType.DateTime && !string.IsNullOrEmpty(ParameterValue?.ToString()))
                {
                    var day = ParameterValue?.ToString()?.Split('/')[0];
                    var month = ParameterValue?.ToString()?.Split('/')[1];
                    var year = ParameterValue?.ToString()?.Split('/')[2];

                    if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
                    {
                        if (year.Length > 4)
                            year = year.Substring(0, 4);

                        _ParameterDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                    }
                }
                else
                {
                    _ParameterDate = DateTime.Now;
                }
                return _ParameterDate;
            }
            set
            {
                ParameterValue = value.ToString("dd/MM/yyyy");
                _ParameterDate = value;
            }
        }
    }
}
