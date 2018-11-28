namespace Eventures.Web.Mapping
{
    using Models.Orders;
    using Models.Events;
    using Eventures.Models;

    using AutoMapper;

    public class EventuresProfile : Profile
    {
        public EventuresProfile()
        {
            CreateMap<Event, EventViewModel>()
                .ForMember(
                    e => e.Name,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(
                    e => e.Start,
                    opt => opt.MapFrom(src => src.Start.ToString("dd-MMM-yy hh:mm:ss")))
                .ForMember(
                    e => e.End,
                    opt => opt.MapFrom(src => src.End.ToString("dd-MMM-yy hh:mm:ss")))
                .ForMember(
                    e => e.Place,
                    opt => opt.MapFrom(src => src.Place));

            CreateMap<Order, MyEventViewModel>()
                .ForMember(
                    o => o.Name,
                    opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(
                    o => o.Start,
                    opt => opt.MapFrom(src => src.Event.Start.ToString("dd-MMM-yy hh:mm:ss")))
                .ForMember(
                    o => o.End,
                    opt => opt.MapFrom(src => src.Event.End.ToString("dd-MMM-yy hh:mm:ss")))
                .ForMember(
                    o => o.Tickets,
                    opt => opt.MapFrom(src => src.TicketsCount));

            CreateMap<Order, OrderViewModel>()
                .ForMember(
                    o => o.EventName,
                    opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(
                    o => o.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.UserName))
                .ForMember(
                    o => o.OrderedOn,
                    opt => opt.MapFrom(src => src.OrderedOn.ToString("dd-MMM-yy hh:mm:ss")));
        }
    }
}
