namespace Identity.Api.Paginado
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalItems { get; set; }       // Total de registros sin paginar
        public int Page { get; set; }             // Página actual
        public int PageSize { get; set; }         // Tamaño de página
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize); // Total de páginas
    }
}
