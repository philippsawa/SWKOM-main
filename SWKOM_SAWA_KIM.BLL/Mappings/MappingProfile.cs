using AutoMapper;
using SWKOM_SAWA_KIM.BLL.DTOs;
using SWKOM_SAWA_KIM.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_SAWA_KIM.BLL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Document, DocumentDTO>().ReverseMap();
        }
    }
}
