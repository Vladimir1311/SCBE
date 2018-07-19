﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SituationCenter.Shared.ResponseObjects.People;
using SituationCenterCore.Data;

namespace SituationCenterCore.DataFormatting.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, PersonView>();
            CreateMap<ApplicationUser, MeAndRoom>()
                .ForMember(mar => mar.Me, map => map.MapFrom(u => u));
        }
    }
}
