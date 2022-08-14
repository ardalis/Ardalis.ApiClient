using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace Ardalis.ApiClient
{
  public class HttpService
  {
    public HttpClient HttpClient { get; private set; }
    private string ApiBaseUrl => HttpClient.BaseAddress == null ? string.Empty: HttpClient.BaseAddress.ToString();

    public HttpRequestHeaders RequestHeaders => HttpClient.DefaultRequestHeaders;

    public HttpService(HttpClient httpClient)
    {
      HttpClient = httpClient;
    }

    public void ResetHttp(string baseUri=null, string accept=null)
    {
      Uri baseUriToAdd = null;

      if (baseUri == null)
      {
        baseUriToAdd = HttpClient.BaseAddress;
      }
      else
      {
        baseUriToAdd = new Uri(baseUri);
      }
      
      var acceptToAdd = accept;
      if (string.IsNullOrEmpty(accept))
      {
        acceptToAdd = HttpClient.DefaultRequestHeaders.Accept.First()?.ToString();
      }

      SetBaseUri(acceptToAdd, baseUriToAdd);
    }

    public void ResetBaseUri()
    {
      var acceptToAdd = HttpClient.DefaultRequestHeaders.Accept.First()?.ToString();

      SetBaseUri(acceptToAdd);
    }

    public void SetBaseUri(string accept, Uri baseUriToAdd=null)
    {
      var token = GetFirstHeader("Authorization");
      var timeout = HttpClient.Timeout;

      HttpClient = new HttpClient();
      if (baseUriToAdd != null)
      {
        HttpClient.BaseAddress = baseUriToAdd;
      }      
      HttpClient.DefaultRequestHeaders.Add("accept", accept);
      HttpClient.DefaultRequestHeaders.Add("Authorization", token);
      HttpClient.Timeout = timeout;
    }

    public void AddHeader(string key, string value)
    {
      if (HttpClient.DefaultRequestHeaders.Contains(key))
      {
        HttpClient.DefaultRequestHeaders.Remove(key);
      }
      HttpClient.DefaultRequestHeaders.Add(key, value);
    }

    public string GetFirstHeader(string key)
    {
      var allValues = HttpClient.DefaultRequestHeaders.GetValues(key);

      return allValues.FirstOrDefault();
    }

    public string[] GetHeader(string key)
    {
      var allValues = HttpClient.DefaultRequestHeaders.GetValues(key);

      return allValues.ToArray();
    }

    public void SetTimeout(int units, TimeoutType timeType = TimeoutType.Seconds)
    {
      if(timeType == TimeoutType.Seconds)
      {
        HttpClient.Timeout = TimeSpan.FromSeconds(units);
      } 
      else if(timeType == TimeoutType.Minutes)
      {
        HttpClient.Timeout = TimeSpan.FromMinutes(units);
      }
      else if (timeType == TimeoutType.Hours)
      {
        HttpClient.Timeout = TimeSpan.FromHours(units);
      }   
    }

    public void SetAuthorization(string value)
    {
      HttpClient.DefaultRequestHeaders.Remove("Authorization");
      HttpClient.DefaultRequestHeaders.Add("Authorization", $"bearer {value}");
    }

    public void SetDefaultTimeout()
    {
      HttpClient.Timeout = TimeSpan.FromSeconds(60);
    }

    public async Task<HttpResponse<T>> HttpGetAsync<T>(string uri)
        where T : class
    {
      var uriToSend = $"{ApiBaseUrl}{uri}";

      var result = await HttpClient.GetAsync(uriToSend);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<T>> HttpGetAsync<T>(string uri, Dictionary<string, string> query)
        where T : class
    {
      var uriToSend = $"{ApiBaseUrl}{QueryHelpers.AddQueryString(uri, query)}";      
      
      var result = await HttpClient.GetAsync(uriToSend);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response =  HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public Task<HttpResponse<T>> HttpDeleteAsync<T>(string uri, object id)
        where T : class
    {
      return HttpDeleteAsync<T>($"{uri}/{id}");
    }

    public async Task<HttpResponse<T>> HttpDeleteAsync<T>(string uri)
        where T : class
    {
      var result = await HttpClient.DeleteAsync($"{ApiBaseUrl}{uri}");
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<bool>> HttpDeleteAsync(string uri)
    {
      var result = await HttpClient.DeleteAsync($"{ApiBaseUrl}{uri}");
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<bool>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode, result.Headers);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<T>> HttpPostAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await HttpClient.PostAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<T>> HttpPostByQueryAsync<T>(string uri, Dictionary<string, string> query)
      where T : class
    {
      var uriToSend = QueryHelpers.AddQueryString(uri, query);

      var result = await HttpClient.PostAsync($"{ApiBaseUrl}{uriToSend}", null);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<T>> HttpPostByFormAsync<T>(string uri, NameValueCollection query)
      where T : class
    {
      var formContent = new FormUrlEncodedContent(ToListKeyValuePair(query).ToArray());
      var result = await HttpClient.PostAsync($"{ApiBaseUrl}{uri}", formContent);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<T>> HttpPostByStringAsync<T>(string uri, string body)
      where T : class
    {

      var result = await HttpClient.PostAsync($"{ApiBaseUrl}{uri}", new StringContent(body));
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<T>> HttpPutJsonAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await HttpClient.PutAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }
    
    public async Task<HttpResponse<T>> HttpPatchAsync<T>(string uri, object dataToSend)
        where T : class
    {
      var content = ToJson(dataToSend);

      var result = await HttpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }
    
    public async Task<HttpResponse<bool>> HttpPatchWithoutResponseAsync(string uri, object dataToSend)
    {
      var content = ToJson(dataToSend);

      var result = await HttpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<bool>.FromHttpResponseMessage(false, result.StatusCode);
      }

      var response = HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<T>> HttpPatchBytesAsync<T>(string uri, byte[] dataToSend)
      where T : class
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await HttpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<bool>> HttpPatchBytesAsync(string uri, byte[] dataToSend)
    {
      ByteArrayContent content = null;
      if (dataToSend != null)
      {
        content = new ByteArrayContent(dataToSend);
      }

      var result = await HttpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<bool>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<HttpResponse<bool>> HttpPatchBytesAsync(string uri, ByteArrayContent content)
    {
      var result = await HttpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<bool>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<bool> HttpPatchBytesWithoutResponseAsync(string uri, byte[] dataToSend)
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await HttpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return false;
      }

      return true;
    }

    public async Task<bool> HttpPatchBytesWithoutResponseAsync(string uri, ByteArrayContent content)
    {
      var result = await HttpClient.PatchAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return false;
      }

      return true;
    }

    public async Task<HttpResponse<T>> HttpPutBytesAsync<T>(string uri, byte[] dataToSend)
      where T : class
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await HttpClient.PutAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<T>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<T>.FromHttpResponseMessage(result);
      response.SetResponseHeaders(response.Headers);

      return response;

    }

    public async Task<HttpResponse<bool>> HttpPutBytesAsync(string uri, byte[] dataToSend)
    {
      ByteArrayContent content = null;
      if (dataToSend != null)
      {
        content = new ByteArrayContent(dataToSend);
      }

      var result = await HttpClient.PutAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return HttpResponse<bool>.FromHttpResponseMessage(result.StatusCode);
      }

      var response = HttpResponse<bool>.FromHttpResponseMessage(true, result.StatusCode);
      response.SetResponseHeaders(response.Headers);

      return response;
    }

    public async Task<bool> HttpPutBytesWithoutResponseAsync(string uri, byte[] dataToSend)
    {
      var content = new ByteArrayContent(dataToSend);

      var result = await HttpClient.PutAsync($"{ApiBaseUrl}{uri}", content);
      if (!result.IsSuccessStatusCode)
      {
        return false;
      }

      return true;
    }

    private StringContent ToJson(object obj)
    {
      return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
    }

    public List<KeyValuePair<string, string>> ToListKeyValuePair(NameValueCollection query) 
    {
      var result = new List<KeyValuePair<string, string>>();
      foreach (var item in query.AllKeys.SelectMany(query.GetValues, (k, v) => new { key = k, value = v }))
      {
        result.Add(new KeyValuePair<string, string>(item.key, item.value));
      }

      return result;
    }
  }
}
