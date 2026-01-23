using PaymentServiceNet.Application.Dtos.Response;
using PaymentServiceNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse> CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Category GetCategoria(int id);
        IEnumerable<object> GetAllCategories();
        IEnumerable<Category> GetAll();
    }
}
