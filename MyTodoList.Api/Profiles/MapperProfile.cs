using AutoMapper;
using MyTodoList.Data.Dto;
using MyTodoList.Data.Models;

namespace MyTodoList.Api.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ToDoItem, FullToDoItem>();
            CreateMap<FullToDoItem, ToDoItem>();
            CreateMap<BriefToDoItem, ToDoItem>();
            CreateMap<ToDoItem, BriefToDoItem>();
        }
    }
}
