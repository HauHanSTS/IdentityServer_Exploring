namespace API_01.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    public class ApiResource
    {
        public static List<Resource> Resources
        {
            get
            {
                return new List<Resource> { 
                    new Resource
                    {
                        Id = 1,
                        Name = "Api 01 Resource 1"
                    },
                    new Resource
                    {
                        Id = 2,
                        Name = "Api 01 Resource 2"
                    }
                };
            }
        }
    }
}
