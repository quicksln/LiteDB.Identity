using Microsoft.AspNetCore.Identity;

namespace LiteDB.Identity.Models
{
    public class LiteDbUserRole : IdentityUserRole<ObjectId>
    {
        public ObjectId Id { get; set; }
    }
}
