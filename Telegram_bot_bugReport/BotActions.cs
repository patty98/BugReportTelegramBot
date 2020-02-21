using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Net;
using System.Net.Http;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;

namespace Telegram_bot_bugReport
{
    public class BotActions
    {

        public TelegramBotClient Bot;
        private IssueClass _issue = new IssueClass();
        private string _postUrl = "https://team-1580832016763.atlassian.net/rest/api/2/";
        private string _path="";
        private string _authData;
        private string _token;
        private Dictionary<string, object> _valuesResponse = new Dictionary<string, object>();
        public BotActions(string authData, string token)
        {
            _authData = authData;
            _token = token;

        }

        public void InitializationBot(WebProxy proxy)
        {
            Bot = new TelegramBotClient(_token, proxy);//"934475293:AAHgthyLni4IMpGHaSR6eU9jx5Ocj86e5z0"
            Bot.OnMessage += BotMessageReceived;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            _issue.fields.assignee = new Assignee();
            _issue.fields.project = new Project();
            _issue.fields.issuetype = new Issuetype();
        }

       private void Bot_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {

        }

        private async void BotMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null)
                return;
            string help = "Справка:" +
                       "/tg_name- имя автора в телеграм, имя нужно писать после пробела от ключевого слова\n /project-название отчета, нужно писать после проблема от ключевого слова\n/summary-ключевые слова отчета, нужно писать после проблема от ключевого слова\n/reporter-имя автора, нужно писать после проблема от ключевого слова\n/description-описание бага, нужно писать после проблема от ключевого слова,\n /get- получить баг ";
            if (message.Text == "/hello")
                await Bot.SendTextMessageAsync(e.Message.From.Id, help);


            if (e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
              await DownloadFile(e).ContinueWith(x =>
                {
                    switch (x.Status)
                    {
                        // Handle any exceptions to prevent UnobservedTaskException.             
                        case TaskStatus.RanToCompletion:
                            _path = x.Result;
                            break;
                        case TaskStatus.Faulted:
                            if (x.Exception != null)
                            {
                                Console.WriteLine(x.Exception);
                            }

                            else
                                Console.WriteLine("Operation failed!");
                            break;
                        default:
                            break;
                    }


                });
                
            }
            else
            {
              ProcessingUserMessage(e).ContinueWith(x =>
                {
                    Console.WriteLine(x.Exception.Message);

                }, TaskContinuationOptions.OnlyOnFaulted);
            }

        }
        private async Task<string> DownloadFile(Telegram.Bot.Args.MessageEventArgs e)
        {

            var fileInfo = await Bot.GetFileAsync(e.Message.Document.FileId);

            var fileName = fileInfo.FileId + "." + fileInfo.FilePath.Split('.').Last();
            _path = Environment.CurrentDirectory + e.Message.Document.FileName;
            try

            {
                FileStream fs = new FileStream(_path, FileMode.Create, FileAccess.Write);
                await Bot.DownloadFileAsync(fileInfo.FilePath, fs);
                fs.Dispose();
                //   await Bot.SendTextMessageAsync(e.Message.From.Id, "Ваш файл сохранен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }
            return _path;


        }
        private async Task ProcessingUserMessage(Telegram.Bot.Args.MessageEventArgs e)
        {
           
            string codeMessage = ParseUserCode(e.Message);
            string userMessage = ParseUserMessage(e.Message);
            if(userMessage==e.Message.Text)
            {
                userMessage = " ";
            }
            codeMessage.ToLower();

            //  var file= await Bot.GetFileAsync(e.Message.Photo[e.Message.Photo.Length - 1].FileId);
            switch (codeMessage)
            {

                //var inlineKeyBoard = new InlineKeyboardMarkup(new[]
                //{
                //     new[]
                //     {
                //         InlineKeyboardButton.WithCallbackData("Telegram name"),
                //         InlineKeyboardButton.WithCallbackData("Name author's"),
                //         InlineKeyboardButton.WithCallbackData("Add file"),
                //         InlineKeyboardButton.WithCallbackData("Project"),
                //         InlineKeyboardButton.WithCallbackData("Summary"),
                //         InlineKeyboardButton.WithCallbackData("Reporter"),
                //         InlineKeyboardButton.WithCallbackData("Description")
                //     }
                //});
                //await Bot.SendTextMessageAsync(e.Message.From.Id, TgNameAnswer);
                //await Bot.SendTextMessageAsync(e.Message.From.Id, "Выберите пункт меню", replyMarkup: inlineKeyBoard);
                
                case "да":
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Пришлите мне документ");
                    break;
                case "/tg_name":
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Круто! " + userMessage + ", Едем дальше");
                    break;
                case "/project":
                    _issue.fields.project.key = userMessage;
                    //   List<JsonFields.project> projects = new List<JsonFields.project> {};
                    //   projects.Add(new JsonFields.project { key = userMessage });
                    //   fieldsJson.fields.Add(new JsonFields.main {project=projects});
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Круто! Едем дальше");
                    break;
                case "/summary":
                    _issue.fields.summary = userMessage;
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Круто! Едем дальше");
                    break;
                case "/reporter":
                    _issue.fields.assignee.name = userMessage;
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Круто! Едем дальше");
                    break;
                case "/description":
                    _issue.fields.description = userMessage;
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "Юхуууу! Ваш отчет почти готов");
                    break;
                case "/get":
                    string jsonContent = GetBugDescription();
                    string response = GetRequest(jsonContent);
                    _valuesResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    if(_valuesResponse.Keys.Any(u=> u=="error"))
                    {
                        await Bot.SendTextMessageAsync(e.Message.From.Id, _valuesResponse["error"] as string);
                    }
                    else
                    {
                       
                        await Bot.SendTextMessageAsync(e.Message.From.Id, "Чудненько! Ваш баг создан! Хотите прикрепить документ?");
                    }
                    break;
                case "/attachments":
                    if (_valuesResponse.Count != 0 && _path.Length!=0)
                    //путь к файлу для загрузки
                    {
                        JiraIssueAttachment jiraIssueAttachment = new JiraIssueAttachment(_postUrl, _path, _authData, _valuesResponse["key"] as string);
                        await Bot.SendTextMessageAsync(e.Message.From.Id, "Ваш документ прикреплен!");
                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(e.Message.From.Id, "Сначала выполните команду для создания бага :=) P.S. /get, так прикрепите, если нужно, файл для бага, затем повторите команду:)");
                    }
                    break;
                default:
                    throw new InvalidUserAnswerException("Неверный ответ от пользователя");
            }

        }

        

        private string ParseUserCode(Message message)
        {
            int indexWhitespace = message.Text.IndexOf(" ");
            string result_split;
            try
            {
                result_split = message.Text.Substring(0, indexWhitespace);
            }
            catch
            {
                return message.Text;
            }
            return result_split;
        }

        private string ParseUserMessage(Message message)
        {

            int indexWhitespace = message.Text.IndexOf(" ");
            string result_split;
            try
            {
                result_split = message.Text.Substring(indexWhitespace+1);
                
            }
            catch
            {
                return message.Text;
            }
            return result_split;


        }
        private string GetBugDescription()
        {
            string jsonStick = JsonConvert.SerializeObject(_issue, Formatting.Indented);
            Console.WriteLine(jsonStick);
            return jsonStick;

        }

        private string GetRequest(string jsonContent)
        {
            string result;

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(_postUrl);
            byte[] cred = UTF8Encoding.UTF8.GetBytes(_authData);//"linascabeskrovnaya@gmail.com:OTaP5ANjI3I287bGrgwK20D5"
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            // System.Net.Http.Formatting.MediaTypeFormatter jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("issue", content).Result;
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            else
            {
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }



        }
    }
}
