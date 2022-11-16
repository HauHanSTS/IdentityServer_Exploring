namespace MVC.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    public class ApiResource
    {
        public List<Resource> Resources { get; set; }
        public ApiResource()
        {
            Resources = new List<Resource>();
        }
    }
}
