namespace Campus.Common;

public class ApiResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = "操作成功";
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static ApiResponse Success(string message = "操作成功")
    {
        return new ApiResponse { Code = 0, Message = message };
    }

    public static ApiResponse Error(int code, string message)
    {
        return new ApiResponse { Code = code, Message = message };
    }

    public static ApiResponse BadRequest(string message = "请求参数错误")
    {
        return new ApiResponse { Code = 400, Message = message };
    }

    public static ApiResponse Unauthorized(string message = "未授权")
    {
        return new ApiResponse { Code = 401, Message = message };
    }

    public static ApiResponse Forbidden(string message = "无权限")
    {
        return new ApiResponse { Code = 403, Message = message };
    }

    public static ApiResponse NotFound(string message = "资源不存在")
    {
        return new ApiResponse { Code = 404, Message = message };
    }

    public static ApiResponse ServerError(string message = "服务器内部错误")
    {
        return new ApiResponse { Code = 500, Message = message };
    }
}

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = "操作成功";
    public T? Data { get; set; }
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static ApiResponse<T> Success(T data, string message = "操作成功")
    {
        return new ApiResponse<T> { Code = 0, Message = message, Data = data };
    }

    public static ApiResponse<T> Error(int code, string message)
    {
        return new ApiResponse<T> { Code = code, Message = message };
    }
}

public class PageResult<T>
{
    public List<T> List { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;

    public static PageResult<T> Of(List<T> list, int total, int page, int pageSize)
    {
        return new PageResult<T>
        {
            List = list,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
