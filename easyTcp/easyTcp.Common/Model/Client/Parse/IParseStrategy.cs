﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easyTcp.Common.Model.Client.Parse
{
    public interface IParseStrategy
    {
        string CommandPrompt();

        Request Parse(string command);
    }
}
