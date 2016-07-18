using System;
using System.Text;

namespace savePhotoETH
{
    class LogoutException : Exception
    {
        public LogoutException(){
        }

        public LogoutException(string message): base(message){
        }

        public LogoutException(string message, Exception inner): base(message, inner){
        }
    }
}
