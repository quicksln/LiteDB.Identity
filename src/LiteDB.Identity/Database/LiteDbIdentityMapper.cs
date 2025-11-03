using LiteDB.Identity.Models;
using System;

namespace LiteDB.Identity.Database
{
    /// <summary>
    /// Specific BsonMapper configuration implementation for Identity LiteDb.
    /// </summary>
    public class LiteDbIdentityMapper
    {
        private static BsonMapper mapper = null;
        public static BsonMapper GetMapper()
        {
            if (mapper != null)
            {
                return mapper;
            }

            mapper = new BsonMapper();

            mapper.Entity<LiteDbRole>().Id(i => i.Id, true);
            // LiteDbRoleClaim does not have an Id property, so do not set Id mapping
            //mapper.Entity<LiteDbRoleClaim>().Id(i => i.Id, true);
            mapper.Entity<LiteDbRoleClaim>();

            mapper.Entity<LiteDbUser>().Id(i => i.Id, true);
            mapper.Entity<LiteDbUserClaim>().Id(i => i.Id, true);
            mapper.Entity<LiteDbUserLogin>();
            mapper.Entity<LiteDbUserToken>().Id(i => i.Id, true);

            mapper.Entity<LiteDbUserRole>().Id(i => i.Id, true);

            return mapper;
        }
    }
}
