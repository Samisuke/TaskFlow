namespace TaskFlow.Core.Dto.Paginacion
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items {get; set;} = [];
        public int Pagina {get; set;}
        public int TamanoPagina {get; set;}
        public int TotalItems {get; set;}
        public int TotalPaginas {get; set;}
    }
}