using System;
using Microsoft.SPOT;

namespace savePhotoETH {
    class PortException: Exception{
        public PortException(){
        }

        public PortException(string message): base(message){
        }

        public PortException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
