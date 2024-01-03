﻿namespace BookStore.Models
{
    public class ApiSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string BooksCollectionName { get; set; } = string.Empty;
    }
}
