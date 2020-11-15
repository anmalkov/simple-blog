﻿using SimpleBlog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleBlog.Services
{
    public interface IArticlesService
    {
        Task<List<Article>> GetAllAsync();
        Task<List<Article>> GetAllAsync(string tag);
        Task<Article> GetAsync(string id);
        Task CreateAsync(Article article);
        Task UpdateAsync(Article article);
        Task DeteleAsync(string id);
        Task ReleaseAsync(string id);
    }
}