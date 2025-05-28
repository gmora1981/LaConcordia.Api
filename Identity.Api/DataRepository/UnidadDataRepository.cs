using Identity.Api.Model.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.DataRepository
{
    public class UnidadDataRepository
    {
        public List<Unidad> UnidadAll()
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return context.Unidads.ToList();
            }
        }

        public List<Unidad> UnidadXUnidad(String Item)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                return context.Unidads.Where(a => a.Unidad1 == Item).ToList();
            }
        }
        public void InsertUnidad(Unidad NewItem)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                context.Unidads.Add(NewItem);
                context.SaveChanges();
            }
        }
    }
}
