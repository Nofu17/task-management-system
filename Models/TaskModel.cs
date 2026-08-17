using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Models
{
    public class TaskModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        // ربط المهمة بالمستخدم
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; } // nullable حتى لا يسبب مشكلة ModelState
    }
}
 

