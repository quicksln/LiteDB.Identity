using LiteDB.Identity.Models;
using System;

namespace LiteDB.Identity.Database
{
    /// <summary>
    /// Specific BsonMapper configuration implementation for Identity LiteDb.
    /// </summary>
    public class LiteDbIdentityMapper
    {
        private static readonly object syncRoot = new object();
        private static BsonMapper mapper = null;
        public static BsonMapper GetMapper()
        {
            if (mapper != null)
            {
                return mapper;
            }

            lock (syncRoot)
            {
                if (mapper != null)
                {
                    return mapper;
                }

                // Build and fully configure the mapper before publishing it,
                // so concurrent callers never observe a partially configured instance.
                var newMapper = new BsonMapper();

                newMapper.Entity<LiteDbRole>().Id(i => i.Id, true);
                // LiteDbRoleClaim does not have an Id property, so do not set Id mapping
                //newMapper.Entity<LiteDbRoleClaim>().Id(i => i.Id, true);
                newMapper.Entity<LiteDbRoleClaim>();

                newMapper.Entity<LiteDbUser>().Id(i => i.Id, true);
                newMapper.Entity<LiteDbUserClaim>().Id(i => i.Id, true);
                newMapper.Entity<LiteDbUserLogin>();
                newMapper.Entity<LiteDbUserToken>().Id(i => i.Id, true);

                newMapper.Entity<LiteDbUserRole>().Id(i => i.Id, true);

                mapper = newMapper;
                return mapper;
            }
        }
    }
}
