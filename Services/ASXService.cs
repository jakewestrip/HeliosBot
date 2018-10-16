using HeliosBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HeliosBot.Services
{
    public interface IASXService
    {
        Task<Result<ASXLookupResult>> Lookup(string ticker);
        Task<Result<ASXCompanyResult>> Company(string ticker);
        Task<Result<List<ASXHighchartResult>>> Highchart(string ticker);
        Task<Result<ASXAnnualReportResult>> AnnualReport(string ticker);
        Task<Result<ASXAnnouncementsResult>> Announcements(string ticker);
        Task<Result<ASXSimilarResult>> Similar(string ticker);
        Task<Result<ASXUpcomingEventsResult>> UpcomingEvents(string ticker);
    }

    public class ASXService : IASXService
    {
        private readonly HttpClient _httpClient;

        public ASXService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://www.asx.com.au/asx/1/");
        }

        public async Task<Result<ASXLookupResult>> Lookup(string ticker)
        {
            var result = await _httpClient.GetAsync("share/" + ticker.ToUpper());
            if (!result.IsSuccessStatusCode)
                return new Result<ASXLookupResult> { IsSuccess = false, Message = result.ReasonPhrase };

            try
            {
                var json = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<ASXLookupResult>(json);
                obj.StockCode = ticker.ToUpper();
                obj.Endpoint = _httpClient.BaseAddress + "share/" + ticker.ToUpper();
                return new Result<ASXLookupResult> {IsSuccess = true, Payload = obj};
            }
            catch (Exception e)
            {
                return new Result<ASXLookupResult> {IsSuccess = false, Message = e.Message};
            }
        }

        public async Task<Result<ASXCompanyResult>> Company(string ticker)
        {
            var result = await _httpClient.GetAsync("company/" + ticker.ToUpper());
            if (!result.IsSuccessStatusCode)
                return new Result<ASXCompanyResult> { IsSuccess = false, Message = result.ReasonPhrase };

            try
            {
                var json = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<ASXCompanyResult>(json);
                obj.StockCode = ticker.ToUpper();
                obj.Endpoint = _httpClient.BaseAddress + "company/" + ticker.ToUpper();
                return new Result<ASXCompanyResult> { IsSuccess = true, Payload = obj };
            }
            catch (Exception e)
            {
                return new Result<ASXCompanyResult> { IsSuccess = false, Message = e.Message };
            }
        }

        public async Task<Result<ASXAnnualReportResult>> AnnualReport(string ticker)
        {
            var result = await _httpClient.GetAsync("company/" + ticker.ToUpper() + "/?fields=latest_annual_reports");
            if (!result.IsSuccessStatusCode)
                return new Result<ASXAnnualReportResult> { IsSuccess = false, Message = result.ReasonPhrase };

            try
            {
                var json = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<ASXAnnualReportResult>(json);
                obj.StockCode = ticker.ToUpper();
                obj.Endpoint = _httpClient.BaseAddress + "company/" + ticker.ToUpper() + "/?fields=latest_annual_reports";
                return new Result<ASXAnnualReportResult> { IsSuccess = true, Payload = obj };
            }
            catch (Exception e)
            {
                return new Result<ASXAnnualReportResult> { IsSuccess = false, Message = e.Message };
            }
        }

        public async Task<Result<ASXAnnouncementsResult>> Announcements(string ticker)
        {
            var result = await _httpClient.GetAsync("company/" + ticker.ToUpper() + "/announcements?count=8");
            if (!result.IsSuccessStatusCode)
                return new Result<ASXAnnouncementsResult> { IsSuccess = false, Message = result.ReasonPhrase };

            try
            {
                var json = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<ASXAnnouncementsResult>(json);
                obj.StockCode = ticker.ToUpper();
                obj.Endpoint = _httpClient.BaseAddress + "company/" + ticker.ToUpper() + "/announcements?count=8";
                return new Result<ASXAnnouncementsResult> { IsSuccess = true, Payload = obj };
            }
            catch (Exception e)
            {
                return new Result<ASXAnnouncementsResult> { IsSuccess = false, Message = e.Message };
            }
        }

        public async Task<Result<ASXSimilarResult>> Similar(string ticker)
        {
            var result = await _httpClient.GetAsync("company/" + ticker.ToUpper() + "/similar?count=20");
            if (!result.IsSuccessStatusCode)
                return new Result<ASXSimilarResult> { IsSuccess = false, Message = result.ReasonPhrase };

            try
            {
                var json = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<ASXSimilarResult>(json);
                obj.StockCode = ticker.ToUpper();
                obj.Endpoint = _httpClient.BaseAddress + "company/" + ticker.ToUpper() + "/similar?count=20";
                return new Result<ASXSimilarResult> { IsSuccess = true, Payload = obj };
            }
            catch (Exception e)
            {
                return new Result<ASXSimilarResult> { IsSuccess = false, Message = e.Message };
            }
        }

        public async Task<Result<ASXUpcomingEventsResult>> UpcomingEvents(string ticker)
        {
            var result = await _httpClient.GetAsync("company/" + ticker.ToUpper() + "/events/upcoming?count=10");
            if (!result.IsSuccessStatusCode)
                return new Result<ASXUpcomingEventsResult> { IsSuccess = false, Message = result.ReasonPhrase };

            try
            {
                var json = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<ASXUpcomingEventsResult>(json);
                obj.StockCode = ticker.ToUpper();
                obj.Endpoint = _httpClient.BaseAddress + "company/" + ticker.ToUpper() + "/events/upcoming?count=10";
                return new Result<ASXUpcomingEventsResult> { IsSuccess = true, Payload = obj };
            }
            catch (Exception e)
            {
                return new Result<ASXUpcomingEventsResult> { IsSuccess = false, Message = e.Message };
            }
        }

        public async Task<Result<List<ASXHighchartResult>>> Highchart(string ticker)
        {
            var result = await _httpClient.GetAsync("chart/highcharts?asx_code=" + ticker + "&years=10");
            if (!result.IsSuccessStatusCode)
                return new Result<List<ASXHighchartResult>> { IsSuccess = false, Message = result.ReasonPhrase };

            try
            {
                var json = await result.Content.ReadAsStringAsync();
                var objs = JsonConvert.DeserializeObject<List<List<string>>>(json);
                var list = objs.Select(x => new ASXHighchartResult
                {
                    Date = DateTime.UnixEpoch + TimeSpan.FromMilliseconds(long.Parse(x[0])),
                    Price = double.Parse(x[1]),
                    Volume = x.Count == 6 ? long.Parse(x[5]) : long.Parse(x[2]),
                    Endpoint = "",
                    StockCode = ""
                }).ToList();
                return new Result<List<ASXHighchartResult>> { IsSuccess = true, Payload = list };
            }
            catch (Exception e)
            {
                return new Result<List<ASXHighchartResult>> { IsSuccess = false, Message = e.Message };
            }
        }
    }
}
