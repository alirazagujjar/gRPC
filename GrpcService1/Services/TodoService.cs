using Grpc.Core;
using GrpcService1.Data;
using GrpcService1.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService1.Services
{
    public class TodoService : ToDoIt.ToDoItBase
    {
        private readonly ILogger<TodoService> _logger;
        private readonly AppDbContext dBContext;

        public TodoService(ILogger<TodoService> logger,AppDbContext appDb)
        {
            _logger = logger;
            dBContext = appDb;
        }
        public override async Task<CreateTodoResponse> CreateToDo(CreateTodoRequest request, ServerCallContext context)
        {
            if(string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
                throw new RpcException(new Status(StatusCode.InvalidArgument,"You must supply valid object"));
            var todoItem = new TodoItems
            {
                Title = request.Title,
                Description = request.Description
            };
            await dBContext.AddAsync(todoItem);
            await dBContext.SaveChangesAsync();

            return await Task.FromResult(new CreateTodoResponse
            {
                Id = todoItem.Id
            });
        }
        public override async Task<ReadTodoResponse> ReadToDo(ReadTodoRequest request, ServerCallContext context)
        {
            if (request.Id<=0 )
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource must be greater than 0"));
            var result = await dBContext.TodoItems.FirstOrDefaultAsync(t=>t.Id==request.Id);
            if(result==null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No Item attached with this {request.Id}"));
            return await Task.FromResult(new ReadTodoResponse
            {
                Id = result.Id,
                Title= result.Title,
                Description= result.Description,
                TodoStatus = result.TodoStatus
            });
        }
        public override async Task<GetAllTodoResponse> ListToDo(GetAllTodoRequest request, ServerCallContext context)
        {
            var response = new GetAllTodoResponse();
            var result = await dBContext.TodoItems.ToListAsync();
            foreach(var res in result)
            {
                response.Todo.Add(new ReadTodoResponse
                {
                    Id = res.Id,
                    Title = res.Title,
                    Description = res.Description,
                    TodoStatus = res.TodoStatus
                });
            }

            return await Task.FromResult(response);
        }
        public override async Task<DeleteTodoResponse> DeleteToDo(DeleteTodoRequest request, ServerCallContext context)
        {
            if (request.Id <=0 )
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource must be greater than 0"));
            var result = await dBContext.TodoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No Item attached with this {request.Id}"));
            var delobj = dBContext.TodoItems.Remove(result);
            await dBContext.SaveChangesAsync();
            return await Task.FromResult(new DeleteTodoResponse
            {
                Id = result.Id
            });
        }
        public override async Task<UpdateTodoResponse> UpdateToDo(UpdateTodoRequest request, ServerCallContext context)
        {
            if(request.Id<=0 || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply valid object"));
            var result = await dBContext.TodoItems.FirstOrDefaultAsync(t => t.Id == request.Id);
            if (result == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"No Item attached with this {request.Id}"));
            result.Description = request.Description;
            result.Title =request.Title;
            result.TodoStatus = request.TodoStatus;
            await dBContext.SaveChangesAsync();
            return await Task.FromResult(new UpdateTodoResponse
            {
                Id =request.Id
            });
        }
    }
}
