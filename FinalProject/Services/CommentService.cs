using FinalProject.HelpClasses;
using FinalProject.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
    public class CommentService
    {
        private readonly IMongoCollection<Comment> _comments;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Post> _posts;

        public CommentService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _comments = database.GetCollection<Comment>("comments");
            _users = database.GetCollection<User>("users");
            _posts = database.GetCollection<Post>("posts");
        }

        public Comment createComment(Comment comment, string email)
        {

            var userIn = _users.Find(u => u.email == email).SingleOrDefault();

            var postIn = _posts.Find(p => p.Id == comment.postId.Id).SingleOrDefault();

            comment.postId = new Post { Id = postIn.Id, text = postIn.text, name = postIn.name, block = postIn.block };

            comment.name = userIn.name;

            _comments.InsertOne(comment);

            return comment;

        }

        public Comment getCommentById(string id)
        {
            return _comments.Find(c => c.Id == id).SingleOrDefault();
        }

        public void removeComment(string id)
        {
            _comments.DeleteOne(c => c.Id == id);
        }

        public void updateComment(string id, Comment comment)
        {
            _comments.ReplaceOne(c => c.Id == id, comment);
        }


    }
}
