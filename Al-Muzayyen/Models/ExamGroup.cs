using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Al_Muzayyen.Models
{
    public class ExamGroup
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Exam")]
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        [ForeignKey("AvailableSlot")]
        public int SlotId { get; set; }
        public Available_slot AvailableSlot { get; set; }

        public DateTime AssignedAt { get; set; }= DateTime.Now;
    }
}