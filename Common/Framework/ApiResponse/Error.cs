﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.ApiResponse
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public Error()
        {

        }
        public Error(string message, string code = "General")
        {
            Message = message;
            Code = code;
        }


    }
}
