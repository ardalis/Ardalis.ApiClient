namespace Ardalis.ApiClient
{
  public interface IBaseApiCaller<T>
  {
    public HttpResponse<T> Execute();
  }
}
