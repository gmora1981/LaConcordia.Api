﻿using Identity.Api.DataRepository;
using Identity.Api.Interfaces;
using Modelo.laconcordia.Modelo.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Identity.Api.Services
{
    public class UnidadServices : IUnidad
    {
        public UnidadDataRepository data = new UnidadDataRepository();

        public IEnumerable<Unidad> GetUnidadInfoAll()
        {
            return data.UnidadAll();
        }

        public Unidad? GetUnidadById(string idUnidad)
        {
            return data.UnidadXUnidad(idUnidad).FirstOrDefault();
        }
        public void InsertUnidad(Unidad nueva)
        {
            data.InsertUnidad(nueva);
        }
        public void UpdateUnidad(Unidad actualizada)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                context.Unidads.Update(actualizada);
                context.SaveChanges();
            }
        }
        public void DeleteUnidadById(string idUnidad)
        {
            using (var context = new DbAa5796GmoraContext())
            {
                var item = context.Unidads.FirstOrDefault(x => x.Unidad1 == idUnidad);
                if (item != null)
                {
                    context.Unidads.Remove(item);
                    context.SaveChanges();
                }
            }
        }

    }
}
