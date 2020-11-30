using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string email { get; set; }

        public string name { get; set; }

        public string password { get; set; }

        [Compare("password")]
        public string confirmPassword { get; set; }

        public ICollection<Srt> srtList { get; set; }
    }
}
