using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api
{
    public class BooksProfile : Profile  //Automapper profiles are a way to organize mapping configurations
        //it is not a good practice to organise mapping configuration in profiles because
        //configuration inside profile only applies to maps inside that profile.configuration applied to the root configuration applies to all maps created
    {
        public BooksProfile()
        {
            CreateMap<Entities.Book, Models.Book>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"));//concat firstname and lastname and map to Tdestination ie.,models.book


            //add new mapper to map request body to bookentity
            CreateMap<Models.BookForCreation, Entities.Book>();


            CreateMap<Entities.Book, Models.BookWithCovers>()
              .ForMember(dest => dest.Author, opt => opt.MapFrom(src =>
                 $"{src.Author.FirstName} {src.Author.LastName}"));

            CreateMap<IEnumerable<ExternalModels.BookCover>, Models.BookWithCovers>()
                .ForMember(dest => dest.BookCovers, opt => opt.MapFrom(src =>
                   src));
        }
    }
}
