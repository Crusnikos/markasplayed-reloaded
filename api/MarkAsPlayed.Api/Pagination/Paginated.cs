﻿namespace MarkAsPlayed.Api.Pagination;

public class Paginated<TData>
{
    public Paginated(IReadOnlyList<TData> data, int page, int total)
    {
        Data = data;
        Page = page;
        Total = total;
    }

    public IReadOnlyList<TData> Data { get; }

    public int Page { get; }

    public int Total { get; }
}
