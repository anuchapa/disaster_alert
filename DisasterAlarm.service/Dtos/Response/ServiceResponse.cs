using System;

namespace DisasterAlarm.service.Dtos.Response;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static ServiceResponse<T> Ok(T data, string message = "Success")
        => new() { Data = data, Success = true, Message = message };

    public static ServiceResponse<T> Fail(string message)
        => new() { Success = false, Message = message };
}
