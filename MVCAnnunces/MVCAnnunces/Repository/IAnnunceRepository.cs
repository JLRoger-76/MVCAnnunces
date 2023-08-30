using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCAnnunces.dal.Entity;

namespace MVCAnnunces.Repository
{
    public interface IAnnunceRepository : IRepository<Annunce>
    {
        IEnumerable<Annunce> GetValid(AnnParam param);
        IEnumerable<Annunce> GetValidPaged(AnnParam param);
        IEnumerable<Annunce> GetPerimed();
        IEnumerable<Annunce> GetByCategory(int? id);
        IEnumerable<Annunce> GetByUser(int id);
        Annunce GetById(int? id);
    }
}

