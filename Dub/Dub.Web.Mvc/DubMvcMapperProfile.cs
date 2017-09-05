namespace Dub.Web.Mvc
{
    using AutoMapper;
    using Dub.Web.Core;
    using Dub.Web.Mvc.Models.Security;

    public class DubMvcMapperProfile : Profile
    {
        public DubMvcMapperProfile()
            : base("DubMvc", Configure)
        {
        }

        private static void Configure(IProfileExpression expression)
        {
            expression.CreateMap<ErrorLog, ErrorLogViewModel>();
        }
    }
}
