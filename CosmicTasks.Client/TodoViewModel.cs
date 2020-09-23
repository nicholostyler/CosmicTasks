using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CosmicTasks.Client.Models;

namespace CosmicTasks.Client
{
    public class TodoViewModel
    {
        private readonly HttpClient _client;
        private const string _serviceEndpoint = "https://localhost:10001/api/TodoItems";


        public List<TodoItem> Items {get;set;}
        public string newTodoName { get; set;}

        public TodoViewModel(HttpClient client)
        {
            Items = new List<TodoItem>();
            _client = client;
        }

        public async Task GetTodoItems()
        {
            Items = await _client.GetFromJsonAsync<List<TodoItem>>(_serviceEndpoint);
        }

        public async Task AddItem()
        {
            if (string.IsNullOrWhiteSpace(newTodoName)) return;
            
            var addItem = new TodoItem { Name = newTodoName, IsComplete = false };
            await _client.PostAsJsonAsync(_serviceEndpoint, addItem);
            newTodoName = string.Empty;
            await GetTodoItems();
        }

        public async Task DeleteItem(long id)
        {
            await _client.DeleteAsync($"{_serviceEndpoint}/{id}");

            // do not call GetItems simply delete
            var deletedTodo = 
                (from todo in Items
                where (todo.Id) == id
                select todo).FirstOrDefault();

            Items.Remove(deletedTodo);
        }
    }
}
