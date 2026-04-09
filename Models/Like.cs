using DAL;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wikimedia.Models
{
    public class Like : Record
    {
        public int MediaId { get; set; }
        public int UserId { get; set; }
    }
}