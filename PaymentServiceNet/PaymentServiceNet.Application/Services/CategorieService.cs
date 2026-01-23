using PaymentServiceNet.Application.Dtos.Response;
using PaymentServiceNet.Application.Interfaces;
using PaymentServiceNet.Core.Entities;
using PaymentServiceNet.Core.IRepositorio;
using PaymentServiceNet.Infrastructure.Repositorio.WorkContainer;
using AutoMapper;

namespace PaymentServiceNet.Application.Services
{
    public class CategorieService : ICategoryService
    {

        private readonly IUnitOfWork contenedorTrabajo;
        private readonly IMapper _mapper;

        public CategorieService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.contenedorTrabajo = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ApiResponse> CreateCategoryAsync(Category category)
        {
            this.contenedorTrabajo.Categories.Add(category);
            await this.contenedorTrabajo.SaveChangesAsync();
            return new ApiResponse(200, "Category created");
        }

        public async Task DeleteCategoryAsync(int id)
        {
            this.contenedorTrabajo.Categories.Remove(id);
            await this.contenedorTrabajo.SaveChangesAsync();
        }

        public IEnumerable<Category> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllCategories()
        {
           return this.contenedorTrabajo.Categories.GetAll();
        }
         
        public Category GetCategoria(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCategoryAsync(Category cat)
        {
            this.contenedorTrabajo.Categories.Update(cat);
            await this.contenedorTrabajo.SaveChangesAsync();
        }
    }
}
