namespace Hospital_Management_System.Application.Common.Models
{
    public class PagedResult<T> 
    {
        public IReadOnlyList<T> Items { get; } 

        public int PageNumber { get; } 

        public int PageSize { get; } 

        public int TotalCount { get; } 

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize); 

        public bool HasPrevious => PageNumber > 1; 

        public bool HasNext => PageNumber < TotalPages; 

        public PagedResult(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items; 
            TotalCount = totalCount; 
            PageNumber = pageNumber; 
            PageSize = pageSize; 
        }

        public PagedResult<TOut> Map<TOut>(Func<T, TOut> mapper) 
        {
            var mapped = Items.Select(mapper).ToList(); 
            return new PagedResult<TOut>(mapped, TotalCount, PageNumber, PageSize); 
        }
    }
}
