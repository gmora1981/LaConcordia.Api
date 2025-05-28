using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Identity.Api.Services
{
    public class UnidadServices : IUnidad
    {
        public UnidadDataRepository data = new UnidadDataRepository();

        public IEnumerable<Unidad> UnidadInfoAll
        {
            get { return data.UnidadAll(); }
        }
        public IEnumerable<Unidad> UnidadXUnidad(string Item)
        {
             return data.UnidadXUnidad(Item); 
        }
        public void InsertUnidad(Unidad New)
        {
            data.InsertUnidad(New);
        }
    }
}
