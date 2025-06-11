using Microsoft.EntityFrameworkCore;

namespace WebApiApp01.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }

        // 테이블 연결
        // 클래스보다 뒤 객체변수명하고 테이블하고 동일하게 생성필요
        // MySQL에 TodoItems 이름으로 테이블이 생성되고 연결.
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
