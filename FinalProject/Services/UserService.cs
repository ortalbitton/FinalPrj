using FinalProject.HelpClasses;
using FinalProject.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FinalProject.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>("users");

        }

        public User createUser(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public List<User> getUser(User user)
        {
            return _users.Find(u => u.email == user.email && u.password == user.password).ToList();
        }

        public User getUserByKey(string email)
        {
            return _users.Find(user => user.email == email).SingleOrDefault();
        }


    }
}
