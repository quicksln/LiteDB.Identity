using Microsoft.AspNetCore.Identity;

namespace LiteDB.Identity.Models
{
    public class LiteDbUserToken : IdentityUserToken<ObjectId>
    {
        public ObjectId Id { get; set; }
    }
}
