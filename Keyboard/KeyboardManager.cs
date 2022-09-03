using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Keyboard
{
    public class KeyboardManager
    {

        HttpClient _client;
        ILogger _logger;
        Appconfig _config;
        public KeyboardManager(HttpClient client, ILogger logger, Appconfig config)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config;
        }

        public async Task<string> BindEvent(BindEventModel evenModel)
        {
            try
            {
                var response = await _client.PostAsJsonAsync<BindEventModel>(_config.BindEvent, evenModel);
                //using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress.AbsoluteUri + _config.BindEvent))
                //{
                //    HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(evenModel), Encoding.UTF8, "application/json");

                //    httpRequestMessage.Content = httpContent;
                //  //  httpRequestMessage.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue() { NoCache = true };
                // //   httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                //  //  httpRequestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                //  //  httpRequestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                //  //  httpRequestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
                //   // var response = await _client.SendAsync(httpRequestMessage);
                //}
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Добавлено событие game: {evenModel.game} event: {evenModel._event}");
                    return "Событие успешно добавлено.";
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Не удалось добавить событие game: {evenModel.game} event: {evenModel._event}");
                    _logger.LogError($"{result}");
                    return result;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _logger.LogError($"{ex.ToString()}");
            }
            return "Прозошла ошибка";

        }

        public async Task<string> RemoveEvent(RemoveModel model)
        {
            try
            {

                var response = await _client.PostAsJsonAsync<RemoveModel>(_config.RemoveEvent, model);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return "Событие удалено успешно";
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Не удалось добавить событие game: {model.game} evet: {model._event}");
                    _logger.LogError($"{result}");
                    return result;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _logger.LogError($"{ex.ToString()}");
            }
            return "Прозошла ошибка";
        }

       

        public async Task FireLanguageEvent(int value)
        {
            FireEventModel model = new FireEventModel();
            model.game = "LANGUAGE";
            model._event = "CHANGE_";
            model.data = new Data() { value = value };

            for (int i = 1; i < 13; i++)
            {
                model._event = "CHANGE_" + i.ToString();
                await FireEvent(model);
            }
           
        }

        public async Task FireEvent(FireEventModel model)
        {
            var response = await _client.PostAsJsonAsync<FireEventModel>(_config.FireEvent, model);
        }

        public async Task BeatEvent()
        {
            BeatModel model = new BeatModel() { game = "LANGUAGE" };
            var response = await _client.PostAsJsonAsync<BeatModel>(_config.ContinueEvents, model);
        }


    }
}
