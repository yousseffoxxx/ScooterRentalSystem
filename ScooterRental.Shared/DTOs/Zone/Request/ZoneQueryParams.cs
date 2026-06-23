namespace ScooterRental.Shared.DTOs.Zone.Request
{
    public record ZoneQueryParams
    {
        private const int defaultPageSize = 5;
        private const int maxPageSize = 10;
        private int pageIndex = 1;
        private int pageSize = defaultPageSize;

        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value < 1 ? 1 : value; }
        }
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > maxPageSize ? maxPageSize : value; }
        }
        public bool? IsActive { get; set; }
    }
}
