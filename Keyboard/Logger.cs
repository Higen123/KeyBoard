using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyboard
{
    public class TxTLogger : ILogger
    {
        private string _filePath;
        public static readonly Lazy<object> _locker = new Lazy<object>(() => new object(), true);
        public TxTLogger(string path)
        {
            _filePath = path;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            try
            {
                if (formatter != null)
                {
                    lock (_locker.Value)
                    {
                        using (var fw = new StreamWriter(_filePath, true))
                        {
                            string text = formatter(state, exception);
                            StringBuilder strBuilder = new StringBuilder();
                            strBuilder.Append(logLevel.ToString());
                            strBuilder.Append("-");
                            strBuilder.Append(text);
                            strBuilder.Append("-");
                            strBuilder.Append(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                            strBuilder.Append(Environment.NewLine);
                            fw.WriteLine(strBuilder.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} {1}", "Ошибка в логгере", ex.ToString());
            }

        }
    }
}
