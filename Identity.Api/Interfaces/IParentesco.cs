using Identity.Api.DTO;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Interfaces
{
    public  interface IParentesco
    {
        IEnumerable<Parentesco> GetParentescoInfoAll();
        ParentescoDTO GetParentescoById(int idParentesco);
        void InsertParentesco(Parentesco New);
        void UpdateParentesco(Parentesco UpdItem);
        void DeleteParentescoById(int idParentesco);
    }
}
