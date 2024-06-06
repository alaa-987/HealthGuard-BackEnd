using HealthGuard.GradProject.DTO;

namespace HealthGuard.GradProject.Helpers
{
    public class Pagination<T>
    {

        //private IEnumerable<ProductToReturnDto> data;
       // public int PageSize { get; set; }
        //public int PageIndex { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; }
        public Pagination(/*int pageIndex, int pageSize,*/ int count, IReadOnlyList<T> data)
        {
            //PageIndex = pageIndex;
            //PageSize = pageSize;
            Count = count;
            Data = data;
        }
    }
}
