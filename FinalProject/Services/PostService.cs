using FinalProject.HelpClasses;
using FinalProject.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
    public class PostService
    {
        private readonly IMongoCollection<Post> _posts;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Comment> _comments;

        public PostService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _posts = database.GetCollection<Post>("posts");
            _users = database.GetCollection<User>("users");
            _comments = database.GetCollection<Comment>("comments");
        }

        public Post createPost(Post post, string email)
        {

            var userIn = _users.Find(u => u.email == email).SingleOrDefault();

            post.name = userIn.name;

            _posts.InsertOne(post);

            return post;

        }

        public List<Post> getPostList()
        {
            return _posts.Find(post => true).ToList();
        }

        public void addCommentToPost(string postId, string commentId, string name)
        {
            var postIn = _posts.Find(p => p.Id == postId).SingleOrDefault();

            var commentIn = _comments.Find(c => c.Id == commentId).SingleOrDefault();

            var userIn = _users.Find(u => u.name == name).SingleOrDefault();

            if (postIn.comList != null)
                postIn.comList.Add(new Comment { Id = commentIn.Id, text = commentIn.text, name = userIn.name, postId = new Post { Id = postIn.Id, text = postIn.text, name = postIn.name, block = postIn.block } });
            else
                postIn.comList = new List<Comment> { new Comment { Id = commentIn.Id, text = commentIn.text, name = userIn.name, postId = new Post { Id = postIn.Id, text = postIn.text, name = postIn.name, block = postIn.block } } };

            _posts.ReplaceOne(p => p.Id == postId, postIn);
        }


        public Post getPostById(string id)
        {
            return _posts.Find(post => post.Id == id).SingleOrDefault();
        }

        public void updatePost(string id, Post post)
        {
            _posts.ReplaceOne(p => p.Id == id, post);
        }

    }
}
