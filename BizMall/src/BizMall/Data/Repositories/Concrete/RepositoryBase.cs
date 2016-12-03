using System;
using BizMall.Data.DBContexts;


namespace BizMall.Data.Repositories.Concrete
{
    //Нужен д.т.ч. не объявлять в каждом классе репозитория context
    public class RepositoryBase : IDisposable
    {
        private bool disposing = false;

        public RepositoryBase(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public ApplicationDbContext _ctx { get; set; }
        public ApplicationDbContext _ctx2 { get; set; }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposing)
            {
                if (disposing)
                {
                    this._ctx.Dispose();
                }
            }
            this.disposing = true;

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
