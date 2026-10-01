import type Event from '../../models/Event';

import '../../styles/components/ui/carousel.css';

export default function EventItem({ event } : { event: Event }) {

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString("en-GB", {
      day: "numeric",
      month: "short",
      year: "numeric"
    });
  }

  return (
    <div className="event-item" key={event.eventId}>
      <img src={''} alt={event.title} />
      <h3 className='event-title'>{event.title}</h3>
      <p className='event-metadata'>{formatDate(event.startDate)}</p>
      <p className='event-metadata'>{event.city}</p>
    </div>
  );
}