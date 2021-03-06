﻿using System;
using AutoMapper;
using SwaggerIntroduction.Models.ApiModels;
using SwaggerIntroduction.Models.DataModels;

namespace SwaggerIntroduction.Models
{
    // ReSharper disable once UnusedMember.Global
    // This is a mapping configuration class used by Automapper
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateUserObjectMap();
            CreateAddressMap();
            CreateRequestTokenUserMap();
            GetUserDetailsMap();
            AddAddressMap();
        }

        private void AddAddressMap()
        {
            CreateMap<AddAddressModel, UserAddress>()
                .ForMember(destination => destination.IsDefaultAddress, opt => opt.MapFrom(scource => scource.MarkAsDefault))
                .ReverseMap()
                .ForMember(destination => destination.MarkAsDefault, opt => opt.MapFrom(source => source.IsDefaultAddress));
        }

        private void CreateAddressMap()
        {
            CreateMap<UserAddress, AddAddressModel>().ReverseMap();
        }

        private void CreateUserObjectMap()
        {
            CreateMap<CreateUserRequestModel, CreateUserResponseModel>();
            CreateMap<CreateUserRequestModel, UserMaster>()
                .ForMember(destination => destination.UserPassword, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(destination => destination.FirstName, opt => opt.Ignore())
                .ForMember(destination => destination.LastName, opt => opt.Ignore());
            CreateMap<CreateUserRequestModel, UserDetails>().
                ForMember(destination => destination.CreationDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();
        }

        private void GetUserDetailsMap()
        {
            CreateMap<UserMaster, GetUserDetailsResponse>()
                .ForMember(destination => destination.CreationDate, opt => opt.Ignore())
                .ForMember(destination => destination.FirstName, opt => opt.Ignore())
                .ForMember(destination => destination.LastName, opt => opt.Ignore());
            CreateMap<UserDetails, GetUserDetailsResponse>()
                .ForMember(destination => destination.UserEmail, opt => opt.Ignore());
        }

        private void CreateRequestTokenUserMap()
        {
            CreateMap<UserMaster, TokenRequestModel>()
                .ForMember(destination => destination.UserEmail, opt => opt.MapFrom(source => source.UserEmail))
                .ForMember(destination => destination.Password, opt => opt.MapFrom(source => source.UserPassword))
                .ReverseMap()
                .ForMember(destination => destination.UserId, opt => opt.Ignore());
        }
    }
}
