﻿using Identity.Api.DataRepository;
using Identity.Api.DTO;
using Identity.Api.Interfaces;
using Identity.Api.Paginado;
using Modelo.laconcordia.Modelo.Database;

namespace Identity.Api.Services
{
    public class NacionalidadServices : INacionalidad
    {
        private NacionalidadRepository _nacionalidadRepository = new NacionalidadRepository();

        public IEnumerable<Nacionalidad> GetNacionalidadInfoAll()
        {
            return _nacionalidadRepository.NacionalidadInfoAll();
        }
        public NacionalidadDTO? GetNacionalidadById(int idNacionalidad)
        {
            return _nacionalidadRepository.GetNacionalidadById(idNacionalidad);
        }
        public void InsertNacionalidad(Nacionalidad nueva)
        {
            _nacionalidadRepository.InsertNacionalidad(nueva);
        }
        public void UpdateNacionalidad(Nacionalidad actualizada)
        {
            _nacionalidadRepository.UpdateNacionalidad(actualizada);
        }
        public void DeleteNacionalidadById(int idNacionalidad)
        {
            _nacionalidadRepository.DeleteNacionalidadById(idNacionalidad);
        }

        //paginado
        public async Task<PagedResult<Nacionalidad>> GetNacionalPaginados(int pagina, int pageSize, string? nacionalidad = null, string? estado = null)
        {
            return await _nacionalidadRepository.GetNacionalPaginados(pagina, pageSize, nacionalidad , estado);
        }
    }
}
