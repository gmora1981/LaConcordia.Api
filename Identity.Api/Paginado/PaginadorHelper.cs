namespace Identity.Api.Paginado
{
    public static class PaginadorHelper
    {
        public const int NumeroDeDatosPorPagina = 8;

        public static int CalcularTotalPaginas(int totalItems, int pageSize)
        {
            return (int)Math.Ceiling(totalItems / (double)pageSize);

        }
    }
}
