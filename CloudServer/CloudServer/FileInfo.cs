using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer
{
  public class FileInfo
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}

//[ForeignKey("Group")]
//public int GroupId { get; set; }
//public Group Group { get; set; }