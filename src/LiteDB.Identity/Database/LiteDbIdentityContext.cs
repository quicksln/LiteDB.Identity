using System;

namespace LiteDB.Identity.Database
{
    public class LiteDbIdentityContext : ILiteDbIdentityContext, IDisposable
    {
        private readonly LiteDatabase liteDatabase;
        public LiteDbIdentityContext(string connectionStringName)
        {
            try
            {
                if(string.IsNullOrEmpty(connectionStringName))
                {
                    throw new ArgumentNullException("LiteDbIdentity", "LiteDbIdentity connection string is missing in appsettings.json configuration file");
                }

                liteDatabase = new LiteDatabase(connectionStringName, LiteDbIdentityMapper.GetMapper());
            }
            catch (Exception)
            {
                // add logger 
                throw;
            }
        }

        public ILiteDatabase LiteDatabase
        {
            get
            {
                ThrowIfDisposed();
                return liteDatabase;
            }
        }

        protected void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #region IDisposable implementation 

        private bool disposed = false;

        // Public implementation of Dispose pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                liteDatabase.Dispose();
            }

            disposed = true;
        }

        ~LiteDbIdentityContext()
        {
            Dispose(false);
        }

        #endregion
    }
}
