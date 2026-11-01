using Hospital_Management_System.Domain.Common;
using Hospital_Management_System.Domain.Enums;

namespace Hospital_Management_System.Domain.Entities
{

    public class Invoice : BaseEntity
    {
        public int AppointmentId { get; set; }

        public Appointment Appointment { get; set; } = null!;

        public int PatientId { get; set; }

        public Patient Patient { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount => TotalAmount - PaidAmount;

        public PaymentStatus Status { get; set; } = PaymentStatus.Unpaid;

        public DateTime? PaidAt { get; set; }
        public string? Description
        {
            get; set;
        }
    }
}