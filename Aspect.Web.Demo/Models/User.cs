
using MessagePack;

namespace Aspect.Web.Demo.Models
{
    [MessagePackObject(true)]
    public class User
    {
        [Dapper.Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}