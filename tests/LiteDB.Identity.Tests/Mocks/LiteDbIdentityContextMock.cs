using LiteDB.Identity.Database;
using System;
using System.IO;

namespace LiteDB.Identity.Tests.Mocks
{
    /// <summary>
    /// InMemory LiteDb implementation for testing purpose
    /// </summary>
    internal class LiteDbIdentityContextMock  : ILiteDbIdentityContext
    {
        private readonly ILiteDatabase liteDatabase;
        public LiteDbIdentityContextMock() {
            liteDatabase = new LiteDatabase(new MemoryStream(), LiteDbIdentityMapper.GetMapper());
        }

        public ILiteDatabase LiteDatabase
        {
            get
            {
                return liteDatabase;
            }
        }
    }
}
