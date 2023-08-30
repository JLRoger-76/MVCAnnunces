using MVCAnnunces.dal.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace MVCAnnunces.Repository
{
    public class AnnunceRepository : GenericRepository<Annunce>, IAnnunceRepository
    {
        public IEnumerable<Annunce> GetValid(AnnParam param)
        {

            var model = _context.Annunces
                .Where(a => a.Category.CategoryId == param.category && 
                        a.User.PostalCode.Substring(0,2) == param.department.Substring(0,2));
            if (!String.IsNullOrEmpty(param.searchString))
            {
                model = model.Where(a => a.Title.Contains(param.searchString));
            }

            switch (param.ordered)
            {
                case "Title Asc":
                    model = model.OrderBy(a => a.Title);
                    break;
                case "Title Desc":
                    model = model.OrderByDescending(a => a.Title);
                    break;
                default:
                    break;
            }
            return model.ToList();

        }

        public IEnumerable<Annunce> GetValidPaged(AnnParam param)
        {
            return GetValid(param).Skip(param.page - 1 * param.take).Take(param.take);
        }
        public IEnumerable<Annunce> GetPerimed()
        {
            return _context.Annunces.Where(a => a.Limit < DateTime.Today).ToList();
        }
        public IEnumerable<Annunce> GetByCategory(int? id)
        {
            return _context.Annunces.Include(p => p.Pictures).Where(a => a.Category.CategoryId == id).ToList();
        }
        public IEnumerable<Annunce> GetByUser(int id)
        {

            return _context.Annunces.Include(a => a.Category).Where(a => a.UserId == id).ToList();
        }
        public Annunce GetById(int? id)
        {
            return _context.Annunces.Include(p => p.Pictures).Include(a => a.Category).SingleOrDefault(a => a.AnnunceId == id);
        }

    }
}