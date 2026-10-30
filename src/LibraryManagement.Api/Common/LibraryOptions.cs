using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Api.Common;

public sealed class LibraryOptions
{
    public const string SectionName = "Library";

    [Required]
    [Range(1, 365, ErrorMessage = "LoanDays must be between 1 and 365")]
    public int? LoanDays { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "MaxActiveLoans must be between 1 and 100")]
    public int? MaxActiveLoans { get; set; }

    [Required]
    [Range(0, 20, ErrorMessage = "MaxRenewals must be between 0 and 20")]
    public int? MaxRenewals { get; set; }

    [Required]
    [Range(typeof(decimal), "0", "100",
    ErrorMessage = "FinePerDay must be between 0 and 100")]
    public decimal? FinePerDay { get; set; }

    [Required]
    [Range(1, 720, ErrorMessage = "ReservationHoldHours must be between 1 and 720")]
    public int? ReservationHoldHours { get; set; }
}
