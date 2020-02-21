using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_bot_bugReport
{
    [Serializable]
   public class InvalidUserAnswerException: Exception
    {
        public InvalidUserAnswerException(string message): base(message)
        {

        }

    }
}
