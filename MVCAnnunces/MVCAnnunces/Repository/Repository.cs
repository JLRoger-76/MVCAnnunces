using MVCAnnunces.dal;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;


namespace MVCAnnunces.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {

        public MyEntities _context = null;
        private DbSet<T> table = null;
        public GenericRepository()
        {
            this._context = new MyEntities();
            table = _context.Set<T>();
        }
        public GenericRepository(MyEntities _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }
        public T GetById(object id)
        {
            return table.Find(id);
        }
        public void Insert(T obj)
        {
            table.Add(obj);
        }
        public void Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}