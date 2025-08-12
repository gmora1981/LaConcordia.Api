namespace Identity.Api.Interfaces
{
    public interface ISegurovidum
    {
        IEnumerable<Modelo.laconcordia.Modelo.Database.Segurovidum> GetSegurovidumInfoAll();
        List<DTO.SegurovidumDTO> GetSegurovidumByCedula(string CiAfiliado); // ahora devuelve lista
        void InsertSegurovidum(DTO.SegurovidumDTO New);
        void UpdateSegurovidum(DTO.SegurovidumDTO UpdItem);
        void DeleteSegurovidumByCedula(string CiBeneficiario);
        // Paginado
        Task<Paginado.PagedResult<Modelo.laconcordia.Modelo.Database.Segurovidum>> GetSegurovidumPaginados(
            int pagina,
            int pageSize,
            string? CiBeneficiario = null,
            string? CiAfiliado = null);
        //paginado por cedula afiliado
        Task<Paginado.PagedResult<Modelo.laconcordia.Modelo.Database.Segurovidum>> GetSegurovidumPaginadosByCedulaAfiliado(
        int pagina,
        int pageSize,
        string CiAfiliado);
    }
}
