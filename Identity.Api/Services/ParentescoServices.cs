using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class ParentescoServices : IParentesco
    {
        private ParentescoRepository _parentesco = new ParentescoRepository();


        public IEnumerable<Parentesco> GetParentescoInfoAll()
        {
            return _parentesco.ParentescoInfoAll();
        }

        public ParentescoDTO GetParentescoById(int idParentesco)
        {
            return _parentesco.GetParentescoById(idParentesco);
        }

        public void InsertParentesco(Parentesco New)
        {
            _parentesco.InsertParentesco(New);
        }

        public void UpdateParentesco(Parentesco UpdItem)
        {
            _parentesco.UpdateParentesco(UpdItem);
        }

        public void DeleteParentescoById(int idParentesco)
        {
            _parentesco.DeleteParentescoById(idParentesco);
        }
    }
}
