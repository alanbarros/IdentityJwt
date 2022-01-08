using AutoMapper;
using IdentityJwt.Models;
using System;

namespace IdentityJwt.Infra.Util
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, ApplicationUser>()
                .ForMember(a => a.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(a => a.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ReverseMap()
                .ConstructUsing(a => new(
                    a.UserName,
                    a.Email,
                    a.EmailConfirmed,
                    a.PasswordHash));
        }
    }
}
