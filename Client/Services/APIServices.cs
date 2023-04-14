using Microsoft.AspNetCore.Components.Authorization;
using WebAppAcademics.Client.Authentication;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace WebAppAcademics.Client.Services
{
    public class APIServices<T> : IAPIServices<T>
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
       
        public APIServices(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider; 
        }

        public async Task<List<T>> GetAllAsync(string requestUri)
        {
            List<T> result = new();
            
            if (APICallParameters.IsAuth)
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/" + requestUri);
                var response = await _httpClient.SendAsync(requestMessage);
                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    result = await Task.FromResult(JsonConvert.DeserializeObject<List<T>>(responseBody));
                }
                else
                {
                    result = default;
                }
            }
            else
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/" + requestUri);
                var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;                
                var token = await customAuthStateProvider.GetToken();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                    var response = await _httpClient.SendAsync(requestMessage);
                    var responseStatusCode = response.StatusCode;
                    if (responseStatusCode.ToString() == "OK")
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        result = await Task.FromResult(JsonConvert.DeserializeObject<List<T>>(responseBody));
                    }
                    else
                    {
                        result = default;
                    }                        
                }
                else
                {
                    result = default;
                }                
            }   
            
            return result;
        }

        public async Task<T> GetByIdAsync(string requestUri, int Id)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/" + requestUri + Id);
            var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = await customAuthStateProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                var response = await _httpClient.SendAsync(requestMessage);
                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return await Task.FromResult(JsonConvert.DeserializeObject<T>(responseBody));
                }
                else
                    return default;
            }
            else
                return default;           
        }

        public async Task<T> SaveAsync(string requestUri, T obj)
        {
            string serializedUser = JsonConvert.SerializeObject(obj);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/" + requestUri);
            var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = await customAuthStateProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                requestMessage.Content = new StringContent(serializedUser);
                requestMessage.Content.Headers.ContentType
                    = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(requestMessage);

                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var returnedObj = JsonConvert.DeserializeObject<T>(responseBody);
                    return await Task.FromResult(returnedObj);
                }
                else
                    return default;
            }
            else
                return default;
        }

        public async Task<T> UpdateAsync(string requestUri, int Id, T obj)
        {
            string serializedUser = JsonConvert.SerializeObject(obj);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/" + requestUri + Id);
            var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = await customAuthStateProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                requestMessage.Content = new StringContent(serializedUser);
                requestMessage.Content.Headers.ContentType
                    = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(requestMessage);
                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var returnedObj = JsonConvert.DeserializeObject<T>(responseBody);
                    return await Task.FromResult(returnedObj);
                }
                else
                    return default;
            }
            else
                return default;
        }

        public async Task<bool> DeleteAsync(string requestUri, int Id)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "api/" + requestUri + Id);
            var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = await customAuthStateProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                var response = await _httpClient.SendAsync(requestMessage);
                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    return await Task.FromResult(true);
                }
                else
                    return default;
            }
            else
                return default;
        }

        public async Task<int> CountAsync(string requestUri, int Id)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/" + requestUri + Id);
            var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = await customAuthStateProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                var response = await _httpClient.SendAsync(requestMessage);
                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return await Task.FromResult(JsonConvert.DeserializeObject<int>(responseBody));
                }
                else
                    return default;
            }
            else
                return default;
        }

        public async Task<int> LicenseCountAsync(string requestUri, int Id)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/" + requestUri + Id);
            var response = await _httpClient.SendAsync(requestMessage);
            var responseStatusCode = response.StatusCode;
            if (responseStatusCode.ToString() == "OK")
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return await Task.FromResult(JsonConvert.DeserializeObject<int>(responseBody));
            }
            else
                return default;
        }

        public async Task<string> GetStringAsync(string requestUri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/" + requestUri);
            var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = await customAuthStateProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                var response = await _httpClient.SendAsync(requestMessage);
                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return await Task.FromResult(responseBody);
                }
                else
                    return default;
            }
            else
                return default;
        }

        public async Task<byte[]> GetResults(string requestUri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/" + requestUri);
            var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = await customAuthStateProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                var response = await _httpClient.SendAsync(requestMessage);
                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    var responseBody = await response.Content.ReadAsByteArrayAsync();
                    return await Task.FromResult(responseBody);
                }
                else
                    return default;
            }
            else
                return default;
        }

        public async Task ExportResultsAsync(string requestUri, T obj)
        {
            string serializedUser = JsonConvert.SerializeObject(obj);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/" + requestUri);
            var customAuthStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = await customAuthStateProvider.GetToken();
            if (!string.IsNullOrWhiteSpace(token))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                requestMessage.Content = new StringContent(serializedUser);
                requestMessage.Content.Headers.ContentType
                    = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await _httpClient.SendAsync(requestMessage);
                var responseStatusCode = response.StatusCode;
                if (responseStatusCode.ToString() == "OK")
                {
                    var responseBody = await response.Content.ReadAsByteArrayAsync();
                    await Task.FromResult(responseBody);
                }
            }
             else
            {
                return;
            }
        }
    }
}
