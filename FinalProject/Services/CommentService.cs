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

        public void addUserToBlockList(string commentId, string email)
        {

            var commentIn = _comments.Find(p => p.Id == commentId).SingleOrDefault();

            var userIn = _users.Find(u => u.email == email).SingleOrDefault();

            if (commentIn.block != null)
                commentIn.block.Add(new User { Id = userIn.Id, name = userIn.name, email = userIn.email });
            else
                commentIn.block = new List<User> { new User { Id = userIn.Id, name = userIn.name, email = userIn.email } };

            _comments.ReplaceOne(p => p.Id == commentId, commentIn);

        }

        public int checkBlockList(string id)
        {
            Comment comment = getCommentById(id);

            int counter = 0;

            HashSet<string> knownValues = new HashSet<string>();


            foreach (User user in comment.block)
            {
                if (knownValues.Add(user.email))
                    counter++;

            }

            return counter;
        }



    }
}
