using FinalProject.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.HelpClasses
{
    public class SRTAndPost
    {
        public IPagedList<Srt> srtList { get; set; }

        public IPagedList<Post> postList { get; set; }
    }
}
