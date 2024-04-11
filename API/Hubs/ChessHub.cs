using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{

    public class ChessHub : Hub
    {

        public ChessHub()
        {

        }

    }
}