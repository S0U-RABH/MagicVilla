using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static MagicVilla_Utility.SD;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responceModel { get;  set; }
        public IHttpClientFactory httpClient { get; set; }
        
        public BaseService(IHttpClientFactory httpClient)
        {
            this.responceModel = new APIResponse();
            this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                if (apiRequest.ContentType == ContentType.MultipartFormData)
                {
                    message.Headers.Add("Accept", "*/*");
                }
                else
                {
                    message.Headers.Add("Accept", "application/json");
                }

                message.RequestUri = new Uri(apiRequest.Url);

                if(apiRequest.ContentType == ContentType.MultipartFormData)
                {
                    var content = new MultipartFormDataContent();

                    foreach(var prop in apiRequest.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(apiRequest.Data);
                        if(value is FormFile)
                        {
                            var file = (FormFile)value;
                            if(file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()),prop.Name, file.FileName);
                            }
                        }
                        else
                        {
                            content.Add(new StringContent(value == null ? "" : value.ToString()),prop.Name);
                        }
                    }

                    message.Content = content;
                }
                else
                {
                    if (apiRequest.Data != null)
                    {
                        message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                            Encoding.UTF8, "Application/json");
                    }
                }


                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage apiResponce = null;

                if(!string.IsNullOrEmpty(apiRequest.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",apiRequest.Token);
                }

                apiResponce = await client.SendAsync(message);

                var apiContent = await apiResponce.Content.ReadAsStringAsync();
                try
                {
                    APIResponse APIResponce = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if(apiResponce.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                        apiResponce.StatusCode == System.Net.HttpStatusCode.NotFound)   
                    {
                        APIResponce.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        APIResponce.IsSuccess = false;
                        var res = JsonConvert.SerializeObject(APIResponce);
                        var returnObj = JsonConvert.DeserializeObject<T>(res);
                        return returnObj;
                    }
                }
                catch (Exception ex)
                {
                    var ExceptionResponce = JsonConvert.DeserializeObject<T>(apiContent);
                    return ExceptionResponce;
                }
                var APIResponc = JsonConvert.DeserializeObject<T>(apiContent);
                return APIResponc;
            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponce = JsonConvert.DeserializeObject<T>(res);
                return APIResponce;
            }
            
        }
    }
}
