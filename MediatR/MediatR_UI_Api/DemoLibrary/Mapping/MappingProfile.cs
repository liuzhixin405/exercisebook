using AutoMapper;
using DemoLibrary.Commands;
using DemoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<PersonModel, InsertPersonModel>().ReverseMap();
        }
    }
}
