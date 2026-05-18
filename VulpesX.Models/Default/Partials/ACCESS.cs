namespace VulpesX.Models.Default
{
    public partial class ACCESS
    {
        public List<SOCBASE>? EnabledCompanies { get; set; }
        public SOCBASE? SelectedCompany { get; set; }
       
        public AUTH_ACCESS_ROLES? Roles { get; set; }

        public string? utevx { get; set; }
    }
}
