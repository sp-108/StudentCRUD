using System.ComponentModel.DataAnnotations.Schema;
namespace StudentCRUD.Models
{
    public class Student
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Course { get; set; }
        public int Age { get; set; }
        public string? ImagePath {  get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
