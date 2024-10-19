﻿using Microsoft.EntityFrameworkCore;

namespace Reddit.Repositories
{
    public class PagedList<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public PagedList(List<T> items, int page, int pageSize, int count, bool hasNextPage, bool hasPreviousPage)
        {
            Items = items;
            PageNumber = page;
            PageSize = pageSize;
            TotalCount = count;
            HasNextPage = hasNextPage;
            HasPreviousPage = hasPreviousPage;
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> items, int page, int pageSize)
        {
            var pageItems = await items.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync<T>();
            var totalCount = await items.CountAsync();
            var hasNextPage = (page * pageSize) < totalCount;
            var hasPreviousPage = page > 1;

            return new PagedList<T>(pageItems, page, pageSize, totalCount, hasNextPage, hasPreviousPage);
        }
    }
}
