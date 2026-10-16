import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import heroBg from '../assets/images/navbar-bg.jpg';
import Hero from '../components/ui/Hero';

import concertsImg from '../assets/images/categories/concerts.jpg';
import festivalsImg from '../assets/images/categories/festivals.jpg';
import theatreImg from '../assets/images/categories/theatre.jpg';
import comedyImg from '../assets/images/categories/comedy.jpg';
import sportsImg from '../assets/images/categories/sports.jpg';
import clubsImg from '../assets/images/categories/clubs.jpg';

import '../styles/pages/home.css';

interface UpcomingEvent {
  eventId: number;
  title: string;
  venue: string;
  city: string;
  startDate: string;
  category: string;
  fromPrice: number;
  sellingFast?: boolean;
}

const categories = [
  { label: 'Concerts', image: concertsImg, fallback: '#000000' },
  { label: 'Festivals', image: festivalsImg, fallback: '#000000' },
  { label: 'Theatre', image: theatreImg, fallback: '#000000' },
  { label: 'Comedy', image: comedyImg, fallback: '#000000' },
  { label: 'Sports', image: sportsImg, fallback: '#000000' },
  { label: 'Clubs', image: clubsImg, fallback: '#000000' }
];

const cities = [
  { label: 'London', image: concertsImg, fallback: '#000000' },
  { label: 'Manchester', image: festivalsImg, fallback: '#000000' },
  { label: 'Nottingham', image: theatreImg, fallback: '#000000' },
  { label: 'Leeds', image: comedyImg, fallback: '#000000' },
  { label: 'Liverpool', image: sportsImg, fallback: '#000000' },
  { label: 'Newcastle', image: clubsImg, fallback: '#000000' }
];

// Placeholder data - replace with your API response
const upcomingEvents: UpcomingEvent[] = [
  { eventId: 1, title: 'Event 1', venue: 'O2 Arena', city: 'London', startDate: '2028-09-12', category: 'Concerts', fromPrice: 45, sellingFast: true },
  { eventId: 2, title: 'Event 2', venue: 'Co-op Live', city: 'Manchester', startDate: '2028-09-18', category: 'Festivals', fromPrice: 89 },
  { eventId: 3, title: 'Event 3', venue: 'M&S Bank Arena', city: 'Liverpool', startDate: '2028-09-23', category: 'Comedy', fromPrice: 28.5 },
  { eventId: 4, title: 'Event 4', venue: 'Royal Albert Hall', city: 'London', startDate: '2028-09-28', category: 'Theatre', fromPrice: 35, sellingFast: true },
  { eventId: 5, title: 'Event 5', venue: 'Principality Stadium', city: 'Cardiff', startDate: '2028-09-30', category: 'Sports', fromPrice: 60 },
  { eventId: 6, title: 'Event 6', venue: 'Motion', city: 'Bristol', startDate: '2028-10-04', category: 'Clubs', fromPrice: 15 }
];

const perks = [
  { title: 'Verified tickets', text: 'Every ticket is checked, so you get in on the night.' },
  { title: 'Secure checkout', text: 'Pay with confidence using encrypted card payments.' },
  { title: 'Tickets on your phone', text: 'Get your tickets instantly and show them at the door.' }
];

const getDateParts = (date: string) => {
  const d = new Date(date);
  return {
    day: d.toLocaleDateString('en-GB', { day: 'numeric' }),
    month: d.toLocaleDateString('en-GB', { month: 'short' }),
    weekday: d.toLocaleDateString('en-GB', { weekday: 'short' })
  };
};

const formatPrice = (price: number) =>
  `£${Number.isInteger(price) ? price : price.toFixed(2)}`;

export default function HomePage() {

  const navigate = useNavigate();

  const [searchQuery, setSearchQuery] = useState('');

  const handleSearch = () => {
    if (searchQuery.trim()) {
      navigate(`/search?q=${encodeURIComponent(searchQuery.trim())}`);
    }
  };

  const handleCategory = (category: string) => {
    navigate(`/search?q=${encodeURIComponent(category)}`);
  };

  return (
    <>
      <Hero bgUrl={heroBg}>
        <div className="search-content">
          <h1 className="search-title">Find tickets to the shows you love.</h1>
          <p className="search-subtitle">Concerts, festivals, theatre, comedy and more.</p>
          <div className="search-search">
            <input
              type="text"
              className="form-control"
              placeholder="Search for concerts, shows, festivals..."
              aria-label="Search events"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  handleSearch();
                }
              }}
            />
            <button type="submit" className="button" onClick={handleSearch}>
              Search
            </button>
          </div>
          <div className="search-chips">
            {categories.map((c) => (
              <button
                key={c.label}
                type="button"
                className="search-chip"
                onClick={() => handleCategory(c.label)}
              >
                {c.label}
              </button>
            ))}
          </div>
        </div>
      </Hero>

      <section className="home-section">
        <div className="home-container">
          <h2 className="home-section-title">Browse by category</h2>
          <div className="category-grid">
            {categories.map((c) => (
              <button
                key={c.label}
                type="button"
                className="category-tile"
                style={{
                  backgroundColor: c.fallback,
                  backgroundImage: `url(${c.image})`
                }}
                onClick={() => handleCategory(c.label)}
              >
                <span className="category-label">{c.label}</span>
              </button>
            ))}
          </div>
        </div>
      </section>

      <section className="home-section">
        <div className="home-container">
          <div className="home-section-header">
            <h2 className="home-section-title">Latest events</h2>
            <button type="button" className="home-link" onClick={() => navigate('/search')}>
              See all events
            </button>
          </div>

          <div className="ticket-grid">
            {upcomingEvents.map((event) => {
              const { day, month, weekday } = getDateParts(event.startDate);
              return (
                <article key={event.eventId} className="ticket-card">
                  <div className="ticket-date">
                    <span className="ticket-weekday">{weekday}</span>
                    <span className="ticket-day">{day}</span>
                    <span className="ticket-month">{month}</span>
                  </div>

                  <div className="ticket-body">
                    <div className="ticket-meta">
                      <span className="ticket-category">{event.category}</span>
                      {event.sellingFast && <span className="ticket-badge">Selling fast</span>}
                    </div>
                    <h3 className="ticket-title">{event.title}</h3>
                    <p className="ticket-venue">{event.venue}, {event.city}</p>

                    <div className="ticket-footer">
                      <span className="ticket-price">
                        <span className="ticket-price-label">From</span> {formatPrice(event.fromPrice)}
                      </span>
                      <button
                        type="button"
                        className="button ticket-button"
                        onClick={() => navigate(`/events/${event.eventId}`)}
                      >
                        Get tickets
                      </button>
                    </div>
                  </div>
                </article>
              );
            })}
          </div>
        </div>
      </section>

      <section className="home-section">
        <div className="home-container">
          <h2 className="home-section-title">Browse by city</h2>
          <div className="category-grid">
            {cities.map((c) => (
              <button
                key={c.label}
                type="button"
                className="category-tile"
                style={{
                  backgroundColor: c.fallback,
                  backgroundImage: `url(${c.image})`
                }}
                onClick={() => handleCategory(c.label)}
              >
                <span className="category-label">{c.label}</span>
              </button>
            ))}
          </div>
        </div>
      </section>

      <section className="home-section">
        <div className="home-container">
          <div className="home-section-header">
            <h2 className="home-section-title">On sale this month</h2>
            <button type="button" className="home-link" onClick={() => navigate('/search')}>
              See all events
            </button>
          </div>

          <div className="ticket-grid">
            {upcomingEvents.map((event) => {
              const { day, month, weekday } = getDateParts(event.startDate);
              return (
                <article key={event.eventId} className="ticket-card">
                  <div className="ticket-date">
                    <span className="ticket-weekday">{weekday}</span>
                    <span className="ticket-day">{day}</span>
                    <span className="ticket-month">{month}</span>
                  </div>

                  <div className="ticket-body">
                    <div className="ticket-meta">
                      <span className="ticket-category">{event.category}</span>
                      {event.sellingFast && <span className="ticket-badge">Selling fast</span>}
                    </div>
                    <h3 className="ticket-title">{event.title}</h3>
                    <p className="ticket-venue">{event.venue}, {event.city}</p>

                    <div className="ticket-footer">
                      <span className="ticket-price">
                        <span className="ticket-price-label">From</span> {formatPrice(event.fromPrice)}
                      </span>
                      <button
                        type="button"
                        className="button ticket-button"
                        onClick={() => navigate(`/events/${event.eventId}`)}
                      >
                        Get tickets
                      </button>
                    </div>
                  </div>
                </article>
              );
            })}
          </div>
        </div>
      </section>

      <section className="home-section">
        <div className="home-container">
          <div className="perks-grid">
            {perks.map((perk) => (
              <div key={perk.title} className="perk">
                <h3 className="perk-title">{perk.title}</h3>
                <p className="perk-text">{perk.text}</p>
              </div>
            ))}
          </div>
        </div>
      </section>
    </>
  );
}
