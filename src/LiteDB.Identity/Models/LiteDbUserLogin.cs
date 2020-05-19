using Microsoft.AspNetCore.Identity;
using System;

namespace LiteDB.Identity.Models
{
    public class LiteDbUserLogin : IdentityUserLogin<ObjectId>
    {
        public ObjectId Id { get; set; }
    }
}
