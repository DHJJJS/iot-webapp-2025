namespace MyPortfolioWebApp.Models
{
    public class AboutModel
    {
        public About? About { get; set; }
        public IEnumerable<Skill> Skill { get; set; }  // 스킬 여러건이 들어갈꺼임
    }
}
