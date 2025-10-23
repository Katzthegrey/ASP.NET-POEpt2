namespace POEpt1.ViewModels.Claims
{
    public class ClaimDetailsViewModel
    {
        public int ClaimID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime ClaimDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        // File information
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }

        // Approver information (if approved)
        public string ApprovedByName { get; set; } = string.Empty;
        public DateTime? ApprovedDate { get; set; }

        // Helper properties for the view
        public string FormattedFileSize => FormatFileSize(FileSize);
        public bool CanDownload => !string.IsNullOrEmpty(FileName);
        public bool CanApprove => Status == "Pending"; // For coordinators/managers

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
