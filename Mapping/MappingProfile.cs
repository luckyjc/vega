using AutoMapper;
using System.Linq;
using vega.Controllers.Resources;
using vega.Models;
using System.Collections.Generic;

namespace vega.Mapping
{
    public class MappingProfile : Profile
    {
         public MappingProfile()
         {
             //Domain to API resource; can cut and paste into separate profiles if app gets larger
             CreateMap<Make, MakeResource>();
             CreateMap<Model, ModelResource>();
             CreateMap<Feature, FeatureResource>();
             CreateMap<Vehicle, VehicleResource>()
                .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContactResource { Name = v.ContactName, Email = v.ContactEmail, Phone = v.ContactPhone}))
                .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => vf.FeatureId)));

             //API resource to domain
             CreateMap<VehicleResource, Vehicle>()
                //this line tells automapper to ignore mapping id property
                .ForMember(v => v.Id, opt => opt.Ignore())
                .ForMember(v => v.ContactName, opt => opt.MapFrom(vr => vr.Contact.Name))
                .ForMember(v => v.ContactEmail, opt => opt.MapFrom(vr => vr.Contact.Email))
                .ForMember(v => v.ContactPhone, opt => opt.MapFrom(vr => vr.Contact.Phone))
                .ForMember(v => v.Features, opt => opt.Ignore())
                .AfterMap((vr, v) => {
                    //remove unselected features
                    var removedFeatures = v.Features.Where(f => !vr.Features.Contains(f.FeatureId));
                    foreach(var f in removedFeatures)
                        v.Features.Remove(f);

                    //add new features
                    var addedFeatures = vr.Features.Where(id => !v.Features.Any(f => f.FeatureId == id)).Select(id => new VehicleFeature {FeatureId = id});
                    foreach(var f in addedFeatures)
                        v.Features.Add(f);
                });
                
         }
    }
}