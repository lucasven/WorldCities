using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Dynamic.Core;

namespace WorldCities.Server.Data
{
    public class ApiResult<T>
    {
        private ApiResult(
            List<T> data, 
            int count, 
            int pageIndex, 
            int pageSize,
            string? sortColumn,
            string? sortOrder,
            string? filterColumn,
            string? filterQuery)
        {
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            FilterColumn = filterColumn;
            FilterQuery = filterQuery;
        }
        public static async Task<ApiResult<T>> CreateAsync(
            IQueryable<T> source, 
            int pageIndex, 
            int pageSize,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            if(!string.IsNullOrEmpty(filterColumn)
                && !string.IsNullOrEmpty(filterQuery)
                && IsValidProperty(filterColumn))
            {
                source = source.Where(
                    string.Format("{0}.StartsWith(@0)",
                        filterColumn),
                        filterQuery);
            }

            var count = await source.CountAsync();

            if(!string.IsNullOrEmpty(sortColumn)
                && IsValidProperty(sortColumn))
            {
                sortOrder = !string.IsNullOrEmpty(sortOrder)
                    && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
                source = source.OrderBy(
                    string.Format("{0} {1}", sortColumn, sortOrder));
            }

            var data = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
            return new ApiResult<T>(
                data, 
                count, 
                pageIndex, 
                pageSize, 
                sortColumn, 
                sortOrder,
                filterColumn,
                filterQuery);
        }

        public static bool IsValidProperty(
            string propertyName, 
            bool throwExceptionIfNotFound = true)
        {
            var prop = typeof(T).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null && throwExceptionIfNotFound)
                throw new NotSupportedException(
                    $"ERROR: Property `{propertyName}` does not exist.");
            return prop != null;
        }

        public List<T> Data { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public string? SortColumn { get; private set; }
        public string? SortOrder { get; private set; }
        public string? FilterColumn { get; private set; }
        public string? FilterQuery { get; private set; }

        public bool HasPreviousPage => PageIndex > 0;
        public bool HasNextPage => ((PageIndex+1) < TotalPages);
    }
}

