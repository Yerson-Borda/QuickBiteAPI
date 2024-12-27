namespace DTO.DishDtos
{
    public class PagedResponse<T>
    {
        public List<T> Dishes { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public int Size { get; set; }
        public int Count { get; set; }
        public int Current { get; set; }
    }
}
