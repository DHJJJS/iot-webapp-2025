using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;  // DB스키마 정의 클래스

namespace WpfTodoListApp.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [Required]  // NotNull. 일경우는 string에 ?(Nullable)을 삭제할 것
        [Column(TypeName = "VARCHAR(100)")] // 이거 사용안하면 컬럼이 LONGTEXT로 생성됨
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "CHAR(8)")]  // 20250610
        public string TodoDate { get; set; }

        // boolean
        public bool IsComplete { get; set; }
    }
}
